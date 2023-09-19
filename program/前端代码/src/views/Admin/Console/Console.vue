
<template>
  <div class="views">

    <router-view></router-view>
    <el-row>
      <el-col
        :span="14"
        class="lightgreen-box"
      >
        <el-card class="box-card">
          <el-row>
            <el-col :span="2">
              <el-avatar
                src="https://cube.elemecdn.com/0/88/03b0d39583f48206768a7534e55bcpng.png"
              ></el-avatar>
            </el-col>
            <el-col
              :span="12"
              class="font-size1"
            >
              你好~{{name}} <font size="1">{{role}}</font><br>
              <el-tag size="mini">{{position}}</el-tag>
            </el-col>
            <el-col
              :span="10"
              class="font-size1"
            >

            </el-col>
          </el-row>
        </el-card>
        <br>
        <el-card class="box-card font-size2">
          <div style="margin-bottom:20px">信息管理栏</div>
          <div
            style="margin-left:30px;margin-bottom:15px"
            @click="toSearch()"
          >
            信息管理
            <el-button
              type="primary"
              icon="el-icon-edit"
            ></el-button>
          </div>
        </el-card>
        <br>
        <el-card class="box-card font-size2">
          <div style="margin-bottom:30px">工作台</div>
          <!--护工资质审核-->
          <div style="margin-bottom:20px">
            医护工作人员资质审核<br>
            <el-tooltip
              placement="top"
              class=scanText
            >
              <span slot="content"></span>
              <el-button @click="toCarerQualify()">待审核：{{CarerNum}}</el-button>
            </el-tooltip>
          </div>

          <!--举报单审核-->
          <div style="margin-bottom:20px">
            举报单审核<br>
            <el-tooltip
              placement="top"
              class=scanText
            >
              <span slot="content"></span>
              <el-button @click="toReportScan()">待审核：{{ReportNum}}</el-button>
            </el-tooltip>
          </div>

          <!--机构入驻审核-->
          <div style="margin-bottom:20px">
            机构入驻审核<br>
            <el-tooltip
              placement="top"
              class=scanText
            >
              <span slot="content"></span>
              <el-button @click="toOrg()">待审核：{{OrgNum}}</el-button>
            </el-tooltip>
          </div>
        </el-card>
      </el-col>

      <!--下面是日历-->
      <el-col :span="10">
        <el-card class="box-card">
          <div
            style="width: 470px;height: 630px; position: relative;overflow: hidden;"
          >
            <div
              id="abc"
              style="overflow-x: hidden;overflow-y: scroll;position: absolute;"
            >
              <div style="width: 470px;height: 565px;">
                <el-calendar v-model="value">
                </el-calendar>
              </div>
            </div>
          </div>
        </el-card>
      </el-col>
    </el-row>

    <!--<ReportBox selection='doctor'></ReportBox>-->

    <!--<el-button @click="testAuditLog()">AuditLog接口测试</el-button>-->

  </div>
</template>

<script>
import { setToken, AccessTokenFailed } from '../../../api/data.js'
import { getReportAuditLog, getInstitutionAuditLog, getQualificationAuditLog } from '../../../api/data.js'
import { getManager } from '../../../api/data.js'
import ReportBox from '../../../components/ReportBox.vue'
import Axios from "axios";

