<template>
  <div class="login_box">
    <el-form
      ref="loginFormRef"
      :model="loginForm"
      :rules="loginFormRules"
      class="login_form"
      label-position="left"
      label-width="80px"
    >
      <h3 class="login_title">登录</h3>
      <el-form-item prop="username" class="input_box" label="账号">
        <el-tooltip
          class="item"
          effect="dark"
          content="账号即为注册时的手机号码"
          placement="right"
        >
          <el-input
            type="text"
            v-model="loginForm.username"
            auto-complete="off"
            placeholder="请输入账号"
            @blur="InputBlurTrim('username')"
          ></el-input>
        </el-tooltip>
      </el-form-item>

      <el-form-item prop="password" class="input_box" label="密码">
        <el-input
          type="password"
          v-model="loginForm.password"
          auto-complete="off"
          placeholder="请输入密码"
          @blur="InputBlurTrim('password')"
        ></el-input>
      </el-form-item>
      <!-- 身份单选框 -->
      <el-radio-group v-model="role_checkBox" size="small" class="radio_group">
        <el-radio-button
          class="radio_item"
          v-for="(role_text, role_index) in roles_text"
          :label="role_text"
          :key="role_index"
          @change.native.prevent="RoleChange"
          >{{ role_text }}</el-radio-button
        >
      </el-radio-group>

      <el-form-item class="up_button_box">
        <el-button type="primary" v-on:click="login" class="button_item"
          >登录
        </el-button>
        <el-button v-on:click="toRegister" class="button_item">注册 </el-button>
      </el-form-item>
      <el-form-item class="low_button_box">
        <el-radio
          v-model="isStayLogin"
          class="stayLogin_radio"
          :label="true"
          @click.native.prevent="onRadioChange(true)"
          >3天内自动登录</el-radio
        >
        <!-- 找回密码 -->
        <!-- <el-button type="text" class="retrieve_password">找回密码 </el-button> -->
        <el-button
          type="text"
          class="apply_institution"
          @click="ApplyInsButtonClick"
          >申请护理机构
        </el-button>
      </el-form-item>
    </el-form>
  </div>
</template>

