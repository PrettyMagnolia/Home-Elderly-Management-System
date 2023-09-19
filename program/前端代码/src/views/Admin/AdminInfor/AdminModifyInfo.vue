<template>
  <div>
    <h3>修改个人信息</h3>

    <el-input
      class="input-item"
      v-model="form.name"
      :disabled="false"
      style="width: 20%; margin-bottom: 20px; margin-right: 5%"
    >
      <template slot="prepend">名字</template>
    </el-input>

    <el-input
      class="input-item"
      v-model="form.idCardNo"
      :disabled="false"
      style="width: 50%; margin-bottom: 20px; margin-right: 10%"
    >
      <template slot="prepend">身份证号</template>
    </el-input>

    <el-input
      class="input-item"
      :disabled="false"
      v-model="form.gender"
      style="width: 20%; margin-bottom: 20px; margin-right: 5%"
    >
      <template slot="prepend">性别</template>
    </el-input>

    <el-input
      class="input-item"
      :disabled="false"
      v-model="form.phone"
      style="
        width: 50%;
        margin-bottom: 20px;
        margin-right: 10%;
        label-width: 1000px;
      "
    >
      <template slot="prepend">电话</template>
    </el-input>

    <el-input
      class="input-item"
      :disabled="false"
      v-model="form.age"
      style="width: 20%; margin-bottom: 20px; margin-right: 5%"
    >
      <template slot="prepend">年龄</template>
    </el-input>
    <br />

    <el-button
      type="primary"
      v-on:click="save"
      style="margin-right: 20px"
    >保存</el-button>
    <el-button v-on:click="exit">返回</el-button>
  </div>
</template>

<script>
import { AccessTokenFailed } from '../../../api/data.js'
import axios from "../../../api/axios";
export default {
  name: "AdminModifyInfo",
  components: {},
  data () {
    //电话号码
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
    return {
      flag: true,
      access_token: this.$cookies.get("token"),
      my_uid: "",
      //表单模型
      form: {},
      //表单检验规则
      rules: {
        phone: [{ validator: validatePhone, trigger: "blur" }],

        name: [{ required: true, message: "请输入姓名", trigger: "blur" }],
        gender: [{ required: true, message: "请输入性别", trigger: "blur" }],

        age: [{ required: true, message: "请输入年龄", trigger: "blur" }],
      },
    };
  },
  methods: {
    //返回
    exit () {
      this.$msgbox
        .confirm("确定退出个人资料修改页面?", "注意", {
          confirmButtonText: "确定退出",
          cancelButtonText: "取消",
          type: "warning",
          center: true,
        })
        .then((action) => {
          if (action === "confirm") {
            //退出界面
            this.$router.push({
              name: "AdminUserInfor",
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
    //提交表单信息，更新个人信息
    save () {
      //msgbox确认
      this.$msgbox
        .confirm("确定更新个人信息?", "注意", {
          confirmButtonText: "确定",
          cancelButtonText: "取消",
          type: "warning",
          center: true,
        })
        .then((action) => {
          if (action === "confirm") {
            //确认更新
            //


            console.log("this.form", this.form);

            axios
              .request({
                url: "/API/Manager/" + this.my_uid,
                method: "put",
                data: this.form,
                headers: {
                  TokenValue: this.access_token,
                },
              })
              .then((res) => {
                console.log("put更新护工信息", res);
                //成功message
                this.$message({
                  type: "success",
                  message: "个人资料更新成功",
                });
                //退出界面
                //退出界面
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
        console.log("this.form", this.form);
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
.disabled /deep/ .el-upload--picture-card {
  display: none !important;
}
.upload_box {
  display: flex;
  .el_upload {
    margin-right: 70px;
  }
  .show_avator {
    .avator_img {
      width: 250px;
    }
  }
}
</style>>
