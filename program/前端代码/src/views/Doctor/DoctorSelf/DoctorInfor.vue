<template>
  <div class="userInfor_page">
    <el-tabs v-model="activeName" class="el_tabs">
      <el-tab-pane label="个人信息" name="first" class="infor_tab">
        <div class="image">
          <img
            v-if="image_show"
            :src="image_url"
            alt="ImgError"
            class="avator_img"
          />
        </div>
        <div class="show_box">
          <el-input v-model="form.name" :disabled="true" class="show_input">
            <template slot="prepend">名字</template>
          </el-input>
          <el-input v-model="form.cardID" :disabled="true" class="show_input">
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

          <el-input v-model="form.phone" :disabled="true" class="show_input">
            <template slot="prepend">电话</template>
          </el-input>

          <el-input v-model="form.age" :disabled="true" class="show_input">
            <template slot="prepend">年龄</template>
          </el-input>

          <el-input v-model="form.field" :disabled="true" class="show_input">
            <template slot="prepend">专业领域</template>
          </el-input>

          <el-input
            v-model="form.community_name"
            :disabled="true"
            class="show_input"
          >
            <template slot="prepend">社区</template>
          </el-input>

          <el-input
            type="textarea"
            v-model="form.history"
            :disabled="true"
            class="show_input"
          >
            <template slot="prepend">经历</template>
          </el-input>
        </div>

        <br />
        <el-button type="primary" v-on:click="toModifyInfor"
          >修改个人信息</el-button
        >
        <el-button v-on:click="toModifyPassword">修改密码</el-button>
      </el-tab-pane>

      <el-tab-pane label="关注列表" name="second" class="attention_tab">
        <el-tabs>
          <el-tab-pane label="我关注的用户">
            <el-table
            :data="tableData1"
        style="width: 100%">
        <el-table-column prop="NAME" v-model="tableData1.NAME" label="姓名" width="200">
        </el-table-column>
        <el-table-column prop="role" v-model="tableData1.role" label="身份" width="200">
        </el-table-column>
            </el-table>
          </el-tab-pane>

          <el-tab-pane label="关注我的用户">
            <el-table
            :data="tableData2"
        style="width: 100%">
        <el-table-column prop="NAME" v-model="tableData2.NAME" label="姓名" width="200">
        </el-table-column>
        <el-table-column prop="role" v-model="tableData2.role" label="身份" width="200">
        </el-table-column>
            </el-table>
          </el-tab-pane>
        </el-tabs>
      </el-tab-pane>
    </el-tabs>
  </div>
</template>

<script>
import { AccessTokenFailed } from "../../../api/data.js";
import axios from "../../../api/axios";
export default {
  name: "DoctorUserInfor",
  components: {},
  data() {
    return {
      AccessTokenFailed: false,
      access_token: null,
      headers: {},

      activeName: "first",
      fits: ["fill"],
      my_info: {},
      my_follow: [],
      my_uid: "",
      total_follow: -1,
      loading: true,
      //个人信息绑定的数据表单
      form: {},
      tableData1: [],
      tableData2: [],
      rules: {},

      //头像显示需要的
      // 从后端获取的图片url
      image_url: null,
      // 是否展示获取的图片
      image_show: true,
    };
  },
  methods: {
    toModifyInfor() {
      this.$router.push({ name: "DoctorModifyInfor" });
    },
    toModifyPassword() {
      this.$router.push({ name: "DoctorModifyPassword" });
    },  
  },
  mounted: async function () {
    this.access_token = this.$cookies.get("token");
    this.headers = {
      TokenValue: this.access_token,
    };

    this.my_uid = this.$cookies.get("USERID");
    // 根据uid获取当前医生信息
    await axios
      .request({
        url: "/API/Doctor/id",
        method: "get",
        headers: this.headers,
        params: {
          doctorid: this.my_uid,
        },
      })
      .then(async (res) => {
        this.my_info = res.data[0];
        console.log("医生信息", this.my_info);
        //获取社区名字
        let community_name;

        // if (this.my_info.COMMUNITYID != null) {
        //   //社区非空
        //   await axios
        //     .request({
        //       url: "/API/Institution/" + this.my_info.INSTITUTIONID,
        //       method: "get",
        //     })
        //     .then((res) => {
        //       community_name = JSON.parse(res.data)[0].NAME;
        //     })
        //     .catch((res) => {
        //       console.log(res);
        //     });
        // }

        //拼凑显示表单
        this.form = {
          age: this.my_info.AGE,
          name: this.my_info.NAME,
          community_name: community_name,
          field: this.my_info.FIELD,
          history: this.my_info.HISTORY,
          cardID: this.my_info.IDCARDNO,
          phone: this.my_info.PHONE,
          gender: this.my_info.GENDER,

          photo: this.my_info.PHOTO,
        };
        // console.log("this.form", this.form);
      })
      .catch((err) => {
        console.log(err);
      });
    // console.log("this.form.photo", this.form.photo);
    if (this.form.photo == null) {
      //本来就无头像
      this.image_url = require("../../../assets/Img/default_avator.png");
      console.log("本来就无头像", this.image_url);
      this.image_show = true;
    } else {
      //有头像
      //请求图片资源
      console.log("/API/file" + this.my_info.PHOTO);
      await axios
        .request({
          url: "/API/file" + this.my_info.PHOTO,
          responseType: "blob",
          method: "get",
          headers: this.headers,
        })
        .then((res) => {
          console.log("请求后端头像,", res);
          this.image_url = window.URL.createObjectURL(res.data);
          this.image_show = true;
        })
        .catch((err) => {
          //加载出错，替换error图片
          console.log(err);
          if (err.message.includes("403") && !this.AccessTokenFailed) {
            AccessTokenFailed();
            this.AccessTokenFailed = true;
          }
          this.image_url = require("../../../assets/Img/ImgError.png");
        });
    }
    await axios
      .request({
        url: "/API/Follow/followerID?followerID=" + this.my_uid,
        method: "get",
        headers: this.headers,
      })
      .then(async (res) => {
        this.tableData1 = JSON.parse(res.data);
      })
      .catch((err) => {
        console.log(err);
        if (err.message.includes("403") && !this.AccessTokenFailed) {
          AccessTokenFailed();
          this.AccessTokenFailed = true;
        }
        // alert("err");
      });
    await axios
      .request({
        url: "/API/Follow/followingID?followingID=" + this.my_uid,
        method: "get",
        headers: this.headers,
      })
      .then(async (res) => {
        this.tableData2 = JSON.parse(res.data);
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
.userInfor_page {
  .el_tabs {
    .infor_tab {
      .image {
        .avator_img {
          width: 200px;
          height: 200px;
          object-fit: contain;
        }
        margin-bottom: 30px;
        margin-top: 10px;
      }
      .show_box {
        display: block;
        .show_input {
          margin-bottom: 15px;
          margin-right: 50px;
          width: 400px;
        }
        .show_input /deep/ .el-textarea__inner:disabled {
          color: black;
          font-weight: bold;
        }
        .show_input /deep/ .el-input__inner:disabled {
          color: black;
          font-weight: bold;
        }
      }
    }
    .attention_tab {
    }
  }
}
</style>>