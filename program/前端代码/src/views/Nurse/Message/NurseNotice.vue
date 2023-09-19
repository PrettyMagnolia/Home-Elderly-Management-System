
<template>
<div class="views">
    <el-tabs v-model="activeName">
    <el-tab-pane name="first">
        <!-- 设置标签内容 -->
        <template slot="label">  
            <span>通知公告</span>
        </template>
        <!-- 搜索框部分 -->
        <el-card class="card-item">
          <el-row>
            <el-col :span="21" >
              <div class="condition-title">筛选条件</div>
            </el-col>
            <el-col :span="3">
              <el-button size="small" class="title-button" @click="ResetButtonClick"> &ensp;重置&ensp; </el-button>
              <el-button type="primary" size="small" class="title-button" @click="FilterButtonClick"> &ensp;筛选&ensp; </el-button>
            </el-col>
          </el-row>
          <div class="condition-body">
            <el-form :inline="true" :model="formInline" class="condition-form">
              <el-form-item class="conditon-form-item1" label="公告标题">
                <el-autocomplete
                  class="inline-input"
                  :trigger-on-focus="false"
                  popper-class="el-autocomplete-suggestion"
                  v-model="filter_key"
                  :fetch-suggestions="ReturnInputPrompt"
                  placeholder="请输入"
                  size="small"
                ></el-autocomplete>
              </el-form-item>
              <el-form-item class="conditon-form-item2" label="发布时间">
                <el-date-picker
                  v-model="filter_time"
                  size="small"
                  type="daterange"
                  range-separator="至"
                  start-placeholder="开始日期"
                  end-placeholder="结束日期"
                  value-format="yyyy-MM-dd" >
                </el-date-picker>
              </el-form-item>
            </el-form>
          </div>
        </el-card>
        <!-- 消息列表部分 -->
        <el-card class="card-item">
          <div style="height:400px;">
            <el-table
              :data="NoticeData"
              stripe
              style="width: 100%">
              <el-table-column
                prop="TITLE"
                label="公告标题"
                width="500">
                <template slot-scope="scope">
                  <div class="status-point"></div>
                    {{scope.row.TITLE}}
                </template>
              </el-table-column>
              <el-table-column
                prop="TO_CHAR(TIME,'YYYY-MM-DDHH24:MI:SS')"
                label="发布日期"
                width="360"
                sortable>
              </el-table-column>
              <el-table-column
                prop="STATUS"
                label="状态">
                <template slot-scope="scope">
                  <el-tag :type="scope.row.STATUS === '已发布' ? 'success' : 'danger'" disable-transitions>
                    {{scope.row.STATUS}}
                  </el-tag>
                </template>
              </el-table-column>
              <el-table-column align="center">
                  <template slot-scope="scope">
                    <el-button
                      @click="ViewDetailButtonClick(scope.row)"
                      type="primary"
                      size="mini">
                      查看详情
                    </el-button>
                  </template>
              </el-table-column>
            </el-table>
          </div>
          <!-- 分页栏 -->
          <div class="form-page">
            <el-pagination
              background
              layout="prev, pager, next"
              :page-size="page_size"
              :total="total_items"
              :current-page="current_page"
              @current-change="HandlePageChange"
            >
            </el-pagination>
          </div>  
        </el-card>
    </el-tab-pane>
    
    </el-tabs>
    <el-dialog
      :title="check_row.TITLE"
      :visible.sync="dialogVisible"
      width="30%">
      <span style="white-space: pre-wrap;" v-text="check_row.CONTENT"></span>
      <span slot="footer" class="dialog-footer">
        <el-button type="primary" @click="dialogVisible = false">确 定</el-button>
      </span>
    </el-dialog>
</div>
</template>

