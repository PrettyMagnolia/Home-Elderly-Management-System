<template>
  <!--注册表单-->
  <div class="register_box">
    <el-form
      ref="registerFormRef"
      :model="registerForm"
      :rules="registerFormRules"
      class="register_form"
      label-position="left"
      v-show="showForm"
    >
      <h3 class="login_title">{{ role_text }}注册</h3>
      <!-- 多级选择，社区 -->
      <el-form-item v-if="role_text == '医生'" class="form_item">
        <el-cascader
          placeholder="社区名字/可输入"
          :options="society_options"
          filterable
          class="cascader"
          :show-all-levels="false"
          v-model="chosen_society"
        ></el-cascader>
      </el-form-item>

      <el-form-item prop="number" v-if="role_text != '用户'">
        <el-input
          v-model="registerForm.number"
          placeholder="身份证号"
        ></el-input>
      </el-form-item>
      <el-form-item prop="username">
        <el-input
          v-model="registerForm.username"
          placeholder="用户名"
        ></el-input>
      </el-form-item>
      <el-form-item prop="phoneNumber">
        <el-input
          v-model="registerForm.phoneNumber"
          placeholder="电话号码"
        ></el-input>
      </el-form-item>
      <el-form-item prop="password">
        <el-input
          type="password"
          v-model="registerForm.password"
          placeholder="密码"
        ></el-input>
      </el-form-item>
      <el-form-item prop="confirmPassword">
        <el-input
          type="password"
          v-model="registerForm.confirmPassword"
          placeholder="确认密码"
        ></el-input>
      </el-form-item>

      <!--    注册按钮-->
      <el-form-item class="button_box">
        <el-button
          type="primary"
          class="button_item"
          v-on:click="register('registerFormRef')"
          >注册</el-button
        >
        <el-button class="button_item" v-on:click="toLogin"
          >已有账号，登录</el-button
        >
      </el-form-item>
    </el-form>

    <!-- 展示注册成功的表单 -->
    <div class="show_box" v-show="!showForm">
      <div class="header_box">
        <i class="el-icon-success"></i>
        <h4>注册成功！</h4>
        <br />
      </div>
      <h4>请前往个人中心完善资料(ノﾟ∀ﾟ)ノ</h4>
      <div class="show_item">
        <h3 class="label">账号:</h3>
        <h4 class="value">{{ registerForm.phoneNumber }}</h4>
      </div>
      <div class="show_item">
        <h3 class="label">密码:</h3>
        <h4 class="value">{{ registerForm.password }}</h4>
      </div>

      <!-- 按钮 -->
      <div class="button_box">
        <el-button class="button_item" type="primary" @click="SuccesstoLogin"
          >前往登录</el-button
        >
      </div>
    </div>
  </div>
</template>

