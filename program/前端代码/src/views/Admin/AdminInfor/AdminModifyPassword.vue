<template>
  <div class="modify_password_view">
    <el-form
      :model="form"
      status-icon
      :rules="rules"
      ref="form"
      label-width="100px"
      class="input_box"
    >
      <el-form-item
        label="原密码"
        prop="password"
      >
        <el-input
          type="password"
          v-model="form.password"
          placeholder="请输入原密码"
        ></el-input>
      </el-form-item>
      <el-form-item
        label="新密码"
        prop="newpassword"
      >
        <el-input
          type="password"
          v-model="form.newpassword"
          placeholder="请输入新密码"
        ></el-input>
      </el-form-item>
      <el-form-item
        label="确认密码"
        prop="confirmpassword"
      >
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
      >确认修改</el-button>
      <el-button
        class="exit_button"
        @click="ExitButtonClick"
      >返回</el-button>
    </div>
  </div>
</template>

<script>
import { AccessTokenFailed } from '../../../api/data.js'
import axios from "../../../api/axios";
export default {
  name: "NurseModifyPassword",
  components: {},
  data () {
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
      flag: true,
      access_token: this.$cookies.get("token"),
      form: {
        password: "",
        newpassword: "",
        confirmpassword: "",
      },
      //实际put传递的data
      put_form: {},
      //用户ID
      my_uid: "",
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
    ConfirmButtonClick () {
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
                    url: "/API/Manager/" + this.my_uid,
                    method: "put",
                    data: this.put_form,
                    headers: {
                      TokenValue: this.access_token,
                    },
                  })
                  .then((res) => {
                    console.log("put更新密码", res);
                    this.$message({
                      type: "success",
                      message: "密码修改成功！",
                    });
                    this.$router.push({
                      name: "AdminUserInfor",
                    });
                  })
                  .catch((err) => {
                    if (err.message == "Request failed with status code 403" && this.flag) {
                      this.flag = false;
                      AccessTokenFailed();
                    }
                    console.log(err);
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
          if (err.message == "Request failed with status code 403" && this.flag) {
            this.flag = false;
            AccessTokenFailed();
          }
          console.log(err);
          // console.log("this.file_data", this.file_data);
        });
    },
    //返回界面
    ExitButtonClick () {
      this.$router.push({
        name: "AdminUserInfor",
      });
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
        this.put_form = {
          ID: raw_data.ID,
          name: raw_data.NAME,
          pwd: raw_data.PWD,
          idCardNo: raw_data.IDCARDNO,
          gender: raw_data.GENDER,
          age: raw_data.AGE,
          phone: raw_data.PHONE,
          position: raw_data.POSITION,
        };

        // console.log("this.put_form", this.put_form);
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
