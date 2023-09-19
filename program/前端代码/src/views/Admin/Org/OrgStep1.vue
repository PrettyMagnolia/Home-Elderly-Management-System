
<template>
  <div class="views">
    <h1>机构入驻审核</h1>

    <div style="margin-left:50px">

    </div>

    <el-form
      ref="org_detail"
      :model="org_detail"
      label-width="150px"
      style="margin-left:60px"
      v-loading="loading"
    >
      <el-form-item label="机构名称">
        {{org_detail.NAME}}
      </el-form-item>
      <el-form-item label="机构地址">
        {{org_detail.ADDRESS}}
      </el-form-item>
      <el-form-item label="机构成立日期">
        {{org_detail.ESTABLISHEDTIME}}
      </el-form-item>
      <el-form-item label="申请人手机号">
        {{org_detail.PRINCIPALPHONE}}
      </el-form-item>
      <el-form-item label="机构经营许可证书">
        <div class="demo-image">
          <div class="block">
            <span class="demonstration"></span>
            <el-image
              :src="image_url"
              style="width: 800px; height: 600px;"
            ></el-image>
          </div>
        </div>
      </el-form-item>
      <el-form-item align=center>
        <el-button
          type="primary"
          @click="onSubmit()"
        >审核通过</el-button>
        <el-button
          type="danger"
          @click="onRefuse()"
        >审核不通过</el-button>
        <el-button @click="Cancel()">取消</el-button>
      </el-form-item>
    </el-form>
  </div>
</template>

