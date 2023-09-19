<!-- 
    创建人：
    编辑人：叶登旭（医疗护理模块）
              2022-7-14内容填充 
    功能：医生角色下，查看老人给出的评价，并在此页面进行回复、举报等操作
 -->
<template>
  <div class="views">
    <el-row :gutter="30">
      <el-col>
        <el-card shadow="hover" class="tableStyle">
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
          <el-row :gutter="20">
            <el-col offset="1" >
              <!-- 表格样式 -->
              <el-table
                :data="tableDataShow"
                style="width: 100%"
                v-loading="loading">
                <el-table-column prop="TIME" label="评论时间"  sortable>
                  <template slot-scope="scope">
                    <i class="el-icon-time"></i>
                    <span style="margin-left: 10px">{{ scope.row.TIME }}</span>
                  </template>
                </el-table-column>
                <el-table-column prop="EVALUATOR" label="评价人">
                  <template slot-scope="scope">
                    <i class="el-icon-user"></i>
                    <el-link href="https://www.baidu.com/" target="_blank">{{ scope.row.EVALUATOR }}</el-link>
                  </template>
                </el-table-column>
                <el-table-column prop="GRADE" label="评价分" sortable>
                  <template slot-scope="scope">
                    <i class="el-icon-star-on"></i>
                    <span style="margin-left: 10px">{{ scope.row.GRADE }}</span>
                  </template>
                </el-table-column>
                <!-- 只显示评价的前五个字 -->
                <el-table-column prop="word" label="评价概要" width="180">
                  <template slot-scope="scope">
                    <span>{{scope.row.MESSAGE|ellipsis}}</span>
                  </template>
                </el-table-column>
                <el-table-column fixed="right" label="操作" width="300">
                  <template slot-scope="scope">
                    <el-row :gutter="30">
                      <el-col :span="8">
                        <el-button
                          size="mini"
                          @click="ViewDetail(scope.row)"><i class="el-icon-view el-icon--right"></i>查看详情</el-button>
                      </el-col>
                      <el-col :span="6">
                        <span v-if="bantime!=null">
                          <ReportBox
                            selection="doctorBan" 
                            :orderID="scope.row.SERVICEID"
                            :activeID="DOCTORID"
                            activeName="匿名"
                            :passiveID="scope.row.ELDERID"
                            :passiveName="scope.row.EVALUATOR"></ReportBox>                          
                        </span>
                        <span v-if="bantime==null">
                          <ReportBox
                            selection="doctor" 
                            :orderID="scope.row.SERVICEID"
                            :activeID="DOCTORID"
                            activeName="匿名"
                            :passiveID="scope.row.ELDERID"
                            :passiveName="scope.row.EVALUATOR"></ReportBox>                          
                        </span>
                      </el-col>
                    </el-row>
                  </template>
                </el-table-column>
              </el-table>
            </el-col>
          </el-row>

        </el-card>
      </el-col>
    </el-row>

    <!-- 查看详情的提示框-->
    <el-dialog
      title="评价详情"
      :visible.sync="dialogDetail"
      width="30%"
      :before-close="handleClose">
      <el-row :gutter="20">
        <el-col :span="24">
          <el-card shadow="hover" class="tableStyle">
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
import ReportBox from '../../../components/ReportBox.vue';
export default {
    name:'DoctorReplyToComment',
    components: { ReportBox },
    data() {
      return {
        // 加载动画初始关闭
        loading:false,
        // 初始给出医生的id信息
        DOCTORID:'',
        DOCTORNAME:'',
        // 查看详情弹窗控制
        dialogDetail:false,
        // 接收所有的数据
        tableData: [],
        tableDataShow:[],
        // 储存所有的评价详细信息
        evaluation_detail:{},
        access_token:'',

        // 搜索框的内容
        search:'',
        // 分页的参数初始设置
        currentPage:1,
        pagesize:8,
        totalNum:0,

        bantime:'',
      }
    },
    // 字符处理，大于5个字用省略号表示
    filters:{
      ellipsis(value){
        if(!value)
          return '';
        if(value.length>5)
          return value.slice(0,5)+'...';
        return value;
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
      this.DOCTORID=this.$cookies.get("USERID");
      this.loading=true;
      const _this=this;
      // 由医生id获取其所有的评价
      this.$http
      .get('/API/medical/doctor/evaluation?doctorID='+_this.DOCTORID,{
          headers: {
            TokenValue: this.access_token,
          },
        })
      .then((res)=>{
        _this.tableData=res.data;

        //在页面挂载后调用此方法，确保数据与页面发生了交互
        this.tableDataShow = this.searchResource(); 
        this.totalNum=this.tableData.length;
        this.pageSize=this.tableData.length;

        this.loading=false;
        console.log("kanzhelikanzheli");
        console.log(res);
      })
      .catch((err)=>{
        console.log(err);
        if(err.message === 'Request failed with status code 500' || err.message === 'Request failed with status code 403'){
                    AccessTokenFailed();
                  }
      });
      // 由医生id获取医生的姓名
      this.$http
        .get('/API/Doctor/id?doctorid='+_this.DOCTORID,{
          headers: {
            TokenValue: this.access_token,
          },
        })
        .then((res)=>{
          this.DOCTORNAME=res.data[0].NAME;
          this.bantime=res.data[0].BANTIME;
          console.log("医生的个人信息",res);
          console.log("医生姓名",this.DOCTORNAME)
        })
        .catch((err)=>{
          console.log(err);
          if(err.message === 'Request failed with status code 500' || err.message === 'Request failed with status code 403'){
                    AccessTokenFailed();
                  }
        });
    },
    methods:{
      //查看会议详情
      ViewDetail(row){
        console.log('//////////////////////////');
        console.log(row);
        this.dialogDetail = true;
        // 给弹窗信息赋值
        this.evaluation_detail=row;
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
  },
}
</script>

<style scoped>
/* 表单的样式设计 */
.tableStyle{
  /* 设置圆角 */
  border-radius: 30px;

}
</style>>
