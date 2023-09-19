<!-- 
  创建人：
  编辑人：叶登旭（医护模块）
    7-28重写该界面
  功能：老人角色下，查看自己的健康报告
 -->
<template>
  <div class="views">
    <!-- 报告展示的列表 -->
    <el-row :gutter="20">
      <el-col :span="24">
        <el-card shadow="hover" class="tableStyle">
          <!-- 正文部分，前面是排版设计 -->
          <el-row :gutter="20">
            <el-col :span="10" offset="1">
              <!-- 这里是搜索框，筛选指定的姓名 -->
              <el-input
                v-model="search"
                placeholder="请输入人名进行搜索">
              </el-input>
            </el-col>
          </el-row>
          <el-divider></el-divider>
          <!-- 报告面板 -->
          <el-row :gutter="20">
            <el-col :span="22" offset="1">
              <el-table
                ref="filterTable"
                :data="report_abstract_show"
                style="width: 100%"
                v-loading="loading">
                <el-table-column prop="TIME" label="时间" sortable>
                  <template slot-scope="scope">
                    <i class="el-icon-time"></i>
                    <span style="margin-left: 10px">{{ scope.row.TIME }}</span>
                  </template>
                </el-table-column>
                <el-table-column prop="DOCTORNAME" label="医生">
                  <template slot-scope="scope">
                    <i class="el-icon-user"></i>
                    <span style="margin-left: 10px">{{ scope.row.DOCTORNAME }}</span>
                  </template>
                </el-table-column>
                <el-table-column fixed="right" label="操作" width="120">
                  <template slot-scope="scope">
                    <el-button
                    size="mini"
                    @click="viewDetail(scope.row)"><i class="el-icon-view el-icon--right"></i>查看详情</el-button>
                  </template>
                </el-table-column>
              </el-table>
            </el-col>
          </el-row>
        </el-card>
      </el-col>
    </el-row>
    
    <!-- 查看详情的对话框 -->
    <el-dialog
    title="报告单"
    :visible.sync="dialogReportDetail"
    width="50%">
      <el-form ref="form" :model="report_detail" label-width="80px">
        <!-- 描述列表 -->
        <el-row>
          <el-col :span="11">
            <el-form-item label="医生">
              <el-input :disabled="true" class="report_input" v-model="report_detail.doctor" :placeholder="doctor_mes.doctor_name">
              </el-input>
            </el-form-item>
          </el-col>
          <el-col :span="11">
            <el-form-item label="时间">
              <el-input :disabled="true" class="report_input" v-model="report_detail.time" :placeholder="doctor_mes.time">
              </el-input>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row label="个人信息"> 
          <el-col :span="8">
            <el-form-item label="姓名">
              <el-input :disabled="true" class="report_input" v-model="report_detail.NAME" :placeholder="report_detail.NAME">
              </el-input>
            </el-form-item>
          </el-col>
          <el-col :span="6">
            <el-form-item label="年龄">
              <el-input :disabled="true" class="report_input" v-model="report_detail.age" :placeholder="report_detail.AGE">
              </el-input>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="性别">
              <el-input :disabled="true" class="report_input" v-model="report_detail.sex" :placeholder="report_detail.GENDER">
              </el-input>
            </el-form-item>
          </el-col>          
        </el-row>
        <el-row>
          <el-col :span="7">
            <el-form-item label="身高">
              <el-input :disabled="true" class="report_input" v-model="report_detail.height" :placeholder="report_detail.HEIGHT">
              </el-input>
            </el-form-item>
          </el-col>
          <el-col :span="6">
            <el-form-item label="体重">
              <el-input :disabled="true" class="report_input" v-model="report_detail.weight" :placeholder="report_detail.WEIGHT">
              </el-input>
            </el-form-item>
          </el-col>
          <el-col :span="8" offset="1">
            <el-form-item label="自理情况">
              <el-input :disabled="true" class="report_input" v-model="report_detail.status" :placeholder="report_detail.SITUATION">
              </el-input>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="11">
            <el-form-item label="心率">
              <el-input :disabled="true" class="report_input" v-model="report_detail.heart_rate" :placeholder="report_detail.RATE">
              </el-input>
            </el-form-item>
          </el-col>
          <el-col :span="11">
            <el-form-item label="血压">
              <el-input :disabled="true" class="report_input" v-model="report_detail.blood_pressure" :placeholder="report_detail.PRESSURE">
              </el-input>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="11">
            <el-form-item label="健康评估" prop="healthassess">
              <el-input :disabled="true" class="report_input" v-model="report_detail.healthassess" 
              :placeholder="report_detail.HEALTHASSESS">
              </el-input>
            </el-form-item>
          </el-col>
          <el-col :span="11">
            <el-form-item label="建议" prop="advice">
              <el-input :disabled="true" class="report_input" v-model="report_detail.advice" 
              :placeholder="report_detail.ADVICE">
              </el-input>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="22" offset="1">
            <el-button class="btn_hisdis"
              type="primary"
              round
              @click="gotoHistoryDis">
              <i class="el-icon-view el-icon--right"></i>点击查看既往病史
            </el-button>
          </el-col>
        </el-row>
      </el-form>

      <span slot="footer" class="dialog-footer">
        <el-button @click="dialogReportDetail = false">关 闭</el-button>
        <el-button type="primary" @click="dialogReportDetail = false">确 定</el-button>
      </span>
    </el-dialog>
  </div>