export default {
  name: 'Console',
  components: { ReportBox },
  data () {
    return {
      flag: true,
      CarerNum: 0,
      ReportNum: 0,
      OrgNum: 0,
      name: "无数据",
      role: "无数据",
      position: "无数据",
      ID: 222022080700000001,
      value: '',
      flag: false,
    };
  },
  methods: {
    toCarerQualify () {
      this.dialogVisible = false;
      this.$router.push({
        name: "CarerQualify",
      })
    },
    toReportScan () {
      this.dialogVisible = false;
      this.$router.push({
        name: "ReportScan",
      })
    },
    toSearch () {
      this.dialogVisible = false;
      this.$router.push({
        name: "Search",
      })
    },
    toOrg () {
      this.dialogVisible = false;
      this.$router.push({
        name: "OrgQualify",
      })
    },
    testAuditLog () {
      var access_token = this.$cookies.get('token');
      setToken(access_token);
      this.getConsoleNum();
    },
    button_close (data) {
      this.flag = data;
    },
    getConsoleNum () {
      Axios.all([getReportAuditLog(), getInstitutionAuditLog(), getQualificationAuditLog()])
        .then(Axios.spread((resReport, resInstitution, resQualification) => {
          let list1 = JSON.parse(resReport.data)[0];
          for (let i in list1)
            this.ReportNum = list1[i];

          let list2 = JSON.parse(resInstitution.data)[0];
          for (let i in list2)
            this.OrgNum = list2[i];

          let list3 = JSON.parse(resQualification.data)[0];
          for (let i in list3)
            this.CarerNum = list3[i];

        }))
        .catch((err) => {
          if (err.message == "Request failed with status code 403" && this.flag) {
            this.flag = false;
            AccessTokenFailed();
          }
          console.log("获取待审核数目出错！");
          console.log(err);
        })

    },

  },
  //初始向后端请求数据
  mounted: function () {
    const loading = this.$loading({
      lock: true,
      text: 'Loading',
      spinner: 'el-icon-loading',
      background: 'rgba(0, 0, 0, 0.7)'
    });

    var access_token = this.$cookies.get('token');
    setToken(access_token);
    console.log(access_token);


    // 从cookie中获得护工的ID
    this.ID = this.$cookies.get("USERID");

    //获取待审核数目
    console.log("hahahaha")
    this.getConsoleNum();

    //获取管理员个人信息
    getManager(this.ID)
      .then((res) => {
        let item = JSON.parse(res.data)[0];

        this.name = item.NAME;
        this.role = "管理员";
        this.position = item.POSITION;
        loading.close();
      })
      .catch((err) => {
        if (err.message == "Request failed with status code 403" && this.flag) {
          this.flag = false;
          AccessTokenFailed();
        }

        console.log("后端请求失败！");
        console.log(err);
        loading.close();
      })



  }
}
</script>

// lang选择less语法，scoped限制该样式只在本文件使用，不影响其他组件
<style lang="less" scoped>
    .upPanel{
        width:500px;
        height:200px;
        border:0px solid #000;
        margin-right:50px;
        background-color:#EDEDED;
        float:left;
        box-shadow: 0 2px 4px rgba(0, 0, 0, .12), 0 0 6px rgba(0, 0, 0, .04)
        //display:inline-block;
    }

    .upPanel2{
        width:500px;
        height:200px;
        border:0px solid #000;
        margin-right:50px;
        background-color:#EDEDED;
        float:right;
        box-shadow: 0 2px 4px rgba(0, 0, 0, .12), 0 0 6px rgba(0, 0, 0, .04)
        //display:inline-block;
    }
    

    .downPanel{
        width:1120px;
        height:400px;
        border:0px solid #000;
        background-color:#F4F4F4;
        float:left;
        margin-top:30px;
        box-shadow: 0 2px 4px rgba(0, 0, 0, .12), 0 0 6px rgba(0, 0, 0, .04)
    }

    //下面的是抄的
    .scanText{
        margin-left:80px;
    }

    .box-card {
    background:white;
    border-radius: 30px;
  }
.font-size1{
    font-weight: bold;
    font-family: "Hiragino Sans GB";
    font-size: 16px;
    line-height:1.5;
}
.font-size2{
    font-weight: bold;
    font-family: "Hiragino Sans GB";
    font-size: 20px;
    line-height:1.7;
}

.el-carousel__item h3 {
    color: #475669;
    font-size: 14px;
    opacity: 0.75;
    line-height: 10px;
    margin: 0;
}

.el-carousel__item:nth-child(2n) {
    background-color: #99a9bf;
}

.el-carousel__item:nth-child(2n+1) {
    background-color: #d3dce6;
}
</style>