<script>
import { AccessTokenFailed } from '../../../api/data.js'
import { getOrgCard } from '../../../api/data.js'
//import {postOrgResult} from '../../../api/data.js'
import { putOrgCard } from '../../../api/data.js'
import axios from "../../../api/axios"
export default {
  name: "OrgStep1",
  components: {},
  data () {
    return {
      flag: true,
      org_detail: {},
      image_url: '',
      loading: true,
      access_token: this.$cookies.get("token"),
    }
  },
  methods: {
    toHisObj () {
      let obj = {
        ID: this.org_detail.ID,
        name: this.org_detail.NAME,
        address: this.org_detail.ADDRESS,
        establishedTime: this.org_detail.ESTABLISHEDTIME,
        principalPhone: this.org_detail.PRINCIPALPHONE,
        uploadTime: this.org_detail.UPLOADTIME,
        auditTime: this.org_detail.AUDITTIME,
        isDone: this.org_detail.ISDONE
      }
      this.org_detail = obj;
    },
    Cancel () {
      this.dialogVisible = false;
      this.$router.push({
        name: "OrgQualify",
      })
    },
    dateToString (currentDate)//返回类型为string
    {
      let currentYear = currentDate.getFullYear();
      let currentMonth = (currentDate.getMonth() + 1);
      let currentDay = (currentDate.getDate());

      if (currentMonth < 10)
        currentMonth = '0' + currentMonth;
      if (currentDay < 10)
        currentDay = '0' + currentDay;
      let str = currentYear + '-' + currentMonth + '-' + currentDay;
      return str;
    },
    //审核不通过提交
    onRefuse () {
      //更改信息
      const today = new Date();
      this.org_detail.AUDITTIME = this.dateToString(today);
      this.org_detail.ISDONE = "审核不通过";
      this.toHisObj();
      //向后端发送更改信息
      putOrgCard(this.org_detail.ID, this.org_detail)
        .then((res) => {
          this.$message({
            type: 'success',
            message: '提交成功！审核不通过！'
          });
          console.log(res);
        })
        .catch((err) => {
          if (err.message == "Request failed with status code 403" && this.flag) {
            this.flag = false;
            AccessTokenFailed();
          }
          this.$message.error('后端请求失败！');
          console.log(err);
        })
      this.Cancel();
    },
    //审核通过提交
    onSubmit () {
      //更改信息
      const today = new Date();
      this.org_detail.AUDITTIME = this.dateToString(today);
      this.org_detail.ISDONE = "审核通过";
      this.toHisObj();
      //向后端发送更改信息
      putOrgCard(this.org_detail.ID, this.org_detail)
        .then((res) => {
          this.$message({
            type: 'success',
            message: '提交成功！审核通过！'
          });
          console.log(res);
        })
        .catch((err) => {
          if (err.message == "Request failed with status code 403" && this.flag) {
            this.flag = false;
            AccessTokenFailed();
          }
          this.$message.error('后端请求失败！');
          console.log(err);
        })
      this.Cancel();
    },
    getPhoto (org_id) {
      axios
        .request({
          // 以Blob对象的数据类型返回数据
          // 需要在main.js中注释掉对mock的引用才能够正常生效
          responseType: "blob",
          // url中的 test/png 是由于刚才post到后端的图片为test.png
          url: "/API/file/photo/institution/" + org_id + "/png",
          method: "get",
          headers: {
            TokenValue: this.access_token,
          }
        })
        .then((res) => {
          console.log("成功拿到举报单图片！");
          this.image_url = window.URL.createObjectURL(res.data);
          console.log(res);
          this.loading = false;
        })
        .catch((err) => {
          console.log(err);
          console.log("后端请求图片失败");
          this.get_jpg(org_id);
        });
    },
    get_jpg (org_id) {
      axios.request({
        // 以Blob对象的数据类型返回数据
        // 需要在main.js中注释掉对mock的引用才能够正常生效
        responseType: "blob",
        url: "/API/file/photo/institution/" + org_id + "/jpg",
        method: "get",
        headers: {
          TokenValue: this.access_token,
        }
      })
        .then((res) => {
          console.log("成功拿到举报单图片！");
          this.image_url = window.URL.createObjectURL(res.data);
          console.log(res);
          this.loading = false;
        })
        .catch((err) => {
          console.log(err);
          console.log("后端请求图片失败");
          this.get_jpeg(org_id);
        });
    },
    get_jpeg (org_id) {
      axios.request({
        // 以Blob对象的数据类型返回数据
        // 需要在main.js中注释掉对mock的引用才能够正常生效
        responseType: "blob",
        url: "/API/file/photo/institution/" + org_id + "/jpeg",
        method: "get",
        headers: {
          TokenValue: this.access_token,
        }
      })
        .then((res) => {
          console.log("成功拿到举报单图片！");
          this.image_url = window.URL.createObjectURL(res.data);
          console.log(res);
          this.loading = false;
        })
        .catch((err) => {
          console.log(err);
          console.log("后端请求图片失败");
          this.get_gif(org_id);
        });
    },
    get_gif (org_id) {
      axios.request({
        // 以Blob对象的数据类型返回数据
        // 需要在main.js中注释掉对mock的引用才能够正常生效
        responseType: "blob",
        url: "/API/file/photo/institution/" + org_id + "/gif",
        method: "get",
        headers: {
          TokenValue: this.access_token,
        }
      })
        .then((res) => {
          console.log("成功拿到举报单图片！");
          this.image_url = window.URL.createObjectURL(res.data);
          console.log(res);
          this.loading = false;
        })
        .catch((err) => {
          console.log(err);
          console.log("后端请求图片失败");
          this.loading = false;
        });
    }
  },

  mounted: function () {
    //接收从上一个页面传来的id
    let id = this.$route.query.id;
    //初始化向后端请求数据
    getOrgCard(id)
      .then((res) => {
        this.org_detail = JSON.parse(res.data)[0];

        this.getPhoto(id);
      })
      .catch((err) => {
        if (err.message == "Request failed with status code 403" && this.flag) {
          this.flag = false;
          AccessTokenFailed();
        }
        console.log(err);
      })
  }
};
</script>

// lang选择less语法，scoped限制该样式只在本文件使用，不影响其他组件
<style lang="less" scoped>
</style>>
