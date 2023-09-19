<!-- 
    创建人：
    编辑人：叶登旭（医疗护理模块）
              2022-7-14内容填充 
              2022-7-22完善页面
    功能：老人角色下，查看自己待评价的服务，并在此页面进评价操作
 -->
<template>
  <div class="views">
    <el-card shadow="hover" class="tableStyle">
      <el-row :gutter="20">
        <el-col :span="15" offset="1">
            <!-- 这里是搜索框，筛选指定的姓名 -->
            <el-input
              v-model="search"
              placeholder="请输入姓名进行搜索">
            </el-input>
        </el-col>
      </el-row>
      <el-divider></el-divider>
      <el-row :gutter="20">
        <el-col :span="22" offset="1">
          <!-- 表格样式 -->
          <el-table
            :data="tableDataShow"
            style="width: 100%"
            v-loading="loading">
            <el-table-column prop="doctor_name" label="医生" width="150">
              <template slot-scope="scope">
                <i class="el-icon-user"></i>
                <span style="margin-left: 10px">{{ scope.row.DOCTORNAME }}</span>
              </template>
            </el-table-column>
            <el-table-column prop="end_time" label="服务结束时间" width="200" sortable>
              <template slot-scope="scope">
                <i class="el-icon-time"></i>
                <span style="margin-left: 10px">{{ scope.row.ENDTIME }}</span>
              </template>
            </el-table-column>
            <el-table-column
              prop="evaluation_status"
              label="评价状态"
              width="100"
              :filters="[{ text: '未评价', value: '未评价' }, { text: '已评价', value: '已评价' }]"
              :filter-method="filterTag"
              filter-placement="bottom-end">
              <template slot-scope="scope">
                <el-tag
                  :type="scope.row.EVALUSTATUS == '未评价' ? 'primary' : 'success'"
                  disable-transitions>{{scope.row.EVALUSTATUS}}</el-tag>
              </template>
            </el-table-column>
            <el-table-column fixed="right" label="操作" width="230">
              <template slot-scope="scope">
                <el-button
                  size="mini"
                  v-if="scope.row.EVALUSTATUS == '未评价' "
                  @click="evaluate(scope.row)"><i class="el-icon-edit"></i>进行评价</el-button>
                <el-button
                  size="mini"
                  v-else
                  @click="viewDetail(scope.row)"><i class="el-icon-view el-icon--right"></i>查看详情</el-button>
              </template>
            </el-table-column>
          </el-table>
        </el-col>
      </el-row>
    </el-card>


    <!-- 进行评价的提示框 -->
    <el-dialog
      title="评价"
      :visible.sync="dialogVisible"
      width="30%"
      :before-close="handleClose">
      <!-- 创建文本域，输入文本 -->
      <el-input
        type="textarea"
        :rows="4"
        placeholder="请输入内容"
        v-model="textarea">
      </el-input>
      <br><br>
      <!-- 选择打分 -->
      <span>您对服务的评分：
        <el-rate
          allow-half
          v-model="slider_score"
          :colors="colors"
          show-text>
        </el-rate>
      </span>
      
      <span slot="footer" class="dialog-footer">
        <el-button @click="dialogVisible = false">取 消</el-button>
        <el-button type="primary" @click="commitEvaluate">提 交</el-button>
      </span>
    </el-dialog>

    <!-- 展示详情的弹窗 -->
    <el-dialog
      title="评价详情"
      :visible.sync="dialogDetail"
      width="30%"
      :before-close="handleClose">
      <el-row :gutter="20">
        <el-col :span="24">
          <el-card shadow="hover" class="detail_form">
            <!-- 显示老人的评价 -->
            <el-descriptions :column="1">
              <el-descriptions-item label="评价内容">{{evaluation_detail.MESSAGE}}</el-descriptions-item>
            </el-descriptions>
            <el-descriptions :column="1">
              <el-descriptions-item label="评价分数">
                <el-rate
                  v-model="evaluation_detail.GRADE"
                  disabled
                  show-score
                  allow-half
                  text-color="#ff9900">
                </el-rate>
              </el-descriptions-item>
            </el-descriptions>
          </el-card>
          <br>          
        </el-col>
      </el-row>
      <!-- 下方的两个按钮 -->
      <span slot="footer" class="dialog-footer">
        <el-row>
          <el-col :span="8">
            <el-button @click="dialogDetail = false" fixed="left">取 消</el-button>
          </el-col>
          <el-col :span="12">
            <el-button type="primary" @click="dialogDetail = false">关 闭</el-button>
          </el-col>
        </el-row>
      </span>
    </el-dialog>
  </div>
</template>

