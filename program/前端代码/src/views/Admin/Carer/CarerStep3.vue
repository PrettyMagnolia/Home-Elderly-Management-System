
<template>
  <div class="views">
    <h1>医护人员资质核验</h1>
    <el-steps :active="3">
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

    <el-form
      ref="form"
      :model="form"
      label-width="150px"
      style="margin-left:60px"
      v-loading="loading"
    >
      <el-form-item label="用户ID">
        {{form.id}}
      </el-form-item>

      <el-form-item label="姓名">
        {{form.name}}
      </el-form-item>

      <el-form-item label="资质名称">
        {{form.title}}
      </el-form-item>

      <el-form-item label="资质说明">
        {{form.proof}}
      </el-form-item>

      <el-form-item label="管理员审核时间">
        {{today}}
      </el-form-item>

      <el-form-item align=center>
        <el-button
          type="primary"
          @click="onSubmit()"
        >审核通过</el-button>
        <el-button
          type="danger"
          @click="onRefuse()"
        >审核不通过</el-button>
        <el-button @click="Cancel()">取消</el-button>
      </el-form-item>
    </el-form>

    <!--<el-button @click="deleteItem()">删除资质记录</el-button>-->

  </div>
</template>

<script>
import { AccessTokenFailed } from '../../../api/data.js'
import { putQualification, getNurseCard, putNurseDisable } from '../../../api/data.js'
export default {
  name: "CarerStep3",
  components: {},
  data () {
    return {
      flag: true,
      form: {},
      today: this.getCurrentDate(),
      admin_id: '',
      loading: true,
    };
  },
  methods: {
    Cancel () {
      this.dialogVisible = false;
      this.$router.push({
        name: "CarerQualify",
      })
    },
    //获取当前日期
    getCurrentDate () {
      var nowDate = new Date();
      var date = {
        year: nowDate.getFullYear(),
        month: nowDate.getMonth() + 1,
        day: nowDate.getDate(),
      }
      var dayDate = date.year + '-' + (date.month >= 10 ? date.month : '0' + date.month) + '-' + (date.day >= 10 ? date.day : '0' + date.day)
      return dayDate;
    },
    //获取明年日期
    getNextYearDate () {
      var nowDate = new Date();
      var date = {
        year: nowDate.getFullYear() + 1,
        month: nowDate.getMonth() + 1,
        day: nowDate.getDate(),
      }
      var dayDate = date.year + '-' + (date.month >= 10 ? date.month : '0' + date.month) + '-' + (date.day >= 10 ? date.day : '0' + date.day)
      return dayDate;
    },
    //审核通过提交
    onSubmit () {
      let his_obj = {
        workerid: this.form.id,
        role: this.form.role,
        title: this.form.title,
        proof: this.form.proof,
        granttime: this.form.granttime,
        adminitorid: this.admin_id,
        isdone: '审核通过',
        audittime: this.today,
        institutionid: this.form.institutionid,
        institutionname: this.form.institutionname
      }

      //向后端发成功信息
      putQualification(his_obj)
        .then((res) => {
          this.$message({
            type: 'success',
            message: '提交成功！审核通过！'
          });
          console.log(res);
          //put护工！！！
          this.ModifyNurse(his_obj.workerid, his_obj)
        })
        .catch((err) => {
          if (err.message == "Request failed with status code 403" && this.flag) {
            this.flag = false;
            AccessTokenFailed();
          }
          this.$message.error('后端请求失败！');
          console.log(err);
          this.Cancel();
        })

    },
    toHisNurseObj (form) {
      let his_nurse_obj = {
        //userid: form.USERID,
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
    ModifyNurse (uid, nurseCard) {
      getNurseCard(uid)
        .then((res) => {
          nurseCard = res.data.message[0];

          nurseCard.INSTITUTIONID = this.form.institutionid;

          nurseCard = this.toHisNurseObj(nurseCard);
          //PUT修改后端数据，完成封号
          putNurseDisable(uid, nurseCard)
            .then((res) => {
              console.log("护工机构ID添加成功！");
              console.log(res);
              this.Cancel();
            })
            .catch((err) => {
              if (err.message == "Request failed with status code 403" && this.flag) {
                this.flag = false;
                AccessTokenFailed();
              }
              console.log("护工机构ID添加失败！");
              console.log(err);
              this.Cancel();
            })
        })
        .catch((err) => {
          if (err.message == "Request failed with status code 403" && this.flag) {
            this.flag = false;
            AccessTokenFailed();
          }
          console.log(err);
          this.Cancel();
        })



    },
    deleteItem () {
      // deleteQualification("112022082900000005","养老护理员四级")
      // .then((res)=>{
      //   console.log(res);
      //   console.log("yes")
      // })
      // .catch((err)=>{
      //   console.log(err);
      //   console.log('no');
      // })

      let his_obj = {
        workerid: "112022082900000004",
        role: '护工',
        title: "养老护理员四级",
        //proof:'通过养老护理员四级考试并获技能鉴定证书' ,
        granttime: '2021-08-22',
        adminitorid: '',
        isdone: '待审核',
        audittime: '',
        institutionid: '',
        institutionname: '',
      }
      putQualification(his_obj)
        .then((res) => {
          this.$message({
            type: 'success',
            message: '提交成功！审核通过！'
          });
          console.log(res);
          //put护工！！！
          //this.ModifyNurse(his_obj.workerid,his_obj)
        })
        .catch((err) => {
          if (err.message == "Request failed with status code 403" && this.flag) {
            this.flag = false;
            AccessTokenFailed();
          }
          this.$message.error('后端请求失败！');
          console.log(err);
          //his.Cancel();
        })
    },
    //审核不通过提交
    onRefuse () {
      let his_obj = {
        workerid: this.form.id,
        role: this.form.role,
        title: this.form.title,
        proof: this.form.proof,
        granttime: this.form.granttime,
        adminitorid: this.admin_id,
        isdone: '审核不通过',
        audittime: this.today,
        institutionid: this.form.institutionid,
        institutionname: this.form.institutionname
      }

      //向后端发不通过信息
      putQualification(his_obj)
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
    }
  },

  mounted: function () {
    this.form = this.$route.query.form;
    this.admin_id = this.$cookies.get("USERID");
    this.loading = false;
    //获取当前和明年的日期
    this.current_date = this.getCurrentDate();
    this.next_date = this.getNextYearDate();
  }

};
</script>

// lang选择less语法，scoped限制该样式只在本文件使用，不影响其他组件
<style lang="less" scoped>
</style>>
