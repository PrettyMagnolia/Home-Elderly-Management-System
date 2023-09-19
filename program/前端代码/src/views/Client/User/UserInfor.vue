<template>
  <div class="userInfor_page">
    <el-tabs v-model="activeName" class="el_tabs">
      <el-tab-pane label="个人信息" name="first" class="infor_tab">
        <div class="demo-image">
          <div class="block" v-for="fit in fits" :key="fit">
            <span class="demonstration"></span>
            <img
              v-if="image_show"
              :src="image_url"
              alt="ImgError"
              class="avator_img"
              style="
                width: 200px;
                object-fit: cover;
                margin-bottom: 30px;
                margin-top: 10px;
              "
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
          v-model="form.urgent_phone"
          :disabled="true"
          style="width: 50%; margin-bottom: 20px; margin-right: 10%"
          class="show_input"
        >
          <template slot="prepend">紧急电话</template>
        </el-input>
        <el-input
          v-model="form.height"
          :disabled="true"
          style="width: 20%; margin-bottom: 20px; margin-right: 5%"
          class="show_input"
        >
          <template slot="prepend">身高</template>
        </el-input>
        <el-input
          v-model="form.address"
          :disabled="true"
          style="width: 50%; margin-bottom: 20px; margin-right: 10%"
          class="show_input"
        >
          <template slot="prepend">住址</template>
        </el-input>
        <el-input
          v-model="form.weight"
          :disabled="true"
          style="width: 20%; margin-bottom: 20px; margin-right: 5%"
          class="show_input"
        >
          <template slot="prepend">体重</template>
        </el-input>
        <el-input
          v-model="form.situation"
          :disabled="true"
          style="width: 50%; margin-bottom: 20px; margin-right: 5%"
          class="show_input"
        >
          <template slot="prepend">自理情况</template>
        </el-input>
        <el-input
          v-model="form.community_name"
          :disabled="true"
          style="width: 75%; margin-bottom: 20px; margin-right: 20%"
          class="show_input"
        >
          <template slot="prepend">社区名字</template>
        </el-input>
        <el-button type="primary" v-on:click="toModifyInfor"
          >修改个人信息</el-button
        >
        <el-button type="danger" v-on:click="toModifyPassword"
          >修改密码</el-button
        >
        <el-button type="success" v-on:click="clickBack" v-show="backIsOk"
          >返回</el-button
        >
      </el-tab-pane>
      <el-tab-pane label="关注列表" name="second" class="attention_tab">
        <el-tabs>
          <el-tab-pane label="我关注的用户">
            <el-table :data="tableData1" style="width: 100%">
              <el-table-column
                prop="NAME"
                v-model="tableData1.NAME"
                label="姓名"
                width="200"
              >
              </el-table-column>
              <el-table-column
                prop="role"
                v-model="tableData1.role"
                label="身份"
                width="200"
              >
              </el-table-column>
              <el-table-column
                prop="USERID"
                v-model="tableData1.USERID"
                label="账号"
                width="200"
              >
              </el-table-column>
              <el-table-column fixed="right" label="操作" width="100">
                <template slot-scope="scope">
                  <el-button size="mini" @click="viewInfor(scope.row)"
                    >查看主页</el-button
                  >
                </template>
              </el-table-column>
            </el-table>
          </el-tab-pane>

          <el-tab-pane label="关注我的用户">
            <el-table :data="tableData2" style="width: 100%">
              <el-table-column
                prop="NAME"
                v-model="tableData2.NAME"
                label="姓名"
                width="200"
              >
              </el-table-column>
              <el-table-column
                prop="role"
                v-model="tableData2.role"
                label="身份"
                width="200"
              >
              </el-table-column>
              <el-table-column
                prop="USERID"
                v-model="tableData2.USERID"
                label="账号"
                width="200"
              >
              </el-table-column>
              <el-table-column fixed="right" label="操作" width="100">
                <template slot-scope="scope">
                  <el-button size="mini" @click="viewInfor(scope.row)"
                    >查看主页</el-button
                  >
                </template>
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
  name: "UserInfor",
  components: {},
  data() {
    return {
      AccessTokenFailed: false,
      access_token: null,
      headers: {},
      jumpBack: {
        backName: "",
      },
      backIsOk: true,
      activeName: "first",
      fits: ["fill"],
      my_info: {},

      my_uid: "",

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
    clickBack() {
      if (this.backIsOk) {
        console.log(this.jumpBack.backName);
        this.$router.push({
          name: this.jumpBack.backName,
          params: {},
        });
      }
    },
    toModifyInfor() {
      //this.$router.push({ path: "/client/user/modifyInfor" });
      if (this.backIsOk) {
        this.$router.push({
          name: "ModifyInfor",
          params: { jumpBack: this.jumpBack },
        });
      } else {
        this.$router.push({
          name: "ModifyInfor",
          params: {},
        });
      }
    },
    toModifyPassword() {
      this.$router.push({ path: "/client/user/modifyPassword" });
    },
    viewInfor(row) {
      this._id = row.USERID;
      if (row.role == "医生")
      this.$router.push({
        name: "DoctorUserInfor",
        params: {doctor_id: this._id}
      });
      else if (row.role == "护工")
      this.$router.push({
        name: "NurseUserInfor",
        params: {nurse_id: this._id}
      });
    },
  },
  mounted: async function () {
    this.access_token = this.$cookies.get("token");
    this.headers = {
      TokenValue: this.access_token,
    };
    this.my_uid = this.$cookies.get("USERID");
    // 根据uid获取当前老人信息
    await axios
      .request({
        url: "/API/Elder/" + this.my_uid,
        method: "get",
        headers: this.headers,
      })
      .then(async (res) => {
        this.my_info = res.data.message[0];

        //获取社区名字
        // console.log("this.my_info", this.my_info);
        let community_name;
        await axios
          .request({
            url: "/API/community/id",
            method: "get",
            params: {
              id: this.my_info.COMMUNITYID,
            },
            headers: this.headers,
          })
          .then((res) => {
            community_name = res.data[0].NAME;
            // console.log("/API/community/", res.data[0].NAME);
          })
          .catch((err) => {
            console.log(err);
            if (err.message.includes("403") && !this.AccessTokenFailed) {
              AccessTokenFailed();
              this.AccessTokenFailed = true;
            }
          });
        //拼凑显示表单
        this.form = {
          name: this.my_info.NAME,
          cardID: this.my_info.IDCARDNO,
          gender: this.my_info.GENDER,
          phone: this.my_info.PHONE,
          age: this.my_info.AGE,
          urgent_phone: this.my_info.URGENT,
          height: this.my_info.HEIGHT,
          address: this.my_info.ADDRESS,
          weight: this.my_info.WEIGHT,
          situation: this.my_info.SITUATION,
          photo: this.my_info.PHOTO,
          community_name: community_name,
        };
        console.log("this.form", this.form);
      })
      .catch((err) => {
        console.log(err);
        if (err.message.includes("403") && !this.AccessTokenFailed) {
          AccessTokenFailed();
          this.AccessTokenFailed = true;
        }
      });

    // console.log("this.form.photo", this.form.photo);
    if (this.form.photo == null) {
      //本来就无头像
      this.image_url = require("../../../assets/Img/default_avator.png");
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
        })
        .catch((err) => {
          //加载出错，替换error图片
          console.log(err);
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
        // alert("err");
      });
    this.jumpBack = this.$route.params.jumpBack;
    try {
      console.log(this.jumpBack.backName);
    } catch (error) {
      this.backIsOk = false;
      console.log(this.backIsOk);
    } finally {
      if (this.backIsOk) {
        console.log(this.jumpBack.backName);
      }
    }
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