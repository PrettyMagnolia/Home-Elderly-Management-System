
<template>
  <div class="views">
    <h1>举报单审核</h1>
    <div style="margin-left:50px">

    </div>

    <el-form
      ref="form"
      :model="form"
      label-width="150px"
      style="margin-left:60px"
      v-loading="loading"
    >
      <el-form-item label="举报单ID">
        {{form.ID}}
      </el-form-item>
      <el-form-item label="举报人ID">
        {{form.IMFORMERID}}
      </el-form-item>
      <el-form-item label="举报人姓名">
        {{form.IMFORMERNAME}}
      </el-form-item>
      <el-form-item label="被举报人ID">
        {{form.DEFENDANTID}}
      </el-form-item>
      <el-form-item label="被举报人姓名">
        {{form.DEFENDANTNAME}}
      </el-form-item>
      <el-form-item label="订单ID">
        {{form.ORDERID}}
      </el-form-item>
      <el-form-item label="举报原因">
        {{form.TYPE}}
      </el-form-item>
      <el-form-item label="举报详述">
        {{form.REPORTTEXT}}
      </el-form-item>
      <el-form-item label="举报时间">
        {{form.REPORTTIME}}
      </el-form-item>
      <el-form-item label="举报截图">
        <div class="demo-image">
          <div class="block">
            <span class="demonstration"></span>
            <el-image :src="image_url"></el-image>
          </div>
        </div>
      </el-form-item>
      <!--过错方认定-->
      <el-form-item label="是否属实">
        <el-radio
          v-model="form.ISREAL"
          label="是"
        >是</el-radio>
        <el-radio
          v-model="form.ISREAL"
          label="否"
        >否</el-radio>
      </el-form-item>

      <!--惩罚类型-->
      <el-form-item label="惩罚类型">
        <el-select
          v-model="form.PUNITIVEMEASURE"
          placeholder="请选择"
        >
          <el-option
            v-for="item in punishment"
            :key="item.value"
            :label="item.label"
            :value="item.value"
          >
            <span style="float: left">{{ item.label }}</span>
            <span
              style="float: right; color: #8492a6; font-size: 13px">{{ item.value }}</span>
          </el-option>
        </el-select>
      </el-form-item>

      <el-form-item align=center>
        <el-button
          type="primary"
          @click="onSubmit()"
        >提交</el-button>
        <el-button @click="Cancel()">取消</el-button>
      </el-form-item>
    </el-form>
  </div>
</template>

