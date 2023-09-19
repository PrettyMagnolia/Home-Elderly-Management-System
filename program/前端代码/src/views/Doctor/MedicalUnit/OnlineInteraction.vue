<!-- 
    创建人：
    编辑人：叶登旭（医疗护理模块）2022-7-14内容填充，后续有修改请填写修改内容
    功能：医生角色下，查看自己需要参加的问诊会议
 -->
 
<template>
<div class="views">
  <div>
    <!-- 添加会议的按钮 -->
    <el-row gutter="30">
      <el-col :span="4" offset="20">
        <el-button
        :disabled="bantime!=null"
        @click="startAddMeeting=true">添加会议</el-button>
      </el-col>
    </el-row>
    <el-divider></el-divider>
    <!-- 会议列表 -->
    <el-card shadow="hover" class="tableStyle">
      <el-table
        ref="filterTable"
        v-loading="loading"
        :data="tableData"
        style="width: 100%">
        <el-table-column type="index"></el-table-column>
        <el-table-column prop="STARTTIME" label="会议开始时间" sortable>
          <template slot-scope="scope">
            <i class="el-icon-time"></i>
            <span style="margin-left: 10px">{{ scope.row.STARTTIME }}</span>
          </template>
        </el-table-column>
        <el-table-column prop="STARTTIME" label="会议结束时间">
          <template slot-scope="scope">
            <i class="el-icon-time"></i>
            <span style="margin-left: 10px">{{ scope.row.ENDTIME }}</span>
          </template>
        </el-table-column>
        <!-- 点击查看详情后给出一个弹窗包括医生姓名，擅长领域，会议时间，会议号，会议密码等信息 -->
        <el-table-column fixed="right" label="操作">
            <template slot-scope="scope">
            <el-button
                size="mini"
                type="primary"
                @click="viewDetail(scope.row)"><i class="el-icon-view el-icon--right"></i>查看详情</el-button>
            </template>
        </el-table-column>
      </el-table>
    </el-card>
  </div>

    <!-- 点击查看详情后的弹窗 -->
    <el-dialog class="serviceDialog" title="会议详情" width="30%" :visible.sync="dialogVisible">
      <div class="dialog-content">
        <!-- :column="2" 一行有2个 -->
        <el-descriptions class="margin-top" :column="1">
          <el-descriptions-item>
            <template slot="label">
              <i class="el-icon-user"></i>
              医生姓名
            </template>
            {{this.NAME}}
          </el-descriptions-item>

          <el-descriptions-item>
            <template slot="label">
              <i class="el-icon-magic-stick"></i>
              擅长领域
            </template>
            {{this.FIELD}}
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
            {{meeting_detail.PASSWORD}}
          </el-descriptions-item>
        </el-descriptions>
      </div>

      <!-- 底部的slot插槽 -->
      <span slot="footer" class="dialogFooter">
        <el-button @click="dialogVisible = false" type="primary">确定</el-button>
        <el-button @click="dialogVisible = false">关闭</el-button>
      </span>
    </el-dialog>

    <!-- 点击添加会议按钮后弹窗 -->
    <el-dialog
      title="会议信息设置"
      width="40%"
      :visible.sync="startAddMeeting">
      <el-form 
        :inline="true"
        :model="add_meeting"
        :rules="input_rule"
        ref="add_meeting">
        <el-form-item label="医生姓名">
          <el-input 
            :disabled="true" 
            :placeholder='this.NAME'></el-input>
        </el-form-item>
        <el-form-item prop="roomid">
          <label slot="label">会&nbsp;议&nbsp;号&nbsp;</label>
          <el-input v-model="add_meeting.roomid"></el-input>
        </el-form-item>
        <el-form-item label="会议密码" prop="password">
          <el-input v-model="add_meeting.password"></el-input>
        </el-form-item>
        <el-row>
          <el-col :span="24">
            <el-form-item label="会议起止时间">
              <el-date-picker
                v-model="time"
                type="datetimerange"
                range-separator="至"
                start-placeholder="开始时间"
                end-placeholder="结束时间"
                value-format="yyyy-MM-dd HH:mm">
              </el-date-picker>
            </el-form-item>
          </el-col>
        </el-row>   
      </el-form>

      <span slot="footer" class="dialog-footer">
        <el-row>
          <el-col :span="9">
            <el-button @click="Cancel">取 消</el-button>
          </el-col>
          <el-col :span="10">
            <el-button type="primary" @click="commitMeeting(add_meeting)">提 交</el-button>
          </el-col>
        </el-row>
      </span>
    </el-dialog>