<script>
import { AccessTokenFailed } from "../../../api/data.js"
export default {
    name:'DoctorReplyToComment',
    components: { },
    data() {
      return {
        // 老人的id
        elderID:'',
        // 开始隐藏弹窗
        dialogVisible: false,//评价的弹窗
        dialogDetail:false,//详情的弹窗
        loading:false,
        // 评价的详情信息
        evaluation_detail:{},
        // 所有服务及其评价状态
        tableData: [],
        tableDataShow:[],
        // 临时保存评价对应的服务id
        serveid:'',
        pickerOptions: {
          shortcuts: [{
            text: '本月',
            onClick(picker) {
              picker.$emit('pick', [new Date(), new Date()]);
            }
          }, {
            text: '今年至今',
            onClick(picker) {
              const end = new Date();
              const start = new Date(new Date().getFullYear(), 0);
              picker.$emit('pick', [start, end]);
            }
          }, {
            text: '最近六个月',
            onClick(picker) {
              const end = new Date();
              const start = new Date();
              start.setMonth(start.getMonth() - 6);
              picker.$emit('pick', [start, end]);
            }
          }]
        },
        // 评价信息
        textarea:'',
        slider_score:'',
        // 时间选择器的值
        value2: '',
        access_token:'',

        // 搜索框的内容
        search:'',
        // 分页的参数初始设置
        currentPage:1,
        pagesize:8,
        totalNum:0,
      }
    },
    watch: { //监听搜索框内容，当搜索框内容发生变化时调用searchResource方法
      search: {
        handler() {
          this.tableDataShow= this.searchResource();
          this.totalNum = this.tableDataShow.length;
        }
      }
    },
    computed: {
      //showTable计算属性通过slice方法计算表格当前应显示的数据
      showTable() {
        return this.tableDataShow.slice((this.currentPage - 1) * this.pageSize, this.currentPage * this.pageSize);
      }
    },
    created(){
      this.access_token = this.$cookies.get("token");
      this.loading=true;
      this.elderID=this.$cookies.get("USERID");
      const _this=this;
      // 获取老人所有已完成的服务及其对应的评价状态
      this.$http
        .get('/API/medical/elder/evaluation?elderID='+_this.elderID,{
          headers: {
            TokenValue: this.access_token,
          },
        })
        .then((res)=>{
          console.log(res);
          _this.tableData=res.data;

          //在页面挂载后调用此方法，确保数据与页面发生了交互
          this.tableDataShow = this.searchResource(); 
          this.totalNum=this.tableData.length;
          this.pageSize=this.tableData.length;
        
          this.loading=false;
        })
        .catch((err)=>{
          console.log(err);
          //AccessTokenFailed();
        });
    },
    methods: {
      // 打开评价窗口
      evaluate(row){
        this.serveid=row.SERVICEID;
        this.dialogVisible = true;
      },

      // 提价评价
      commitEvaluate(){
        const _this=this;
        //若是小于10就加个0，后续获取时间需要使用
        function repair(i){
            if (i >= 0 && i <= 9) {
                return "0" + i;
            } else {
                return i;
            }
        }
        // 获取当前时间
        var date = new Date();//当前时间
        var year = date.getFullYear() //年
        var month = repair(date.getMonth() + 1);//月
        var day = repair(date.getDate());//日
        var hour = repair(date.getHours());//时
        var minute = repair(date.getMinutes());//分
        var second = repair(date.getSeconds());//秒
        //构造回传数据
        var returnmes={
          id:this.elderID,
          time:year + "-" + month + "-" + day+ " " + hour + ":" + minute + ":" + second,
          grade:this.slider_score+"",
          message:this.textarea
        };
        // 发送信息
        this.$http
          .post("/API/Evaluation",returnmes,{
          headers: {
            TokenValue: this.access_token,
          },
        })
          .then((res)=>{
            console.log(res);
            console.log();
            // 将评价与服务绑定并更新服务的评价状态
            _this.$http  
              .put("/API/medical/evaluation?serviceid="+_this.serveid+"&evaluationid="+res.data.message.id,null,{
          headers: {
            TokenValue: this.access_token,
          },
        })
              .then((res)=>{
                console.log(res);
              })
              .catch((err)=>{
                console.log(err);
                // AccessTokenFailed();
              });
          })
          .catch((err)=>{
            console.log(err);
            // AccessTokenFailed();
          });
        // 关闭弹窗
        this.dialogVisible=false;
      },

      //查看详情按钮
      viewDetail(row){
        // 为详情页面赋初值
        this.evaluation_detail=row;
        // 启动弹窗
        this.dialogDetail=true;
      },

      // 处理搜索框的内容
      searchResource() {
        this.currentPage = 1; //将当前页设置为1，确保每次都是从第一页开始搜
        const search = this.search;
        if (search) {
          // filter() 方法创建一个新的数组，新数组中的元素是通过检查指定数组中符合条件的所有元素。
          // 注意： filter() 不会对空数组进行检测。
          // 注意： filter() 不会改变原始数组。
          return this.tableData.filter(data => {
            // some() 方法用于检测数组中的元素是否满足指定条件;
            // some() 方法会依次执行数组的每个元素：
            // 如果有一个元素满足条件，则表达式返回true , 剩余的元素不会再执行检测;
            // 如果没有满足条件的元素，则返回false。
            // 注意： some() 不会对空数组进行检测。
            // 注意： some() 不会改变原始数组。
            return Object.keys(data).some(key => {
              // indexOf() 返回某个指定的字符在某个字符串中首次出现的位置，如果没有找到就返回-1；
              // 该方法对大小写敏感！所以之前需要toLowerCase()方法将所有查询到内容变为小写。
              return String(data[key]).toLowerCase().indexOf(search) > -1
            })
          })
        }
        return this.tableData;
      },

      // 以下5个均为是否评价标签的选择器
      resetDateFilter() {
        this.$refs.filterTable.clearFilter('date');
      },
      clearFilter() {
        this.$refs.filterTable.clearFilter();
      },
      formatter(row) {
        return row.address;
      },
      filterTag(value, row) {
        return row.EVALUSTATUS === value;
      },
      filterHandler(value, row, column) {
        const property = column['property'];
        return row[property] === value;
      }
  }
}
</script>


<!-- // lang选择less语法，scoped限制该样式只在本文件使用，不影响其他组件 -->
<style >
/* #select_time{
  width: 400px;
} */

/* 表单的样式设计 */
.tableStyle{
  /* 设置圆角 */
  border-radius: 30px;

}
</style>>
