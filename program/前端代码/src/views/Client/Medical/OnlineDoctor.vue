<!-- 
    创建人：
    编辑人：叶登旭（医疗护理模块）
              2022-7-16内容填充 
    功能：老人角色下，查看已发布的线上问诊信息（与同济云课程查看类似）
 -->
<template>
<div class="views">
  <el-row :gutter="20">
    <el-col :span="22" offset="1">
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
        <!-- 列表展示线上问诊信息 -->
        <el-table
          :data="tableDataShow"
          style="width: 100%"
          v-loading="loading">
          <el-table-column prop="DOCTORNAME" label="主讲医生" width="140">
            <template slot-scope="scope">
              <i class="el-icon-user"></i>
              <span style="margin-left: 10px">{{ scope.row.DOCTORNAME }}</span>
            </template>
          </el-table-column>
          <el-table-column prop="STARTTIME" label="会议开始时间" sortable>
            <template slot-scope="scope">
              <i class="el-icon-time"></i>
              <span style="margin-left: 10px">{{ scope.row.STARTTIME }}</span>
            </template>
          </el-table-column>
          <el-table-column prop="ENDTIME" label="会议结束时间">
            <template slot-scope="scope">
              <i class="el-icon-watch"></i>
              <span style="margin-left: 10px">{{ scope.row.ENDTIME }}</span>
            </template>
          </el-table-column>
          <el-table-column fixed="right" label="操作" width="120">
            <template slot-scope="scope">
              <el-button
                size="mini"
                type="primary"
                @click="viewDetail(scope.row)"><i class="el-icon-view el-icon--right"></i>查看详情</el-button>
            </template>
          </el-table-column>
        </el-table>      
      </el-card>
    </el-col>
  </el-row>

  <!-- 查看详情的弹窗 -->
  <el-dialog 
  class="serviceDialog" 
  title="会议详情" 
  :visible.sync="dialogVisible"
  width="30%">
    <div class="dialog-content">
      <!-- :column="2" 一行有2个 -->
      <el-descriptions class="margin-top" :column="1" :size="size">
        <el-descriptions-item label-class-name="meeting_doctor_name">
          <template slot="label">
            <i class="el-icon-user"></i>
            医生姓名
          </template>
          {{meeting_detail.DOCTORNAME}}
        </el-descriptions-item>

        <el-descriptions-item>
          <template slot="label">
            <i class="el-icon-magic-stick"></i>
            擅长领域
          </template>
          {{meeting_detail.FIELD}}
        </el-descriptions-item>    
        
        <el-descriptions-item>
          <template slot="label">
            <i class="el-icon-time"></i>
            会议开始时间
          </template>
          {{meeting_detail.STARTTIME}}
        </el-descriptions-item>

        <el-descriptions-item>
          <template slot="label">
            <i class="el-icon-time"></i>
            会议结束时间
          </template>
          {{meeting_detail.ENDTIME}}
        </el-descriptions-item>

        <el-descriptions-item>
          <template slot="label">
            <i class="el-icon-data-board"></i>
            会议号
          </template>
          {{meeting_detail.ROOMID}}
        </el-descriptions-item>

        <el-descriptions-item>
          <template slot="label">
            <i class="el-icon-key"></i>
            会议密码
          </template>
          {{meeting_detail.ROOMPASSWORD}}
        </el-descriptions-item>
      </el-descriptions>
    </div>

    <!-- 底部的slot插槽 -->
    <span slot="footer" class="dialogFooter">
      <el-button @click="dialogVisible = false" type="primary">确定</el-button>
      <el-button @click="dialogVisible = false">关闭</el-button>
    </span>
  </el-dialog>
</div>
</template>

<script>
import { AccessTokenFailed } from "../../../api/data.js"
export default {
    name:'OnlineDoctor',
    components: { },
    data() {
        return {
            // 老人的id
            elderid:'',
            // 老人的社区信息
            communityid:'',
            // 开始隐藏弹窗
            dialogVisible: false,
            loading:false,
            //  临时的填充数据
            tableData:[],
            tableDataShow:[],
            // 会议的详细数据，用于弹窗显示
            meeting_detail:{},
            access_token:'',

            // 搜索框的内容
            search:'',
            // 分页的参数初始设置
            currentPage:1,
            pagesize:8,
            totalNum:0,
        };
    },
    created(){
      this.access_token = this.$cookies.get("token");
      // 启动加载动画
      this.loading=true;
      // 获取老人的id
      this.elderid=this.$cookies.get("USERID");
      // 获取老人所在社区
      this.$http
        .get('/API/Elder/'+this.elderid,{
          headers: {
            TokenValue: this.access_token,
          },
        })
        .then((res)=>{
          console.log("老人的信息",res.data.message[0]);
          this.communityid=res.data.message[0].COMMUNITYID;
          // 获取老人所在社区的所有会议信息
          this.$http
            .get('/API/room/community?communityid='+this.communityid,{
              headers: {
                TokenValue: this.access_token,
              },
            })
            .then((res)=>{
              this.tableData=res.data;
              //在页面挂载后调用此方法，确保数据与页面发生了交互
              this.tableDataShow= this.searchResource(); 
              this.totalNum=this.tableData.length;
              this.pageSize=this.tableData.length;
              console.log("会议信息",res);
              this.loading=false;
            })
            .catch((err)=>{
              console.log(err);
              AccessTokenFailed();
            });
        })
        .catch((err)=>{
          console.log(err);
          AccessTokenFailed();
        });  
    },
    watch: { //监听搜索框内容，当搜索框内容发生变化时调用searchResource方法
      search: {
        handler() {
          this.tableDataShow = this.searchResource();
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
    methods: {
      //查看会议详情
      viewDetail(row){
        // 初始化弹窗数据
        this.meeting_detail=row;
        // 启动弹窗
        this.dialogVisible = true;
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

<style lang="less" scoped>
// .meeting_doctor_name{
//   background: #c41b1b;
// }

/* 表单的样式设计 */
.tableStyle{
  /* 设置圆角 */
  border-radius: 30px;

}

</style>>