<script>
import axios from "axios";
export default {
  name: "Register",
  data() {
    //电话号码校验
    var validatePhone = (rule, value, callback) => {
      if (!value) {
        return callback(new Error("请输入电话号码"));
      } else {
        const reg = /^1[3|4|5|7|8][0-9]\d{8}$/;
        const isPhone = reg.test(value);

        value = Number(value); //转换为数字
        if (typeof value === "number" && !isNaN(value)) {
          //判断是否为数字
          value = value.toString(); //转换成字符串
          if (value.length < 0 || value.length > 12 || !isPhone) {
            //判断是否为11位手机号
            callback(new Error("手机号码格式如:138xxxx8754"));
          } else {
            callback();
          }
        } else {
          callback(new Error("请勿输入字符  "));
        }
      }
    };
    //密码校验
    var checkPassword = (rule, value, cb) => {
      const regPassword = /^\w{6,50}$/;
      if (regPassword.test(value)) {
        //合法密码
        return cb();
      }
      cb(new Error("长度必须为6-50个字符,只能由大小写字母数字下划线组成"));
    };
    //二次密码校验
    var checkConfirmPassword = (rule, value, cb) => {
      const regPassword = this.registerForm.password;
      if (regPassword === value) {
        //合法密码
        return cb();
      }
      cb(new Error("前后两次输入的密码必须一致!"));
    };
    //身份证校验
    var checkNumber = (rule, value, cb) => {
      const regNumber = /^\d{18}$/;
      if (regNumber.test(value)) {
        //合法密码
        return cb();
      }
      cb(new Error("身份证号必须是18位数字!"));
    };

    return {
      access_token: null,
      headers: {},
      //选择的社区数据
      chosen_society: [],
      //社区，多级选择
      society_options: [],
      role_text: "", //注册的身份,用户、护工、医生
      //注册表单数据绑定
      registerForm: {
        number: "", //身份证
        username: "",
        phoneNumber: "",
        password: "",
        confirmPassword: "",
      },
      //是否展示注册表单
      showForm: true,
      //表单的验证规则
      registerFormRules: {
        //    验证身份证号是否合法
        number: [
          { required: true, message: "请输入身份证号", trigger: "blur" },
          { validator: checkNumber, trigger: "blur" },
        ],
        //    验证姓名是否合法
        username: [{ required: true, message: "请输入姓名", trigger: "blur" }],
        //    验证电话号码是否合法
        phoneNumber: [
          { required: true, message: "请输入电话号码", trigger: "blur" },
          {
            validator: validatePhone,
            trigger: "blur",
          },
        ],
        //    验证密码是否合法
        password: [
          { required: true, message: "请输入密码", trigger: "blur" },
          { validator: checkPassword, trigger: "blur" },
        ],
        confirmPassword: [
          { required: true, message: "请再次确认密码", trigger: "blur" },
          { validator: checkConfirmPassword, trigger: "blur" },
        ],
      },
    };
  },
  //回车注册操作
  created() {
    //创建后挂载
    let _this = this;

    document.onkeydown = function (e) {
      let key = window.event.keyCode;

      if (key === 13) {
        _this.register(); //注册
      }
    };
  },
  methods: {
    //注册
    register(refName) {
      //触发该表单检测
      this.$refs[refName].validate(async (valid) => {
        //表单检测通过
        if (valid) {
          console.log("registerForm", this.registerForm);
          //注册身份不同
          if (this.role_text == "用户") {
            //向后端请求注册
            //请求后端注册,post 新用户
            let post_data = {
              pwd: this.registerForm.password,
              name: this.registerForm.username,
              phone: this.registerForm.phoneNumber,
            };
            axios
              .request({
                url: "/API/Elder",
                method: "post",
                data: post_data,
                headers: this.headers,
              })
              .then((res) => {
                console.log("注册client", res);

                if (res.data.message === "账号已被注册") {
                  //手机号码已经注册过
                  this.$alert(
                    "(ﾟ∀。)该手机号已经注册过" + "\n" + "请更换手机号",
                    "注册失败",
                    {
                      confirmButtonText: "确定",
                    }
                  );
                } else {
                  //成功展示页面

                  this.showForm = false;
                }
              })
              .catch((err) => {
                console.log("注册client", err);
                this.$message({
                  type: "error",
                  message: "注册错误(ﾟдﾟ)\n请尝试重新注册",
                });
              });
            // //手机号码已经注册过
            // this.$alert(
            //   "(ﾟ∀。)该手机号已经注册过" + "\n" + "请更换手机号",
            //   "注册失败",
            //   {
            //     confirmButtonText: "确定",
            //   }
            // );
            // this.$message({
            //   type: "error",
            //   message: "注册错误(ﾟдﾟ)\n请尝试重新注册",
            // });
          } else if (this.role_text == "护工") {
            //请求后端注册,post 新护工
            let post_data = {
              pwd: this.registerForm.password,
              name: this.registerForm.username,
              phone: this.registerForm.phoneNumber,
              idcardno: this.registerForm.number,
            };
            await axios
              .request({
                url: "/API/NursingWorker",
                method: "post",
                data: post_data,
                headers: this.headers,
              })
              .then((res) => {
                console.log("post 护工", res);
                if (res.data.message === "账号已被注册") {
                  //手机号码已经注册过
                  this.$alert(
                    "(ﾟ∀。)该手机号已经注册过" + "\n" + "请更换手机号",
                    "注册失败",
                    {
                      confirmButtonText: "确定",
                    }
                  );
                } else {
                  //成功展示页面
                  this.showForm = false;
                }
              })
              .catch((err) => {
                console.log(err);
                this.$message({
                  type: "error",
                  message: "注册错误(ﾟдﾟ)\n请尝试重新注册",
                });
              });
          } else if (this.role_text == "医生") {
            //请求后端，检查这个手机号是否注册

            axios
              .request({
                url: "/API/Doctor/phone",
                method: "get",
                params: {
                  phonenumber: this.registerForm.phoneNumber,
                },
                headers: this.headers,
              })
              .then((res) => {
                console.log("手机号", this.registerForm.phoneNumber, res.data);

                if (res.data == "unexist") {
                  //请求后端注册,post 新医生
                  let post_data = {
                    pwd: this.registerForm.password,
                    name: this.registerForm.username,
                    phone: this.registerForm.phoneNumber,
                    idcardno: this.registerForm.number,
                    communityid: this.chosen_society[2],
                  };
                  axios
                    .request({
                      url: "/API/Doctor",
                      method: "post",
                      data: post_data,
                      headers: this.headers,
                    })
                    .then((res) => {
                      console.log("post 医生", res);
                      //成功展示页面
                      this.showForm = false;
                    })
                    .catch((err) => {
                      console.log(err);
                      this.$message({
                        type: "error",
                        message: "注册错误(ﾟдﾟ)\n请尝试重新注册",
                      });
                    });
                }
              })
              .catch((err) => {
                console.log(err);
                this.$message({
                  type: "error",
                  message: "注册错误(ﾟдﾟ)\n请尝试重新注册",
                });
              });
          }
        } else {
          //表单检测失败
          this.$message({
            message: "注册格式有误",
            type: "error",
          });

          return false;
        }
      });
      //请求后端注册
      //返回
    },

    //空白地,返回login界面
    toLogin() {
      this.$router.push({ path: "/loginFrame/login" });
    },
    //注册成功，返回login界面自动登录
    SuccesstoLogin() {
      let params = {
        password: this.registerForm.password,
        role_text: this.role_text,
        phone: this.registerForm.phoneNumber,
      };
      this.$router.push({ name: "Login", params: params });
    },
  },
  mounted: async function () {
    this.access_token = this.$cookies.get("token");
    this.headers = {
      TokenValue: this.access_token,
    };

    // this.$store.commit("setData", { userName: 132 });
    // console.log("vuex中用户信息:", this.$store.state.user);

    //测试cookies
    // this.$cookies.set("testKeyName", "keyValue", "1d");
    // let all_cookies = this.$cookies.keys();
    // console.log("全部的cookies:", all_cookies);

    //要注册的身份是
    this.role_text = this.$route.params.role;
    console.log("role_text", this.role_text);

    //若医生，请求社区数据
    var society_data = {};
    if (this.role_text == "医生") {
      await axios
        .request({
          url: "/API/community",
          method: "get",
          headers: this.headers,
        })
        .then((res) => {
          console.log("/API/community", res);
          //处理数据
          res.data.forEach((ele) => {
            if (ele.CITY in society_data) {
            } else {
              society_data[ele.CITY] = {};
            }
            if (ele.BLOCK in society_data[ele.CITY]) {
              society_data[ele.CITY][ele.BLOCK].push({
                name: ele.NAME,
                id: ele.COMMUNITYID,
              });
            } else {
              society_data[ele.CITY][ele.BLOCK] = [];
            }
          });
          console.log("society_data", society_data);
        })
        .catch((err) => {
          console.log(err);
        });

      //组装选择器
      let i = 0,
        j = 0;
      for (var city_name in society_data) {
        this.society_options.push({
          value: city_name,
          label: city_name,
          children: [],
        });
        j = 0;
        for (var block_name in society_data[city_name]) {
          this.society_options[i].children.push({
            value: block_name,
            label: block_name,
            children: [],
          });
          society_data[city_name][block_name].forEach((ele) => {
            try {
              this.society_options[i].children[j].children.push({
                value: ele.id,
                label: ele.name,
              });
            } catch (error) {
              console.log(i, ".", j, ";", city_name, block_name, ele.name);
            }
          });
          j++;
        }
        i++;
      }
      console.log("society_options", this.society_options);
    }
  },
  destroyed() {
    //取消键盘监听事件
    document.onkeydown = null;
  },
};
</script>
<style lang="less" scoped>
.register_box {
  width: 100%;
  .register_form {
    height: 100%;
    width: 100%;
    .login_title {
      display: flex;
      justify-content: center;
      align-items: center;
      font-family: "Microsoft YaHei";
      font-weight: bold;
      font-size: 20px;
    }
    .form_item {
      .cascader {
        width: 100%;
      }
    }
    .button_box {
      display: flex;
      justify-content: center;
      align-items: center;
      .button_item {
        margin-left: 10px;
        margin-right: 10px;
        padding-left: 25px;
        padding-right: 25px;
      }
    }
  }

  .show_box {
    .header_box {
      display: flex;
      justify-content: center;
      align-items: center;

      i {
        color: #8ecd5d;
        font-size: 45px;
        margin-right: 20px;
      }
      h4 {
        font-size: 25px;
      }
    }
    .show_item {
      display: flex;
      justify-content: left;
      align-items: center;
      // border: solid 2px #dcdfe6;
      border-radius: 10px;
      padding-left: 15px;
      padding-right: 15px;
      margin-bottom: 20px;
      .label {
      }
      .value {
      }
    }
    .button_box {
      display: flex;
      justify-content: center;
      align-items: center;
    }
  }
}
</style>
