<template>
  <div class="views">
    <el-row :gutter="20">
      <el-col
        :span="14"
        class="lightgreen-box"
      >
        <!-- 个人信息模块 -->
        <el-card class="box-card">
          <el-row>
            <el-col :span="2">
              <el-avatar :src="avatar_url"></el-avatar>
            </el-col>
            <el-col
              :span="8"
              class="font-size1"
            >
              <!-- 问候语 -->
              你好~护工{{my_info.NAME}}<br>
              <!-- 资质信息 -->
              <el-tag v-if="my_info.INSTITUTIONID===null" type="info" size="mini">尚未完成资质核验</el-tag>

              <el-tag v-else size="mini">{{my_ins_info.NAME}}</el-tag>
            </el-col>
            <el-col
              :offset="7"
              :span="4"
              class="font-size1"
            >
              <!-- 账号状态 -->
              <el-tag
                v-if="my_info.bantime===null || my_info.bantime < Today"
                type="success"
                effect="plain"
              >
                信誉良好，当前账号状态正常
              </el-tag>
              <el-tag
                v-else
                type="danger"
                effect="plain"
              >
                账号封禁中，{{my_info.bantime}}后解封
              </el-tag>
            </el-col>
          </el-row>
        </el-card>
        <br>
        <!-- 通知公告模块 -->
        <el-card class="box-card font-size2">
          <el-row>
            <el-col :span="22">
              通知公告
            </el-col>
            <el-col :span="2">
              <el-button
                type="text"
                @click="JumpToInfo"
              >更多</el-button>
            </el-col>
          </el-row>
          <el-table
            :data="NoticeData"
            stripe
            :show-header="false"
            highlight-current-row
            @current-change="handleCurrentChange"
            style="width: 100%"
          >
            <el-table-column
              prop="TITLE"
              width="460"
            >
            </el-table-column>
            <el-table-column
              prop="TO_CHAR(TIME,'YYYY-MM-DDHH24:MI:SS')"
              width="220"
            >
            </el-table-column>
          </el-table>
        </el-card>
        <br>
        <!-- 优秀员工模块 -->
        <el-card class="box-card font-size2">
          优秀员工
          <ExcellentNurser></ExcellentNurser>
        </el-card>
      </el-col>

      <!-- 日历模块 -->
      <el-col :span="10">
        <el-card class="box-card">
          <div
            style="width: 470px;height: 565px; position: relative;overflow: hidden;">
            <div
              id="abc"
              style="overflow-x: hidden;overflow-y: scroll;position: absolute;">
              <div style="width: 470px;height: 565px;">
                <el-calendar
                  v-model="value"
                  style="height:620px"
                  class="el_cal_out">
                  <div
                    slot="dateCell"
                    slot-scope="{ data }"
                    @click="allcalendar(data)">
                    <p style="margin:0px">
                      {{ data.day.split('-').slice(2).join() }}<br />
                      <!--标记-->
                    <div
                      v-for="(i, index) in WorkDay"
                      :key="index">
                      <!-- 为所有正在进行中的订单日期显示红点 -->
                      <div
                        v-if="data.day==i"
                        class="budge"
                      ></div>
                    </div>
                  </div>
                </el-calendar>
              </div>
            </div>
          </div>
        </el-card>
      </el-col>
    </el-row>

    <!-- 弹窗对话框 用于通知公告 -->
    <el-dialog
      :title="check_row.TITLE"
      :visible.sync="dialogVisible"
      width="30%"
    >
      <span
        style="white-space: pre-wrap;"
        v-text="check_row.CONTENT"
      ></span>
      <span
        slot="footer"
        class="dialog-footer"
      >
        <el-button
          type="primary"
          @click="dialogVisible = false"
        >确 定</el-button>
      </span>
    </el-dialog>
  </div>
</template>

<script>
import { setToken } from '../../../api/data.js'
import ExcellentNurser from '../../../components/ExcellentNurser.vue'
import { AccessTokenFailed } from "../../../api/data.js"

import axios from "../../../api/axios";
import Axios from "axios";