</template>

<script>
import { AccessTokenFailed } from "../../../api/data.js"
export default {
  name: "CareServiceMine",
  components: {},
  data() {
    return {
      // 初始获得老人的id
      elderid:'',
      // 报告弹窗的详细数据
      report_detail:{},
      // 医生的部分信息
      doctor_mes:{
        doctor_name:'',
        time:''
      },
      // 对弹窗进行初始化
      dialogReportDetail:false,
      loading:false,

      // 报告摘要的数据
      report_abstract:[],
      report_abstract_show:[],

      // 保存既往疾病
      history_disease:[],
      access_token:'',

      // 搜索框的内容
      search:'',
      // 分页的参数初始设置
      currentPage:1,
      pagesize:8,
      totalNum:0,
    }
  },
  created(){
    this.access_token = this.$cookies.get("token");
    this.loading=true;
    this.elderid=this.$cookies.get("USERID");
    const _this=this;
    // 根据老人id获取健康报告的简略信息
    this.$http
      .get("/API/health/elder?elderid="+_this.elderid,{
          headers: {
            TokenValue: this.access_token,
          },
        })
      .then((res)=>{
        this.report_abstract=res.data;

        //在页面挂载后调用此方法，确保数据与页面发生了交互
        this.report_abstract_show = this.searchResource(); 
        this.totalNum=this.report_abstract.length;
        this.pageSize=this.report_abstract.length;
        console.log("获取健康报告的简要信息",res);
        this.loading=false;
      })
      .catch((err)=>{
        AccessTokenFailed();
        console.log(err);
      });
  },
  watch: { //监听搜索框内容，当搜索框内容发生变化时调用searchResource方法
      search: {
        handler() {
          this.report_abstract_show = this.searchResource();
          this.totalNum = this.report_abstract_show.length;
        }
      }
    },
  computed: {
    //showTable计算属性通过slice方法计算表格当前应显示的数据
    showTable() {
      return this.report_abstract_show.slice((this.currentPage - 1) * this.pageSize, this.currentPage * this.pageSize);
    }
  },
  methods: {
    // 报告单中跳转到既往疾病页面
    gotoHistoryDis(){
      let jumpBack={
        backName : 'HReport'
      };
      //跳转到 页面
      this.$router.push({
        name: "HistoryDisease",
        params: {jumpBack:jumpBack},
      });
    },

    // 查看报告单详情界面
    viewDetail(row){
      const _this=this;
      this.doctor_mes.doctor_name=row.DOCTORNAME;
      this.doctor_mes.time=row.TIME;
      // 根据本行的健康报告id获取详细信息
      this.$http
        .get("/API/health/reportid?reportid="+row.REPORTID,{
          headers: {
            TokenValue: this.access_token,
          },
        })
        .then((res)=>{
          _this.report_detail=res.data[0];
          console.log("健康报告的详细信息",res);
        })
        .catch((err)=>{
          console.log(err);
          AccessTokenFailed();
        }); 
      // 启动弹窗
      this.dialogReportDetail=true;
    },

    // handleSizeChange(psize) {
    //     this.pageSize = psize;
    //   },

    // handleCurrentChange(currentPage) {
    //   this.currentPage = currentPage;
    // },

    clearFilter() {
        this.$refs.filterTable.clearFilter();
      },

    // 处理搜索框的内容
    searchResource() {
        this.currentPage = 1; //将当前页设置为1，确保每次都是从第一页开始搜
        const search = this.search;
        if (search) {
          // filter() 方法创建一个新的数组，新数组中的元素是通过检查指定数组中符合条件的所有元素。
          // 注意： filter() 不会对空数组进行检测。
          // 注意： filter() 不会改变原始数组。
          return this.report_abstract.filter(data => {
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
        return this.report_abstract;
      },
  },
};
</script>

<style lang="less" scoped>
// 既往疾病卡片头部的样式
// .card-header{

// }

/* 表单的样式设计 */
.tableStyle{
  /* 设置圆角 */
  border-radius: 30px;

}

.btn_hisdis{
  text-align:center;
  margin:0 auto;
}
</style>>