<script>
import { AccessTokenFailed } from '../../../api/data.js'
import { putDoctorDisable, putReportCard, putNurseDisable, getNurseCard, getDoctorCard } from '../../../api/data.js'
import axios from "../../../api/axios";
export default {
  name: "DetailReport",
  components: {},
  data () {
    return {
      flag: true,
      image_url: '',
      access_token: this.$cookies.get("token"),
      punishment: [
        {
          value: '0',
          label: '不作惩罚'
        },
        {
          value: '1',
          label: '封号1天'
        }, {
          value: '3',
          label: '封号3天'
        }, {
          value: '7',
          label: '封号7天'
        }, {
          value: '14',
          label: '封号14天'
        }, {
          value: '30',
          label: '封号30天'
        }, {
          value: '90',
          label: '封号90天'
        }],

      faultSide: '',
      punishType: '',
      form: {},
      his_obj: {},
      loading: true,
    }
  },
  methods: {
    // 测试从后端获取图片
    getPhoto (report_id) {
      axios
        .request({
          // 以Blob对象的数据类型返回数据
          // 需要在main.js中注释掉对mock的引用才能够正常生效
          responseType: "blob",
          url: "/API/file/photo/report/" + report_id + "/png",
          method: "get",
          headers: {
            TokenValue: this.access_token,
          }
        })
        .then((res) => {
          console.log("成功拿到举报单图片！");
          this.image_url = window.URL.createObjectURL(res.data);
          console.log(res);
          this.loading = false;
        })
        .catch((err) => {
          console.log(err);
          console.log("后端请求图片失败");
          this.get_jpg(report_id);
        });
    },
    get_jpg (id) {
      axios
        .request({
          // 以Blob对象的数据类型返回数据
          // 需要在main.js中注释掉对mock的引用才能够正常生效
          responseType: "blob",
          url: "/API/file/photo/report/" + id + "/jpg",
          method: "get",
          headers: {
            TokenValue: this.access_token,
          }
        })
        .then((res) => {
          console.log("成功拿到举报单图片！");
          this.image_url = window.URL.createObjectURL(res.data);
          console.log(res);
          this.loading = false;
        })
        .catch((err) => {
          console.log(err);
          console.log("后端请求图片失败");
          this.get_jpeg(id);
        });
    },
    get_jpeg (id) {
      axios
        .request({
          // 以Blob对象的数据类型返回数据
          // 需要在main.js中注释掉对mock的引用才能够正常生效
          responseType: "blob",
          url: "/API/file/photo/report/" + id + "/jpeg",
          method: "get",
          headers: {
            TokenValue: this.access_token,
          }
        })
        .then((res) => {
          console.log("成功拿到举报单图片！");
          this.image_url = window.URL.createObjectURL(res.data);
          console.log(res);
          this.loading = false;
        })
        .catch((err) => {
          console.log(err);
          console.log("后端请求图片失败");
          this.get_gif(id);
        });
    },
    get_gif (id) {
      axios
        .request({
          // 以Blob对象的数据类型返回数据
          // 需要在main.js中注释掉对mock的引用才能够正常生效
          responseType: "blob",
          url: "/API/file/photo/report/" + id + "/gif",
          method: "get",
          headers: {
            TokenValue: this.access_token,
          }
        })
        .then((res) => {
          console.log("成功拿到举报单图片！");
          this.image_url = window.URL.createObjectURL(res.data);
          console.log(res);
          this.loading = false;
        })
        .catch((err) => {
          console.log(err);
          console.log("后端请求图片失败");
          this.loading = false;
        });
    },
    getRole (id) {
      if (id[0] == '0' && id[1] == '0')
        return "老人";
      else if (id[0] == '1' && id[1] == '0')
        return "医生";
      else if (id[0] == '1' && id[1] == '1')
        return "护工";
      else if (id[0] == '2' && id[1] == '2')
        return "管理员";
      else
        return "未知角色";
    },
    isAfterToday (bantime)//传入的参数为string类型
    {
      if (typeof (bantime) != 'string' || bantime.length < 9)
        return false;

      //处理当前时间
      let currentDate = new Date();
      let currentYear = currentDate.getFullYear();
      let currentMonth = currentDate.getMonth() + 1;
      let currentDay = currentDate.getDay();

      //处理封禁的截止时间
      let previousYear = parseInt(bantime.substring(0, 4), 10);
      let previousMonth = parseInt(bantime.substring(5, 7), 10);
      let previousDay = parseInt(bantime.substring(8, 10), 10);

      if (currentYear > previousYear)
        return false;
      else if (currentMonth > previousMonth)
        return false;
      else if (currentDay > previousDay)
        return false;

      return true;

    },
    dateToString (currentDate)//返回类型为string
    {
      let currentYear = currentDate.getFullYear();
      let currentMonth = (currentDate.getMonth() + 1);
      let currentDay = (currentDate.getDate());

      if (currentMonth < 10)
        currentMonth = '0' + currentMonth;
      if (currentDay < 10)
        currentDay = '0' + currentDay;
      let str = currentYear + '-' + currentMonth + '-' + currentDay;
      return str;
    },
    onSubmit () {
      //审核单没填完整就直接跳出
      if (!this.FormIsFull()) {
        this.$message({
          type: 'info',
          message: '审核单未填写完整！'
        });
        return false;
      }

      //填写完整审核日期和审核状态
      this.form.ISDONE = (this.form.ISREAL == "是") ? "审核通过" : "审核不通过";
      const today = new Date();
      this.form.AUDITTIME = this.dateToString(today);
      this.toHisObj();

      //向后端发送put申请
      putReportCard(this.form.ID, this.his_obj)
        .then((res) => {
          console.log(res);
          this.$message({
            type: 'success',
            message: '审核单提交成功！审核结果：' + this.form.ISDONE
          });
        })
        .catch((err) => {
          if (err.message == "Request failed with status code 403" && this.flag) {
            this.flag = false;
            AccessTokenFailed();
          }
          console.log(err);
          this.$message({
            type: 'info',
            message: '审核单提交失败'
          });
          return false;
        });

      //put更改封号天数
      if (this.form.PUNITIVEMEASURE != '0') {
        let num = parseInt(this.form.PUNITIVEMEASURE);
        if (this.getRole(this.form.DEFENDANTID) == "医生") {
          let uid = this.form.DEFENDANTID;
          let doctorCard = {};
          //先获取医生原本信息
          getDoctorCard(uid)
            .then((res) => {
              doctorCard = res.data[0];
              console.log("获取医生信息成功！");

              //然后修改该用户的bantime（修改可覆盖，避免恶意多次举报）
              var tempDate = new Date() // 获取今天的日期
              tempDate.setDate(tempDate.getDate() + num) // 今天的前N天的日期，N自定义
              let dateString = this.dateToString(tempDate);
              doctorCard.BANTIME = dateString;

              doctorCard = this.toHisDoctorObj(doctorCard);
              console.log(doctorCard);

              //PUT修改后端数据，完成封号
              putDoctorDisable(uid, doctorCard)
                .then((res) => {
                  console.log("医生封号的PUT请求成功！");
                  console.log(res);
                })
                .catch((err) => {
                  if (err.message == "Request failed with status code 403" && this.flag) {
                    this.flag = false;
                    AccessTokenFailed();
                  }
                  console.log("医生封号失败！");
                  console.log(err);
                })
            })
            .catch((err) => {
              if (err.message == "Request failed with status code 403" && this.flag) {
                this.flag = false;
                AccessTokenFailed();
              }
              console.log("获取医生原本信息失败！");
              console.log(err);
            })

        }
        else if (this.getRole(this.form.DEFENDANTID) == "护工") {
          let uid = this.form.DEFENDANTID;
          let nurseCard = {};
          //先获取护工原本信息
          getNurseCard(uid)
            .then((res) => {
              nurseCard = res.data.message[0];

              //然后修改该用户的bantime（修改可覆盖，避免恶意多次举报）
              var tempDate = new Date() // 获取今天的日期
              tempDate.setDate(tempDate.getDate() + num) // 今天的前N天的日期，N自定义
              let dateString = this.dateToString(tempDate);
              nurseCard.bantime = dateString;

              nurseCard = this.toHisNurseObj(nurseCard);
              //PUT修改后端数据，完成封号
              putNurseDisable(uid, nurseCard)
                .then((res) => {
                  console.log("护工封号的PUT请求成功！");
                  console.log(res);
                })
                .catch((err) => {
                  if (err.message == "Request failed with status code 403" && this.flag) {
                    this.flag = false;
                    AccessTokenFailed();
                  }
                  console.log("护工封号的PUT请求失败！");
                  console.log(err);
                })
            })
            .catch((err) => {
              if (err.message == "Request failed with status code 403" && this.flag) {
                this.flag = false;
                AccessTokenFailed();
              }
              console.log(err);
            })

        }
      }

      this.$router.push({
        name: "ReportScan",
      })

      return true;
    },
    toHisNurseObj (form) {
      let his_nurse_obj = {
        pwd: form.PASSWORD,
        name: form.NAME,
        idcardno: form.IDCARDNO,
        gender: form.GENDER,
        age: form.AGE,
        phone: form.PHONE,
        description: form.DESCRIPTION,
        photo: form.PHOTO,
        userstatus: form.USERSTATUS,
        institutionid: form.INSTITUTIONID,
        bantime: form.bantime
      }
      return his_nurse_obj;
    },
    toHisDoctorObj (form) {
      let his_doctor_obj = {
        //userid: form.USERID,
        pwd: form.PASSWORD,
        name: form.NAME,
        idcardno: form.IDCARDNO,
        gender: form.GEBDER,
        age: form.AGE,
        phone: form.PHONE,
        field: form.FIELD,
        history: form.HISTORY,
        communityid: form.COMMUNITYID,
        photo: form.PHOTO,
        bantime: form.BANTIME
      }
      return his_doctor_obj;
    },
    Cancel () {
      this.dialogVisible = false;
      this.$router.push({
        name: "ReportScan",
      })
    },
    FormIsFull () {
      if (this.form.PUNITIVEMEASURE == '' || this.form.ISREAL == '')
        return false;
      return true;
    },
    JSON_to_myObject (res) {
      //res是只含一个对象的列表(JSON格式)
      //把JSON转化成对象
      let list = JSON.parse(res.data);
      return list[0];
    },
    toHisObj () {
      this.his_obj = {
        ID: this.form.ID,
        type: this.form.TYPE,
        reportTime: this.form.REPORTTIME,
        isDone: this.form.ISDONE,
        imformerID: this.form.IMFORMERID,
        imformerName: this.form.IMFORMERNAME,
        defendantID: this.form.DEFENDANTID,
        defendantName: this.form.DEFENDANTNAME,
        orderID: this.form.ORDERID,
        isReal: this.form.ISREAL,
        punitiveMeasure: this.form.PUNITIVEMEASURE,
        auditTime: this.form.AUDITTIME,
        reportText: this.form.REPORTTEXT
      }
      //console.log("aaa")
      //console.log(this.his_obj.imformerID)
    },

  },
  mounted: function () {
    //上一个页面传来的参数
    let res = this.$route.query.res;
    this.form = this.JSON_to_myObject(res);
    this.getPhoto(this.form.ID);
  }
};
</script>

// lang选择less语法，scoped限制该样式只在本文件使用，不影响其他组件
<style lang="less" scoped>
.myimage {
  width: 600px;
  height: 400px;
}
</style>