</div>
</template>

<script>
import { AccessTokenFailed } from "../../../api/data.js"
import axios from 'axios';
export default {
    name:'DoctorCheckMeetingToBeAttended',
    components: { },
    data() {
        return {
            // 医生id，初始给出
            DOCTORID: "",
            // 医生姓名和擅长领域，根据id在页面初始获取
            NAME:'',
            FIELD:'',
            // 加载动画初始化
            loading: false,
            access_token:'',
            // 添加会议的弹窗
            startAddMeeting:false,            
            // 查看详情的弹窗
            dialogVisible: false,
            // 会议信息，用于查看详情弹窗接收
            meeting_detail:{
              ROOMID: '',
              STARTTIME: '',
              ENDTIME: '',
              PASSWORD: '',
              NAME:'',
              FIELD:''
            },
            // 表格每一行的信息
            tableData: [],
            // 添加会议时的填充信息，用于向后端发送
            add_meeting:{
              starttime:'',
              endtime:'',
              roomid:'',
              password:'',
              doctorid:''
            },
            // 会议起止时间数组，仅起暂存作用
            time:'',
            bantime:'',

            // 添加会议表格的规则限制
            input_rule:{
              // blur: 失去焦点时触发事件
              roomid:[
                {required:true, message:'请输入会议号', trigger:'blur'},
                {min:8,max:10,message: '长度在8-10个字符' ,trigger:'blur'}],
              password:[
                {required:true, message:'请输入会议密码',trigger:'blur'}],
            },
        };
    },
    // 页面加载时触发该函数
    created(){
      this.access_token = this.$cookies.get("token");
      this.DOCTORID=this.$cookies.get("USERID");
      const _this=this;
      this.loading=true;
      // 获取医生的所有会议信息
      this.$http
        .get('/API/room/doctor?doctorid='+_this.DOCTORID,{
          headers: {
            TokenValue: this.access_token,
          },
        })
        .then((res)=>{
          console.log(res);
          _this.tableData=res.data;
          this.loading=false;
        })
        .catch((err)=>{
          console.log(err);
          if(err.message === 'Request failed with status code 500' || err.message === 'Request failed with status code 403'){
                    AccessTokenFailed();
                  }
        });
      // 获取医生的姓名和擅长领域信息
      this.$http
        .get('/API/Doctor/id?doctorid='+_this.DOCTORID,{
          headers: {
            TokenValue: this.access_token,
          },
        })
        .then((res)=>{
          _this.NAME=res.data[0].NAME;
          _this.FIELD=res.data[0].FIELD;
          this.bantime=res.data[0].BANTIME;
          console.log(res);
        })
        .catch((err)=>{
          console.log(err);
          if(err.message === 'Request failed with status code 500' || err.message === 'Request failed with status code 403'){
                    AccessTokenFailed();
                  }
        });
    },
    methods:{
      // 取消提交会议
      Cancel(){
        this.startAddMeeting = false;
        this.$message({
            type: 'info',
            message: '已取消!'
          });
      },
      // 提交会议按钮
      commitMeeting(){
        const _this=this;
        // 判断输入是否合法
        this.$refs.add_meeting.validate((valid)=>{
          if(valid){
            // 医生id赋值
            this.add_meeting.doctorid=this.DOCTORID;
            // 起止时间赋值
            this.add_meeting.starttime=this.time[0];
            this.add_meeting.endtime=this.time[1];
            // 发送信息
            axios
              .request({
                url:"/API/room",
                method:"post",
                data:_this.add_meeting,
                headers:{TokenValue: this.access_token},
              })
              .then((res)=>{
                console.log(res);
                this.$message({
                  message: '提交成功',
                  type: 'success'
                });
                this.$forceUpdate;
              })
              .catch((err)=>{
                this.$message.error('提交失败');
                console.log(err);
                if(err.message === 'Request failed with status code 500' || err.message === 'Request failed with status code 403'){
                    AccessTokenFailed();
                  }
              });
            // 关闭弹窗
            this.startAddMeeting = false; 
          }else{
            console.log("error");
            return false;
          }
        });
      },

      // 查看详情按钮事件
      viewDetail(row){
        // 启动弹窗
        this.dialogVisible = true;
        // 填充弹窗内容
        this.meeting_detail=row;
      },

      clearFilter() {
        this.$refs.filterTable.clearFilter();
      },
    }
}
</script>

<style scoped>
/* 表单的样式设计 */
.tableStyle{
  /* 设置圆角 */
  border-radius: 30px;

}
</style>
