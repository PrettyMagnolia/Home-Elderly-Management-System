<template>
  <div class="userInfor_page">
    <el-tabs
      v-model="activeName"
      class="el_tabs"
    >
      <el-tab-pane
        label="个人信息"
        name="first"
        class="infor_tab"
      >
        <el-input
          v-model="form.name"
          :disabled="true"
          style="width: 20%; margin-bottom: 20px; margin-right: 5%"
          class="show_input"
        >
          <template slot="prepend">名字</template>
        </el-input>
        <el-input
          v-model="form.idCardNo"
          :disabled="true"
          style="width: 50%; margin-bottom: 20px; margin-right: 10%"
          class="show_input"
        >
          <template slot="prepend">身份证号</template>
        </el-input>
        <el-input
          v-model="form.gender"
          :disabled="true"
          style="width: 20%; margin-bottom: 20px; margin-right: 5%"
          class="show_input"
        >
          <template slot="prepend">性别</template>
        </el-input>
        <el-input
          v-model="form.phone"
          :disabled="true"
          style="
            width: 50%;
            margin-bottom: 20px;
            margin-right: 10%;
            label-width: 1000px;
          "
          class="show_input"
        >
          <template slot="prepend">电话</template>
        </el-input>
        <el-input
          v-model="form.age"
          :disabled="true"
          style="width: 20%; margin-bottom: 20px; margin-right: 5%"
          class="show_input"
        >
          <template slot="prepend">年龄</template>
        </el-input>

        <el-input
          v-model="form.position"
          :disabled="true"
          style="
            width: 30%;
            margin-bottom: 20px;
            margin-right: 10%;
            label-width: 1000px;
          "
          class="show_input"
        >
          <template slot="prepend">职位</template>
        </el-input>

        <br />
        <el-button
          type="primary"
          v-on:click="toModifyInfor"
        >修改个人信息</el-button>
        <el-button v-on:click="toModifyPassword">修改密码</el-button>
      </el-tab-pane>
    </el-tabs>
  </div>
</template>

<script>
import { AccessTokenFailed } from '../../../api/data.js'
import axios from "../../../api/axios";
export default {
  name: "AdminUserInfor",
  components: {},
  data () {
    return {
      flag: true,
      access_token: this.$cookies.get("token"),
      activeName: "first",

      my_uid: "",

      loading: true,
      //个人信息绑定的数据表单
      form: {},

      rules: {},

      //头像显示需要的
      // 从后端获取的图片url
      image_url: null,
      // 是否展示获取的图片
      image_show: true,
    };
  },
  methods: {
    toModifyInfor () {
      this.$router.push({ name: "AdminModifyInfo" });
    },
    toModifyPassword () {
      this.$router.push({ name: "AdminModifyPassword" });
    },
  },
  mounted: async function () {
    this.my_uid = this.$cookies.get("USERID");
    // 根据uid获取当前管理员信息
    await axios
      .request({
        url: "/API/Manager/" + this.my_uid,
        method: "get",
        headers: {
          TokenValue: this.access_token,
        },
      })
      .then(async (res) => {
        let raw_data = JSON.parse(res.data)[0];
        // console.log("管理员信息", raw_data);

        //拼凑put表单
        this.form = {
          ID: raw_data.ID,
          name: raw_data.NAME,
          pwd: raw_data.PWD,
          idCardNo: raw_data.IDCARDNO,
          gender: raw_data.GENDER,
          age: raw_data.AGE,
          phone: raw_data.PHONE,
          position: raw_data.POSITION,
        };
        // console.log("this.form", this.form);
      })
      .catch((err) => {
        if (err.message == "Request failed with status code 403" && this.flag) {
          this.flag = false;
          AccessTokenFailed();
        }
        console.log(err);
      });
  },
};
</script>

// lang选择less语法，scoped限制该样式只在本文件使用，不影响其他组件
<style lang="less" scoped>
.userInfor_page {
  .el_tabs {
    .infor_tab {
      .show_input /deep/ .el-textarea__inner:disabled {
        color: black;
        font-weight: bold;
      }
      .show_input /deep/ .el-input__inner:disabled {
        color: black;
        font-weight: bold;
      }
    }
    .attention_tab {
    }
  }
}
</style>>