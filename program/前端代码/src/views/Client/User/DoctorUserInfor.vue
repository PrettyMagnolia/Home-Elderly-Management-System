<template>
  <div>
    <el-tabs v-model="activeName" @tab-click="handleClick">
    <el-tab-pane label="医生个人信息" name="first">
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
                object-fit: contain;
                margin-bottom: 30px;
                margin-top: 10px;
              "
            />
          </div>
        </div>



    <el-input v-model="form.name" :disabled="true" style="width: 20%; margin-bottom: 20px; margin-right: 5%">
      <template slot="prepend">名字</template>
    </el-input>

    <el-input v-model="form.phone" :disabled="true" style="width: 50%; margin-bottom: 20px; margin-right: 10%" :placeholder="info.PHONE">
      <template slot="prepend">电话</template>
    </el-input>

    <el-input v-model="form.age" :disabled="true" style="width: 20%; margin-bottom: 20px; margin-right: 5%" :placeholder="info.AGE">
      <template slot="prepend">年龄</template>
    </el-input>
    <el-input v-model="form.field" :disabled="true" style="width: 50%; margin-bottom: 20px; margin-right: 5%" :placeholder="info.FIELD">
      <template slot="prepend">主治</template>
    </el-input>
    <el-input v-model="form.history" :disabled="true" style="width: 75%; margin-bottom: 20px; margin-right: 5%" :placeholder="info.HISTORY">
      <template slot="prepend">履历</template>
    </el-input>
    <el-input v-model="form.communityid" :disabled="true" style="width: 75%; margin-bottom: 20px; margin-right: 20%" :placeholder="info.COMMUNITYID">
      <template slot="prepend">社区ID</template>
    </el-input>
    <el-button
        :style="{backgroundColor: bg_color, color: ft_color,}"
        v-on:click="Follow"
          >{{content}}</el-button
    >
    <el-button v-on:click="Exit">返回</el-button>
    </el-tab-pane>
    <el-tab-pane label="医生关注列表" name="second" class="attention_tab">
        <el-tabs>
          <el-tab-pane label="医生关注的用户">
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
            </el-table>
          
          </el-tab-pane>

          <el-tab-pane label="关注医生的用户">
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
      fits: ['fill'],
      activeName: "first",
      form: {},
      info:[],
      tableData1: [],
      tableData2: [],
      rules: {

      },

      followed:false,
      content:"关注",
      bg_color:"#8080ff",
      ft_color:"#ffffff",

      //头像显示需要的
      // 从后端获取的图片url
      image_url: null,
      // 是否展示获取的图片
      image_show: true,
    };
  },

  methods: {
    Exit() {
      this.$router.push({
        name: "UserInfor",
      });
    },
    Follow(e) {
      this.followed=!this.followed;
      if(!this.followed){
          this.content="关注"
          this.bg_color="#8080ff";
          this.ft_color="#ffffff"
          this.$http
        .delete(
          '/API/follow?FOLLOWERID='+this.$cookies.get("USERID")
          +'&FOLLOWINGID='+this.$route.params.doctor_id,
          {
            headers: {
              TokenValue: this.access_token,
            },
          }
        )
        .then((result) => {
          console.log("******delete******");
        })
        .catch((err) => {
          if (
            err.message === "Request failed with status code 500" ||
            err.message === "Request failed with status code 403"
          ) {
            AccessTokenFailed();
          }
          console.log(err);
        });
      this.$message({
        message: "取消关注成功",
        type: "success",
      });
      }

      else{
          this.content="取消关注";
          this.bg_color="#f56c6c";
          this.ft_color="#fef0f0";
          this.$http
        .post(
          "/API/follow",
          {
            FOLLOWERID: this.$cookies.get("USERID"),
            FOLLOWINGID: this.$route.params.doctor_id,
          },
          {
            headers: {
              TokenValue: this.access_token,
            },
          }
        )
        .then((result) => {
          console.log("******success******");
        })
        .catch((err) => {
          if (
            err.message === "Request failed with status code 500" ||
            err.message === "Request failed with status code 403"
          ) {
            AccessTokenFailed();
          }
          console.log(err);
        });
      this.$message({
        message: "关注成功",
        type: "success",
      });
      }

      
    },
  },
  mounted: async function () {
    this.access_token = this.$cookies.get("token");
    this.headers = {
      TokenValue: this.access_token,
    };
    
    this.my_uid = this.$route.params.doctor_id

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

        //拼凑显示表单
        this.form = {
          age: this.my_info.AGE,
          name: this.my_info.NAME,
          communityid: this.my_info.COMMUNITYID,
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
        if (err.message.includes("403")) {
          AccessTokenFailed();
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
          this.image_show = true;
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
      });
    await axios
      .request({
        url: "/API/Follow/followingID?followingID=" + this.my_uid,
        method: "get",
        headers: this.headers,
      })
      .then(async (res) => {
        this.tableData2 = JSON.parse(res.data);
        for(let i = 0; i < this.tableData2.length; i++)
          if(this.$cookies.get("USERID") === this.tableData2[i].USERID)
            {
              this.followed = true;
              this.content = "取消关注";
              this.bg_color = "#f56c6c";
              this.ft_color = "#fef0f0";
              break;
            }
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
