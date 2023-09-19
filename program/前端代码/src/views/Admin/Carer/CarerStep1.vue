
<template>
  <div class="views">
    <h1>医护人员资质核验</h1>
    <el-steps :active="1">
      <el-step
        title="步骤 1"
        description="身份信息核验"
      ></el-step>
      <el-step
        title="步骤 2"
        description="资质核验"
      ></el-step>
      <el-step
        title="步骤 3"
        description="核验确认"
      ></el-step>
    </el-steps>
    <el-divider></el-divider>
    <div style="margin-left:50px">

    </div>

    <el-form
      ref="order_detail"
      :model="order_detail"
      label-width="80px"
      style="margin-left:60px"
      v-loading="loading"
    >
      <el-form-item label="用户ID">
        {{order_detail.id}}
      </el-form-item>
      <el-form-item label="姓名">
        {{order_detail.name}}
      </el-form-item>
      <el-form-item label="性别">
        {{order_detail.sex}}
      </el-form-item>
      <el-form-item label="身份">
        {{order_detail.role}}
      </el-form-item>
      <el-form-item label="年龄">
        {{order_detail.age}}
      </el-form-item>
      <el-form-item label="身份证号">
        {{order_detail.idcardno}}
      </el-form-item>
      <el-form-item label="手机号">
        {{order_detail.phone}}
      </el-form-item>

      <el-form-item align=center>
        <el-button
          type="primary"
          @click="GotoStep2()"
        >下一步</el-button>

        <el-button @click="Cancel()">取消</el-button>
      </el-form-item>
    </el-form>
  </div>
</template>

<script>
import { AccessTokenFailed } from '../../../api/data.js'
import { getNurseCard, getDoctorCard, getQualificationCard } from '../../../api/data.js'
import { postCarerResult } from '../../../api/data.js'
import Axios from "axios";
export default {
  name: "CarerStep1",
  components: {},
  data () {
    return {
      flag: true,
      order_detail: {},
      loading: true,
    }
  },
  methods: {
    GotoStep2 () {
      console.log("第一个页面的最终数据：", this.order_detail);
      //弹窗设置不可见
      this.dialogVisible = false;

      //跳转页面，下订单
      this.$router.push({
        name: "CarerStep2",
        query: { form: this.order_detail }
      })
    },
    Cancel () {
      this.dialogVisible = false;
      this.$router.push({
        name: "CarerQualify",
      })
    },
    //审核不通过提交
    onRefuse () {
      //向后端发失败信息
      postCarerResult(false)
        .then((res) => {
          this.$message({
            type: 'success',
            message: '提交成功！审核不通过！'
          });
          console.log(res);
        })
        .catch((err) => {
          if (err.message == "Request failed with status code 403" && this.flag) {
            this.flag = false;
            AccessTokenFailed();
          }
          this.$message.error('后端请求失败！');
          console.log(err);
        })
      this.Cancel();
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
  },
  mounted: function () {
    let uid = this.$route.query.id;
    let title = this.$route.query.title;

    if (this.getRole(uid) == "护工") {
      Axios.all([getNurseCard(uid), getQualificationCard(uid, title)])
        .then(Axios.spread((resNurse, resQualification) => {
          let nurseDetail = resNurse.data.message[0];

          //选择此人正确的资质
          let titleChoice;
          let qualificationList = JSON.parse(resQualification.data);
          for (titleChoice in qualificationList) {
            console.log(titleChoice)
            if (qualificationList[titleChoice].TITLE == title) {
              break;
            }
          }
          let qualificationDetail = JSON.parse(resQualification.data)[titleChoice];

          console.log(nurseDetail);
          console.log(qualificationDetail);
          this.order_detail = {
            id: nurseDetail.USERID,
            name: nurseDetail.NAME,
            role: qualificationDetail.ROLE,
            idcardno: nurseDetail.IDCARDNO,
            age: nurseDetail.AGE,
            phone: nurseDetail.PHONE,
            sex: nurseDetail.GENDER,
            institutionid: qualificationDetail.INSTITUTIONID,
            institutionname: qualificationDetail.INSTITUTIONNAME,
            title: qualificationDetail.TITLE,
            granttime: qualificationDetail.GRANTTIME,
            proof: qualificationDetail.PROOF,
          }
          console.log("rua");
          console.log(this.order_detail);
          this.loading = false;
        }))
        .catch((err) => {
          if (err.message == "Request failed with status code 403" && this.flag) {
            this.flag = false;
            AccessTokenFailed();
          }
          console.log(err);
        })
    }
    else if (this.getRole(uid) == "医生") {
      Axios.all([getDoctorCard(uid), getQualificationCard(uid)])
        .then(Axios.spread((resNurse, resQualification) => {
          let nurseDetail = resNurse.data[0];
          let qualificationDetail = JSON.parse(resQualification.data)[0];
          console.log("kua");
          console.log(nurseDetail);
          console.log(qualificationDetail);
          this.order_detail = {
            id: nurseDetail.USERID,
            name: nurseDetail.NAME,
            role: qualificationDetail.ROLE,
            idcardno: nurseDetail.IDCARDNO,
            age: nurseDetail.AGE,
            phone: nurseDetail.PHONE,
            sex: nurseDetail.GENDER,
            institutionid: qualificationDetail.INSTITUTIONID,
            institutionname: qualificationDetail.INSTITUTIONNAME,
            title: qualificationDetail.TITLE,
            granttime: qualificationDetail.GRANTTIME,
            proof: qualificationDetail.PROOF,
          }
          console.log("rua");
          console.log(this.order_detail);
          this.loading = false;

        }))
        .catch((err) => {
          if (err.message == "Request failed with status code 403" && this.flag) {
            this.flag = false;
            AccessTokenFailed();
          }
          console.log(err);
        })
    }
    // getCarerCard()
    // .then((res)=>{
    //   this.order_detail=res.data;
    //   this.sex=res.data.sex;
    //   console.log("订单的详情",this.order_detail);
    // })
    // .catch((err)=>{
    //   if (err.message == "Request failed with status code 403" || err.message == "Request failed with status code 500") {
    //AccessTokenFailed();
    // }
    //   console.log(err)
    //   });
  }
};
</script>

// lang选择less语法，scoped限制该样式只在本文件使用，不影响其他组件
<style lang="less" scoped>
</style>>
