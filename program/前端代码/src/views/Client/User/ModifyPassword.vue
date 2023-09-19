<template>
  <div class="modify_password_view">
    <!-- <div class="input_box">
      <h2 class="head_text">修改密码</h2>
      <el-input
        v-model="form.password"
        placeholder="请输入原密码"
        class="input_pwd"
      >
        <template slot="prepend">原密码</template>
      </el-input>
      <el-input
        v-model="form.newpassword"
        placeholder="请输入新密码"
        class="input_pwd"
      >
        <template slot="prepend">新密码</template>
      </el-input>
      <el-input
        v-model="form.confirmpassword"
        placeholder="请再次输入新密码"
        class="input_pwd"
      >
        <template slot="prepend">确认密码</template>
      </el-input>
    </div> -->

    <el-form
      :model="form"
      status-icon
      :rules="rules"
      ref="form"
      label-width="100px"
      class="input_box"
    >
      <el-form-item label="原密码" prop="password">
        <el-input
          type="password"
          v-model="form.password"
          placeholder="请输入原密码"
        ></el-input>
      </el-form-item>
      <el-form-item label="新密码" prop="newpassword">
        <el-input
          type="password"
          v-model="form.newpassword"
          placeholder="请输入新密码"
        ></el-input>
      </el-form-item>
      <el-form-item label="确认密码" prop="confirmpassword">
        <el-input
          type="password"
          v-model="form.confirmpassword"
          placeholder="请确认新密码"
        ></el-input>
      </el-form-item>
    </el-form>

    <!-- 按钮 -->
    <div class="button_box">
      <el-button
        type="primary"
        class="confirm_button"
        @click="ConfirmButtonClick"
        >确认修改</el-button
      >
      <el-button class="exit_button" @click="ExitButtonClick">返回</el-button>
    </div>
  </div>
</template>

<script>
import { AccessTokenFailed } from "../../../api/data.js";
import axios from "../../../api/axios";
export default {
  name: "ModifyPassword",
  components: {},
  data() {
    //验证规则
    //旧密码
    let validateOldPassword = (rule, value, callback) => {
      if (value != this.put_form.pwd) {
        callback(new Error("旧密码输入错误"));
      } else {
        callback();
      }
    };

    //新密码
    let validateNewPassword = (rule, value, callback) => {
      if (value === this.put_form.pwd) {
        callback(new Error("新密码不能与旧密码相同!"));
      } else {
        callback();
      }
    };
    //再次输入新密码
    let validateNewPassword2 = (rule, value, callback) => {
      if (value !== this.form.newpassword) {
        callback(new Error("与新密码不一致!"));
      } else {
        callback();
      }
    };
    return {
      AccessTokenFailed: false,
      access_token: null,
      headers: {},
      form: {
        password: "",
        newpassword: "",
        confirmpassword: "",
      },
      //实际put传递的data
      put_form: {},
      //用户ID
      user_id: "",
      //表单规则
      rules: {
        password: [
          { required: true, message: "请输入原密码", trigger: "blur" },
          { validator: validateOldPassword, trigger: "blur" },
        ],
        newpassword: [
          { required: true, message: "请设置新密码", trigger: "blur" },
          { validator: validateNewPassword, trigger: "blur" },
        ],
        confirmpassword: [
          { required: true, message: "请确认新密码", trigger: "blur" },
          { validator: validateNewPassword2, trigger: "blur" },
        ],
      },
    };
  },
  methods: {
    //确认修改密码
    ConfirmButtonClick() {
      this.$msgbox
        .confirm("确定修改密码?", "注意", {
          confirmButtonText: "确定修改",
          cancelButtonText: "取消",
          type: "warning",
          center: true,
        })
        .then((action) => {
          if (action === "confirm") {
            //提交表单检验
            this.$refs["form"].validate((valid) => {
              if (valid) {
                //检验通过
                // this.$message({
                //   type: "success",
                //   message: "密码检验通过！",
                // });
                //修改put的data
                this.put_form.pwd = this.form.newpassword;
                // console.log("this.form", this.form);
                // console.log("this.put_form", this.put_form);
                //put 修改
                axios
                  .request({
                    url: "/API/Elder/" + this.user_id,
                    method: "put",
                    data: this.put_form,
                    headers: this.headers,
                  })
                  .then((res) => {
                    console.log("put更新密码", res);
                    this.$message({
                      type: "success",
                      message: "密码修改成功！",
                    });
                    this.$router.push({
                      name: "UserInfor",
                    });
                  })
                  .catch((err) => {
                    console.log(err);
                    if (
                      err.message.includes("403") &&
                      !this.AccessTokenFailed
                    ) {
                      AccessTokenFailed();
                      this.AccessTokenFailed = true;
                    }
                  });
              } else {
                //检验不通过
                this.$message({
                  type: "warning",
                  message: "密码输入有误！",
                });

                return false;
              }
            });
          } else {
            //无事发生
          }
        })
        .catch((err) => {
          console.log(err);
          // console.log("this.file_data", this.file_data);
        });
    },
    //返回界面
    ExitButtonClick() {
      this.$router.push({
        name: "UserInfor",
      });
    },
  },
  mounted: async function () {
    this.access_token = this.$cookies.get("token");
    this.headers = {
      TokenValue: this.access_token,
    };
    this.user_id = this.$cookies.get("USERID");
    //向后端请求老人信息
    await axios
      .request({
        url: "/API/Elder/" + this.user_id,
        method: "get",
        headers: this.headers,
      })
      .then((res) => {
        console.log("/API/Elder/", res.data.message[0]);
        let raw_data = res.data.message[0];
        //构建put数据，默认是初始的，完全一致
        this.put_form = {
          pwd: raw_data.PASSWORD,
          name: raw_data.NAME,
          idcardno: raw_data.IDCARDNO,
          gender: raw_data.GENDER,
          age: raw_data.AGE,
          phone: raw_data.PHONE,
          height: raw_data.HEIGHT,
          weight: raw_data.WEIGHT,
          address: raw_data.ADDRESS,
          urgent: raw_data.URGENT,
          situation: raw_data.SITUATION,
          communityid: raw_data.COMMUNITYID,
        };
      })
      .catch((err) => {
        console.log(err);
        if (err.message.includes("403") && !this.AccessTokenFailed) {
          AccessTokenFailed();
          this.AccessTokenFailed = true;
        }
      });
  },
};
</script>

// lang选择less语法，scoped限制该样式只在本文件使用，不影响其他组件
<style lang="less" scoped>
.modify_password_view {
  .input_box {
    display: flex;
    justify-content: center;
    flex-direction: column;
    .head_text {
    }
    .input_pwd {
      margin-bottom: 20px;
      width: 50%;
    }
  }
  .button_box {
    margin-top: 20px;
    .confirm_button {
      margin-right: 20px;
    }
    .exit_button {
    }
  }
}
</style>>
