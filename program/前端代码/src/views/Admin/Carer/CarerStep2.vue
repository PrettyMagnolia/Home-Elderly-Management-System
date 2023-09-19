
<template>
  <div class="views">
    <h1>医护人员资质核验</h1>
    <el-steps :active="2">
      <el-step
        title="步骤 1"
        description="身份信息核验"
      ></el-step>
      <el-step
        title="步骤 2"
        description="资质核验"
      ></el-step>
      <el-step
        title="步骤 3"
        description="核验确认"
      ></el-step>
    </el-steps>
    <el-divider></el-divider>

    <el-form
      ref="form"
      :model="form"
      label-width="150px"
      style="margin-left:60px"
      v-loading="loading"
    >
      <el-form-item label="所属机构ID">
        {{form.institutionid}}
      </el-form-item>
      <el-form-item label="所属机构名称">
        {{form.institutionname}}
      </el-form-item>
      <el-form-item label="资质名称">
        {{form.title}}
      </el-form-item>
      <el-form-item label="资质授权日期">
        {{form.granttime}}
      </el-form-item>
      <el-form-item label="资质证件照">
        <div class="demo-image">
          <div class="block">
            <span class="demonstration"></span>
            <el-image
              :src="image_url"
              style="width: 600px; height: 400px;"
            ></el-image>
          </div>
        </div>
      </el-form-item>
      <el-form-item align=center>
        <el-button
          type="primary"
          @click="GotoStep3()"
        >下一步</el-button>

        <el-button @click="Cancel()">取消</el-button>
      </el-form-item>
    </el-form>
  </div>
</template>

<script>
import { AccessTokenFailed } from '../../../api/data.js'
import { postCarerResult } from '../../../api/data.js'
import axios from "../../../api/axios";
export default {
  name: "CarerStep2",
  components: {},
  data () {
    return {
      flag: true,
      image_url: '',
      form: {},
      loading: true,
      access_token: this.$cookies.get("token"),
    }
  },
  methods: {
    getPhoto (id, title) {
      axios
        .request({
          // 以Blob对象的数据类型返回数据
          // 需要在main.js中注释掉对mock的引用才能够正常生效
          responseType: "blob",
          url: "/API/file/photo/qualification/" + id + '_' + title + "/png",
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
          this.get_jpg(id, title);
        });
    },
    get_jpg (id, title) {
      axios
        .request({
          // 以Blob对象的数据类型返回数据
          // 需要在main.js中注释掉对mock的引用才能够正常生效
          responseType: "blob",
          url: "/API/file/photo/qualification/" + id + '_' + title + "/jpg",
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
          this.get_jpeg(id, title);
        });
    },
    get_jpeg (id, title) {
      axios
        .request({
          // 以Blob对象的数据类型返回数据
          // 需要在main.js中注释掉对mock的引用才能够正常生效
          responseType: "blob",
          url: "/API/file/photo/qualification/" + id + '_' + title + "/jpeg",
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
          this.get_gif(id, title);
        });
    },
    get_gif (id, title) {
      axios
        .request({
          // 以Blob对象的数据类型返回数据
          // 需要在main.js中注释掉对mock的引用才能够正常生效
          responseType: "blob",
          url: "/API/file/photo/qualification/" + id + '_' + title + "/gif",
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
    },
    GotoStep3 () {
      //弹窗设置不可见
      this.dialogVisible = false;

      //跳转页面，下订单
      this.$router.push({
        name: "CarerStep3",
        query: { form: this.form }
      })
    },
    Cancel () {
      this.dialogVisible = false;
      this.$router.push({
        name: "CarerQualify",
      })
    },
    //审核不通过提交
    onRefuse () {
      //向后端发失败信息
      postCarerResult()
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
    MakeEmpty () {
      for (let i in this.form) {
        this.form[i] = null;
      }
    },

  },
  mounted: function () {
    this.MakeEmpty();
    this.form = this.$route.query.form;
    console.log("rua", this.form);
    this.getPhoto(this.form.id, this.form.title);
    console.log("rua");
    console.log(this.form);
  }

};
</script>

// lang选择less语法，scoped限制该样式只在本文件使用，不影响其他组件
<style lang="less" scoped>
.views {
  .CareServices-Row {
    display: flex;
    flex-direction: row;
    justify-content: center;
    flex-wrap: wrap;
    .service-card {
      margin-right: 80px;
      margin-bottom: 40px;
      .card-content {
        .card-img {
          display: flex;
          align-content: center;
          justify-content: center;
        }
        .card-info {
          padding-left: 14px;
        }
        .requireServieButton {
          display: flex;
          align-content: center;
          justify-content: center;
        }
      }
    }
  }
}
</style>