<script>
import py from "pinyin"
import axios from "../../../api/axios"
export default {
    name:'NurseNotice',
    components: { },
    data() {
        return {
            page_size:10,
            dialogVisible:false,
            filter_key:null,
            filter_time:null,
            check_row:{},
            // 列表中的数据
            NoticeData:null,
            // 消息的数量
            NoticeCount:4,
            activeName: 'first',
            input1:'',
            formInline: {
                user: '',
                region: '',
                data1:'',
                data2:''
            },
            value1:'',
            current_page:1,
            total_page:10,
            getOrderPage_params: {
              pageNum: 1,
              amount: 100, //不变
              sortOrder: "descend",
              sortKey: "work_time",
              state: "all",
            },
            NoticeData_copy:{},
            
        };
    },
    computed:{
      total_items: function() {
        return this.NoticeData.length;
      }
    },
    methods: {
      Get_Nurse_Notice(){
        return axios.request({
            headers: {
                TokenValue: this.access_token,
            },
            url:"/API/nursetips",
            method:'get',
        })
      },
      ViewDetailButtonClick(row){
        this.check_row = row;
        this.dialogVisible = true;
      },
      // el-autocomplete组件返回输入提示
      ReturnInputPrompt(queryString, cb) {
        // 存放返回结果的数组
        let results = [];
        // 将查询字符分解并转为正则表达式，用于模糊搜索
        let queryStringArr = queryString.split("");
        let str = "(.*?)";
        let regStr = str + queryStringArr.join(str) + str;
        let reg = RegExp(regStr, "i"); // 以mh为例生成的正则表达式为/(.*?)m(.*?)h(.*?)/i

        // search_info目前只有地址和老人姓名信息
        this.NoticeData.some(element => {
          Object.keys(element).some(key => {
            if(key !== "TITLE")
              return false;
            let val = element[key];
            // 获取汉字的拼音，并进行扁平化
            let pyArr = py(val, {
              style: py.STYLE_NORMAL // 设置拼音风格设置为普通风格（不带声调），
            }).flat();
            let pyStr = pyArr.join("");

            // 拼音符合正则表达式 || 文字符合正则表达式
            if((reg.test(pyStr)) || (reg.test(val))){
              let re = false;
              results.some(element => {
                if(element.value === val){
                  // 存在重复的
                  re = true;
                  return true;
                }
              });
              // 创建符合组件要求的对象（一定要有value值）
              if(!re){
                let obj = {value:val};
                results.push(obj);
              }
              // 跳出遍历
              return true;
            }
          })
        })
        // 调用 callback 返回建议列表的数据
        cb(results);
      },
      // 在页面改变时进行的操作
      HandlePageChange(currentPage) {
        // 更改当前页面
        this.current_page = currentPage;
      },
      // 点击筛选框中的筛选按钮的操作
      FilterButtonClick(){
        if(this.filter_time === null && this.filter_key ===null){
          this.NoticeData = this.NoticeData_copy;
        }
        else if(this.filter_time === null){
          // 根据关键词来进行筛选
          console.log("ys");
          this.NoticeData = this.FilterDataByKey();
        }
        else if(this.filter_key === null){
          // 根据时间进行筛选
          this.NoticeData = this.FilterDataByTime();
        }
        else{
          // 根据时间进行筛选
          this.NoticeData = this.FilterDataByTime();
          // 根据关键词进行筛选
          this.NoticeData = this.FilterDataByKey();
        }
      },
      // 根据筛选栏中的关键词搜索数据（对象是全部数据）
      FilterDataByKey(){
        let queryString = this.filter_key;
        // 如果关键词为空，则返回所有数据
        if(queryString === ""){
          return this.all_care_orders;
        }
        let results = [];
        // 将查询字符分解并转为正则表达式，用于模糊搜索
        let queryStringArr = queryString.split("");
        let str = "(.*?)";
        let regStr = str + queryStringArr.join(str) + str;
        let reg = RegExp(regStr, "i"); // 以mh为例生成的正则表达式为/(.*?)m(.*?)h(.*?)/i

        this.NoticeData.some(element => {
          Object.keys(element).some(key => {
            if(key !== "TITLE")
              return false;
            let val = element[key];
            // 获取汉字的拼音，并进行扁平化
            let pyArr = py(val, {
              style: py.STYLE_NORMAL // 设置拼音风格设置为普通风格（不带声调），
            }).flat();
            let pyStr = pyArr.join("");

            // 拼音符合正则表达式 || 文字符合正则表达式
            if((reg.test(pyStr)) || (reg.test(val))){
              // 创建符合组件要求的对象（一定要有value值）
              results.push(element);
              // 跳出遍历
              return true;
            }
          })
        })
        return results;
      },
      // 根据筛选栏中的时间范围搜索数据（对象是全部数据）
      FilterDataByTime(){
        let start = this.filter_time[0];
        let end = this.filter_time[1];
        console.log(start,end);
        let results = this.NoticeData.filter((e) => {
          return e.TO_CHAR(TIME,'YYYY-MM-DDHH24:MI:SS') >= start && e.TO_CHAR(TIME,'YYYY-MM-DDHH24:MI:SS') <= end
        })
        return results;
      },
      // 点击筛选框中的重置按钮的操作
      ResetButtonClick(){
        // 清空搜索框中的内容
        this.filter_key = null;
        this.filter_time = null;
        // 重置数据
        this.NoticeData = this.NoticeData_copy;
      },
    },
    mounted: function () {
      this.Get_Nurse_Notice()
      .then(res =>{
        this.NoticeData = res.data.message;
        this.NoticeData_copy = this.NoticeData;
        console.log(this.NoticeData);
      })
      
    },
    
 }
</script>

<!-- lang选择less语法，scoped限制该样式只在本文件使用，不影响其他组件 -->
<style lang="less" scoped>
.views{
  .card-item {
    background:white;
    border-radius: 10px;
    margin-bottom: 15px;
    padding: 5px;
    .condition-title {
      color: #2b3b4e;
      font-size: 18px;
      font-weight: 700;
      line-height: 24px;
      margin-bottom: 15px;
      .title-button /deep/ {
        min-width: 80px;
      }
    }
    .condition-body{
      display: flex;
      margin-left: 60px;
      align-items: center;
      .condition-form{
        margin-bottom:0px;
        .conditon-form-item1 
        {
          margin-bottom:0px;
          /deep/ #input1{
            width: 300px;
          }
        }

        .conditon-form-item2{
          margin-bottom: 0px;
          margin-right: 100px;
          margin-left: 100px;
        }
      }
    }
    .form-page {
      margin-top: 20px;
      display: flex;
      flex-direction: column;
      justify-content: center;
      align-items: center;
    }
  }
  .status-point {
    display: inline-block;
    width: 6px;
    height: 6px;
    border-radius: 50%;
    background-color:red;
    margin-right: 5px;
  }
}

</style>>
