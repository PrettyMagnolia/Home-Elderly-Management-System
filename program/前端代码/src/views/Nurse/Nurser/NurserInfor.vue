<template>
  <div class="userInfor_page">
    <el-tabs v-model="activeName" class="el_tabs">
      <el-tab-pane label="个人信息" name="first" class="infor_tab">
        <div class="box">
          <img
            v-if="image_show"
            :src="image_url"
            alt="ImgError"
            class="my_avatar"
          />
        </div>
        <el-descriptions class="my_info"  :column="3" border>
          <el-descriptions-item label="姓名">{{form.name}}</el-descriptions-item>
          <el-descriptions-item label="身份证号">{{form.cardID}}</el-descriptions-item>
          <el-descriptions-item label="手机号">{{form.phone}}</el-descriptions-item>
          <el-descriptions-item label="性别">{{form.gender}}</el-descriptions-item>
          <el-descriptions-item label="年龄">{{form.age}}</el-descriptions-item>
          <el-descriptions-item label="所属机构">{{form.ins_name}}</el-descriptions-item>
        </el-descriptions>
        <el-descriptions class="my_info"  :column="1" border>
          <el-descriptions-item label="个人介绍" direction="vertical">

            <el-input
              type="textarea"
              v-model="form.description"
              :disabled="true"
              style="width: 100%;height:auto; margin-bottom: 20px; margin-right: 20%"
              class="show_input"
            >
            </el-input>
          </el-descriptions-item>
        </el-descriptions>
          <!-- <div class="demo-image">
            <div class="block" v-for="fit in fits" :key="fit">
              <span class="demonstration"></span>
              <img
                v-if="image_show"
                :src="image_url"
                alt="ImgError"
                class="avator_img"
                style="width: 300px; margin-bottom: 30px; margin-top: 10px"
              />
            </div>
          </div>
          <el-input
            v-model="form.name"
            :disabled="true"
            style="width: 20%; margin-bottom: 20px; margin-right: 5%"
            class="show_input"
          >
            <template slot="prepend">名字</template>
          </el-input>
          <el-input
            v-model="form.cardID"
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
            v-model="form.ins_name"
            :disabled="true"
            style="width: 75%; margin-bottom: 20px; margin-right: 20%"
            class="show_input"
          >
            <template slot="prepend">所属机构</template>
          </el-input>

          <el-input
            type="textarea"
            v-model="form.description"
            :disabled="true"
            style="width: 60%;height:auto; margin-bottom: 20px; margin-right: 20%"
            class="show_input"
          >
            <template slot="prepend">个人描述</template>
          </el-input> -->
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
import { AccessTokenFailed } from "../../../api/data.js"
import axios from "../../../api/axios";
export default {
  name: "NurserInfor",
  components: {},
  data() {
    return {
      access_token:null,
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

      // 从后端获取的图片url
      image_url: null,
      // 是否展示获取的图片
      image_show: false,
    };
  },
  methods: {
    toModifyInfor() {
      this.$router.push({ name: "NurseModifyInfor" });
    },
    toModifyPassword() {
      this.$router.push({ name: "NurseModifyPassword" });
    },
    viewInfor(row) {
      if (row.role == "医生") this.role = "doctor";
      else if (row.role == "护工") this.role = "nurse";
      this.username = row.username;
      this.$router.push({
        path: `/client/user/${this.role}Infor/${this.username}`,
      });
    },
    getFollow() {
      this.$http
        .get("/API/Follow", {
          params: {
            followerID: this.my_uid,
          },
        })
        .then((result) => {
          console.log("/API/Follow", result);
          let raw_data = JSON.parse(result.data);
          console.log("raw_data[0]", raw_data[0]);
          this.tableData = result.data;
        })
        .catch((err) => {
          console.log(err);
          if (err.message == "Request failed with status code 403") {
            AccessTokenFailed();
          }
        });
    },
  },
  mounted: async function () {
    const loading = this.$loading({
      lock: true,
      text: 'Loading',
      spinner: 'el-icon-loading',
      background: 'rgba(0, 0, 0, 0.7)'
    });
    this.my_uid = this.$cookies.get("USERID");
    this.access_token = this.$cookies.get("token");
    // 根据uid获取当前护工信息
    await axios
      .request({ url: "/API/NursingWorker/" + this.my_uid, method: "get" ,headers: {
          TokenValue: this.access_token,
      },})
      .then(async (res) => {
        this.my_info = res.data.message[0];
        console.log("护工信息", this.my_info);
        //获取机构名字
        let ins_name = "";

        if (this.my_info.INSTITUTIONID != null) {
          //机构非空
          await axios
            .request({
              headers: {
                  TokenValue: this.access_token,
              },
              url: "/API/Institution/" + this.my_info.INSTITUTIONID,
              method: "get",
            })
            .then((res) => {
              console.log(res);
              ins_name = JSON.parse(res.data)[0].NAME;
            })
            .catch((err) => {
              console.log(err);
              if (err.message == "Request failed with status code 403") {
                AccessTokenFailed();
              }
            });
        }

        //拼凑显示表单
        this.form = {
          name: this.my_info.NAME,
          cardID: this.my_info.IDCARDNO,
          gender: this.my_info.GENDER,
          phone: this.my_info.PHONE,
          age: this.my_info.AGE,
          photo: this.my_info.PHOTO,
          ins_name: ins_name,
          description: this.my_info.DESCRIPTION,
        };
        // console.log("this.form", this.form);
      })
      .catch((err) => {
        console.log(err);
        if (err.message == "Request failed with status code 403") {
          AccessTokenFailed();
        }
      });
    // console.log("this.form.photo", this.form.photo);
    if (this.form.photo == null) {
      //本来就无头像
      this.image_url = require("../../../assets/Img/default_avator.png");
      this.image_show = true;
      loading.close();
    } else {
      //有头像
      //请求图片资源
      console.log("/API/file" + this.my_info.PHOTO);
      await axios
        .request({
          headers: {
              TokenValue: this.access_token,
          },
          url: "/API/file" + this.my_info.PHOTO,
          responseType: "blob",
          method: "get",
        })
        .then((res) => {
          console.log("请求后端头像,", res);
          this.image_url = window.URL.createObjectURL(res.data);
          this.image_show = true;
          loading.close();
        })
        .catch((err) => {
          //加载出错，替换error图片
          console.log(err);
          this.image_url = require("../../../assets/Img/ImgError.png");
          this.image_show = true;
        });
    }
    await axios
      .request({ url: "/API/Follow/followerID?followerID=" + this.my_uid, method: 'get' ,headers: {
          TokenValue: this.access_token,
      },})
      .then(async(res) => {
        this.tableData1=JSON.parse(res.data);
      })
      .catch((err) => {
        console.log(err);
        if (err.message == "Request failed with status code 403") {
          AccessTokenFailed();
        }
      });
      await axios
      .request({ url: "/API/Follow/followingID?followingID=" + this.my_uid, method: 'get' ,headers: {
          TokenValue: this.access_token,
      },})
      .then(async(res) => {
        this.tableData2=JSON.parse(res.data);
      })
      .catch((err) => {
        console.log(err);
        if (err.message == "Request failed with status code 403") {
          AccessTokenFailed();
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

.box{
  display: flex;
  justify-content: center;
}
.my_avatar{
  width: 200px;
  margin-bottom: 30px;
  margin-top: 10px;
  border-radius: 50%;
}
.my_info{
  font-size: 20px;
  font-weight: bold;
  font-family: "Hiragino Sans GB";
}
</style>>