<script>
import axios from "axios";
export default {
  name: "Login",
  data() {
    return {
      access_token: null,
      headers: {},

      //登录表单数据绑定
      loginForm: {
        username: "",
        password: "",
      },
      //表单的验证规则
      loginFormRules: {
        //验证用户ID是否合法
        username: [
          { required: true, message: "请输入账号", trigger: "blur" },
          {
            min: 11,
            max: 11,
            message: "用户账号必须是11位",
            trigger: "blur",
          },
        ],
        //    验证密码是否合法
        password: [
          { required: true, message: "请输入密码", trigger: "blur" },
          {
            min: 1,
            max: 50,
            message: "密码必须在1-50个字符之间",
            trigger: "blur",
          },
        ],
        userToken: "",
      },
      //是否保持登录
      isStayLogin: false,
      //身份多选框文字
      roles_text: ["用户", "护工", "医生", "管理员"],
      role_checkBox: "用户",
      role_num: "0",
    };
  },
  //回车登录操作
  created() {
    //创建后挂载
    let _this = this;

    document.onkeydown = function () {
      let key = window.event.keyCode;

      if (key === 13) {
        _this.login(); //登录的方法
      }
    };
  },
  methods: {
    //input blur trim
    InputBlurTrim(name) {
      this.loginForm[name] = this.loginForm[name].trim();
    },
    //进入 申请护理机构 页面
    ApplyInsButtonClick() {
      //hym更新过该函数
      //登录"黄彦铭"管理员账号，保证机构提交时有token
      //后端登录，获取双token
      let params = {
        uname: "18950393523",
        upassword: "1",
        rule: "3",
      };

      console.log("账号密码", params);
      this.$http
        .get("/API/user", { params: params })
        .then((result) => {
          console.log("/API/user", result);
          //用户ID
          var userID;

          userID = JSON.parse(result.data.user)[0].ID;

          //本次会话token
          let TokenValue = result.data.access_token;
          //refresh token
          let refresh_token = result.data.refresh_toekn;
          //身份
          let role = this.role_num;
          //ban time
          let ban_time = JSON.parse(result.data.user)[0].BANTIME;
          let headers = {
            TokenValue: TokenValue,
          };
          console.log("TokenValue", TokenValue);
          console.log("refresh_token", refresh_token);

          //临时存cookies
          this.$cookies.set("token", TokenValue, "session");
          this.$cookies.set("role", role, "session");
          this.$cookies.set("USERID", userID, "session");

          //需要保持登录
          if (this.isStayLogin) {
            //存role、fresh token
            window.localStorage.setItem("refresh_token", refresh_token);
            window.localStorage.setItem("role", role);
          }

          console.log("成功登录临时管理员账号！");
          this.$router.push({ name: "ApplyIns" });
        })
        .catch((err) => {
          console.log(err);
        });
    },
    // 根据角色，跳转页面
    enterHome(role) {
      //根据身份，进入不同的用户主页
      switch (role) {
        //client
        case "0":
          this.$router.push({ name: "Home" });
          break;
        case "1":
          this.$router.push({ name: "NurseHome" });
          break;
        case "2":
          this.$router.push({ name: "DoctorHome" });
          break;
        case "3":
          this.$router.push({ name: "Console" });
          break;
      }
    },
    //登录
    login() {
      //登录系统暂时故障，这里直接进入，无token等信息
      //根据身份，进入不同的用户主页
      // let role = this.role_num;
      // switch (role) {
      //   //client
      //   case "0":
      //     this.$router.push({ name: "Home" });
      //     break;
      //   case "1":
      //     this.$router.push({ name: "NurseHome" });
      //     break;
      //   case "2":
      //     this.$router.push({ name: "DoctorHome" });
      //     break;
      //   case "3":
      //     this.$router.push({ name: "Console" });
      //     break;
      // }
      // return;

      //输入账号密码非空
      if (this.loginForm.username === "" || this.loginForm.password === "") {
        this.$message({
          message: "用户ID或密码不能为空",
          type: "error",
        });
      } else {
        //分身份
        if (this.role_checkBox) {
          //后端登录，获取双token
          let params = {
            uname: this.loginForm.username,
            upassword: this.loginForm.password,
            rule: this.role_num,
          };

          console.log("账号密码", params);
          this.$http
            .get("/API/user", { params: params })
            .then((result) => {
              console.log("/API/user", result);
              //用户ID
              var userID;
              if (this.role_checkBox == "管理员") {
                userID = JSON.parse(result.data.user)[0].ID;
              } else {
                userID = JSON.parse(result.data.user)[0].USERID;
              }
              //本次会话token
              let TokenValue = result.data.access_token;
              //refresh token
              let refresh_token = result.data.refresh_toekn;
              //身份
              let role = this.role_num;
              //ban time
              let ban_time = JSON.parse(result.data.user)[0].BANTIME;
              let headers = {
                TokenValue: TokenValue,
              };
              console.log("TokenValue", TokenValue);
              console.log("refresh_token", refresh_token);

              // //测试refresh token
              // axios
              //   .request({
              //     url: "/API/user/refresh",
              //     method: "get",
              //     params: {
              //       userrefreshtoken: refresh_token,
              //     },
              //   })
              //   .then((res) => {
              //     console.log("/API/user/refresh", res);
              //   })
              //   .catch((err) => {
              //     console.log(err);
              //   });
              //临时存cookies
              this.$cookies.set("token", TokenValue, "session");
              this.$cookies.set("role", role, "session");
              this.$cookies.set("USERID", userID, "session");

              //需要保持登录
              if (this.isStayLogin) {
                //存role、fresh token
                window.localStorage.setItem("refresh_token", refresh_token);
                window.localStorage.setItem("role", role);
              }

              this.$message({
                message: "登录成功",
                type: "success",
              });
              this.enterHome(role);
              //根据身份，进入不同的用户主页
            })
            .catch((err) => {
              console.log(err);
              this.$message({
                message: "登录失败！\n请检查账号密码是否正确",
                type: "warning",
              });
            });
        }
      }
    },

    //跳转注册
    toRegister() {
      if (
        this.role_num == "0" ||
        this.role_num == "1" ||
        this.role_num == "2"
      ) {
        this.$router.push({
          name: "Register",
          params: {
            role: this.role_checkBox,
          },
        });
      } else if (this.role_num == "3") {
        this.$message({
          type: "warning",
          message: "这里还能注册管理员的吗Σ(ﾟдﾟ)",
        });
      }
    },
    //单选框改写
    onRadioChange(e) {
      this.isStayLogin = e === this.isStayLogin ? false : e;
    },
    //身份单选
    RoleChange() {
      console.log("this.role_checkBox", this.role_checkBox);
      switch (this.role_checkBox) {
        case "用户":
          this.role_num = "0";
          break;
        case "护工":
          this.role_num = "1";
          break;
        case "医生":
          this.role_num = "2";
          break;
        case "管理员":
          this.role_num = "3";
          break;
      }
    },
  },
  mounted: function () {
    this.access_token = this.$cookies.get("token");
    this.headers = {
      TokenValue: this.access_token,
    };
    let router_params = this.$route.params;
    console.log("router_params", router_params);
    if (router_params.password != null) {
      this.loginForm.username = router_params.phone;
      this.loginForm.password = router_params.password;
      this.role_checkBox = router_params.role_text;
      switch (this.role_checkBox) {
        case "用户":
          this.role_num = "0";
          break;
        case "护工":
          this.role_num = "1";
          break;
        case "医生":
          this.role_num = "2";
          break;
      }
      console.log("设置表单");
    }
  },
  destroyed() {
    //取消键盘监听事件
    document.onkeydown = null;
  },
};
</script>
<style lang="less" scoped>
.login_box {
  width: 100%;
  height: 100%;
  margin-bottom: 30px;
  margin-top: 0px;
  .login_form {
    height: 100%;
    width: 100%;
    display: flex;
    flex-direction: column;

    .login_title {
      display: flex;
      justify-content: center;
      align-items: center;
      font-family: "Microsoft YaHei";
      font-weight: bold;
      font-size: 25px;
    }
    .input_box {
    }
    //身份单选
    .radio_group {
      display: flex;
      flex-direction: row;
      align-items: center;
      justify-content: center;
      margin-bottom: 20px;
      .radio_item {
      }
    }
    .up_button_box {
      // background-color: black;
      .button_item {
      }
    }

    .low_button_box {
      .stayLogin_radio {
      }
      .retrieve_password {
      }
    }
  }
}
</style>