export default {
  name: 'NurseHome',
  components: { ExcellentNurser },
  data () {
    return {
      // 通行token
      access_token: null,

      // 护工的ID
      my_uid: "",
      // 护工的个人信息
      my_info: {},
      // 护工的机构信息
      my_ins_info: {},
      // 护工头像的url 
      avatar_url: "",

      // 公告栏中的数据
      NoticeData:null,
      // 弹窗对话框是否可见
      dialogVisible: false,
      // 公告栏选择的行
      check_row: {},
      
      // 日历模块的数据
      value: new Date(),
      // 获取当天的日期
      Today: "",
      // 护工需要工作的日期
      WorkDay: [],

      form:null,
    }
  },
  methods: {
    /* 后端请求部分方法 */
    // 获取护工的公告
    Get_Nurse_Notice(){
      return axios.request({
          headers: {
              TokenValue: this.access_token,
          },
          url:"/API/nursetips",
          method:'get',
      })
    },
    // 根据uid获取当前护工信息
    getNurseInfo () {
      return axios.request({
        headers: {
          TokenValue: this.access_token,
        },
        url: "/API/NursingWorker" + "/" + this.my_uid,
        method: "get",
      })
    },
    // 根据uid获取当前护工正在进行中的订单（用于日历显示）
    getNurseProgressingRecord () {
      return axios.request({
        headers: {
          TokenValue: this.access_token,
        },
        url: "/API/ServiceRecord",
        method: 'get',
        params: {
          nursingworkerId: this.my_uid,
          status: 1,
        },
      })
    },
    // 获取机构信息
    getInstitutionInfo () {
      return axios.request({
        headers: {
          TokenValue: this.access_token,
        },
        url: "/API/Institution" + "/" + this.my_info.INSTITUTIONID,
        methods: 'get'
      })
    },
    // 获取头像信息
    getMyAvatar () {
      return axios.request({
        headers: {
          TokenValue: this.access_token,
        },
        url: "/API/file" + this.my_info.PHOTO,
        methods: 'get',
        responseType: "blob",
      })
    },

    /* 路由跳转方法 */
    // 跳转到通知栏
    JumpToInfo () {
      this.$router.push({
        name: "NurseNotice",
      });
    },
    // 跳转到我的订单
    allcalendar () {
      this.$router.push({
        name: "TakeOrderMine",
        params: {
          fromHomePage: true,
        }
      });
    },
    
    /* 通知栏方法 */
    // 用户选择某一行后弹出弹窗
    handleCurrentChange (row) {
      console.log(row);
      this.check_row = row;
      this.dialogVisible = true;
    },

    /* 其他方法*/
    // 获取今天的日期
    getToady () {
      const nowDate = new Date();
      const date = {
        year: nowDate.getFullYear(),
        month: nowDate.getMonth() + 1,
        date: nowDate.getDate(),
      }
      const newmonth = date.month > 10 ? date.month : '0' + date.month
      const day = date.date > 10 ? date.date : '0' + date.date
      this.Today = date.year + '-' + newmonth + '-' + day
      console.log(this.Today);
    },
  },
  //挂载时向后端请求数据
  mounted: function () {
    //获取护工的token并传入data.js
    this.access_token = this.$cookies.get('token');
    // 从cookie中获得护工的ID
    this.my_uid = this.$cookies.get("USERID");
    setToken(this.access_token);

    // 设loading
    const loading = this.$loading({
      lock: true,
      text: 'Loading',
      spinner: 'el-icon-loading',
      background: 'rgba(0, 0, 0, 0.7)'
    });
    
    // 获取今天的日期
    this.getToady();

    // 从后端获取数据
    Axios.all([this.getNurseInfo(), this.getNurseProgressingRecord(),this.Get_Nurse_Notice()])
      .then(Axios.spread((resp1, resp2, resp3) => {
        // 个人信息数据
        this.my_info = resp1.data.message[0];
        console.log("my_info",resp1);
        
        // 护工接单数据
        let tmp = resp2.data.message;
        console.log("WorkTIme",resp2);
        // 筛选出工作的日期
        tmp.forEach(element => {
          // 获取到每一条正在进行中的订单的时间
          let time = element.ORDER_TIME.split(" ")[0];
          // 判断是否已经在时间数组中
          if (!this.WorkDay.includes(time)) {
            // 如果不在则push
            this.WorkDay.push(time);
          }
        });

        // 通知公告数据
        this.NoticeData = resp3.data.message.slice(0,3);

        // 根据情况获取机构信息和个人头像
        // 机构信息和个人头像都存在
        if(this.my_info.INSTITUTIONID !== null && this.my_info.PHOTO !== null){
          this.getInstitutionInfo()
          .then(res => {
            this.my_ins_info = JSON.parse(res.data)[0];
            this.getMyAvatar()
            .then(res =>{
              // 获取用户头像
              this.avatar_url = window.URL.createObjectURL(res.data);
              loading.close();
            })
            .catch(err =>{
              // 加载失败时替换的图片
              this.avatar_url = require("../../../assets/Img/ImgError.png");
              console.log(err);
              loading.close();
            })
          })
          .catch(err =>{
            console.log(err);
            if (err.message == "Request failed with status code 403") {
              AccessTokenFailed();
              loading.close();
            }
          })
        }
        //仅存在个人头像
        else if(this.my_info.INSTITUTIONID === null && this.my_info.PHOTO !== null){
          this.getMyAvatar()
          .then(res =>{
            // 获取用户头像
            this.avatar_url = window.URL.createObjectURL(res.data);
            loading.close();
          })
          .catch(err =>{
            // 加载失败时替换的图片
            this.avatar_url = require("../../../assets/Img/ImgError.png");
            console.log(err);
            loading.close();
          })
        }
        // 仅存在机构信息
        else if(this.my_info.PHOTO === null && this.my_info.INSTITUTIONID !== null ){
          this.getInstitutionInfo()
          .then(res => {
            this.my_ins_info = JSON.parse(res.data)[0];
            // 将头像设置为默认头像
            this.avatar_url = require("../../../assets/Img/default_avator.png");
            loading.close();
          })
          .catch(err =>{
            console.log(err);
            if (err.message == "Request failed with status code 403") {
              AccessTokenFailed();
              loading.close();
            }
          })
        }
        // 两者都不存在
        else{
          // 将头像设置为默认头像
          this.avatar_url = require("../../../assets/Img/default_avator.png");
          console.log(this.avatar_url);
          loading.close();
        }
      }))
      .catch(err => {
        console.log(err);
        if (err.message == "Request failed with status code 403") {
          AccessTokenFailed();
          loading.close();
        }
      });
  },
}
</script>

<!-- lang选择less语法，scoped限制该样式只在本文件使用，不影响其他组件 -->
<style lang="less" scoped>
.views{
  .box-card {
    background: white;
    border-radius: 30px;
    .font-size1 {
      font-weight: bold;
      font-family: "Hiragino Sans GB";
      font-size: 16px;
      line-height: 1.5;
    }
  }
  .budge {
    width: 8px;
    height: 8px;
    background: #e6a23c;
    border-radius: 50%;
    margin: 0 auto;
    margin-top: 20px;
  }
}
.font-size2 {
  font-weight: bold;
  font-family: "Hiragino Sans GB";
  font-size: 20px;
  line-height: 1.7;
}
</style>
