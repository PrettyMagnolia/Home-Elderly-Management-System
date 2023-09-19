
<template>
  <div class="ApplyIns">
    <div class="head_box">
      <h1 class="head_text">护理机构入驻申请</h1>
    </div>

    <div class="form_box">
      <!-- tab -->
      <el-form
        :model="form"
        :rules="rules"
        ref="form"
        label-width="150px"
        class="el_form"
      >
        <el-form-item
          label="机构名称"
          prop="org_name"
        >
          <el-input
            v-model="form.org_name"
            placeholder="请输入内容"
          ></el-input>
        </el-form-item>

        <el-form-item
          label="机构详细地址"
          prop="address"
        >
          <el-input
            v-model="form.address"
            placeholder="请输入内容"
          ></el-input>
        </el-form-item>

        <!--出生日期-->
        <el-form-item
          label="机构成立日期"
          prop="setup_date"
        >
          <div class="block">
            <span class="demonstration"></span>
            <el-date-picker
              v-model="form.setup_date"
              type="date"
              placeholder="选择日期"
              :picker-options="pickerOptions"
            >
            </el-date-picker>
          </div>
        </el-form-item>

        <el-form-item
          label="申请人手机号"
          prop="phone_num"
        >
          <el-input
            v-model="form.phone_num"
            placeholder="请输入内容"
          ></el-input>
        </el-form-item>

        <el-form-item
          label="机构经营许可证书"
          prop="pic"
        >
          <div class="card-img">
            <!--照片上传-->
            <el-upload
              action=""
              list-type="picture-card"
              :auto-upload="false"
              :on-change="onUploadChange"
              :on-remove="handleRemove"
              :on-preview="handlePictureCardPreview"
              :class="{disabled:this.fileList.length === 1}"
              :file-list="fileList"
            >
              <i class="el-icon-plus"></i>
            </el-upload>
          </div>
        </el-form-item>

        <el-form-item align="center">
          <el-button
            type="primary"
            @click="onSubmit()"
          >提交</el-button>
          <!-- 返回button -->
          <el-button
            icon="el-icon-arrow-left"
            @click="toLogin()"
            class="backButton"
          >返回</el-button>
        </el-form-item>
      </el-form>
    </div>
  </div>
</template>

<script>
import { getWaitingOrgDetail, postOrg, setToken } from "../../api/data.js";
import axios from "../../api/axios";
export default {
  name: "ApplyIns",
  components: {},
  data () {
    return {
      access_token: null,
      headers: {},
      //order_table: [],
      dialogVisible: false,
      selected_index: 0,
      //表单输入的内容
      form: {
        org_name: "",
        address: "",
        phone_num: "",
        setup_date: "",
        upload_time: "",
        //pic:'',
      },

      // 查看放大图的url
      dialogImageUrl: "",
      // 上传文件数据
      file_data: null,
      // 从后端获取的图片url
      image_url: null,
      // 是否展示获取的图片
      image_show: false,
      // 当前上传的图片数量
      picture_amount: 0,
      // 上传的文件列表
      fileList: [],

      //只能选择当前日期之前的作为生日
      pickerOptions: {
        disabledDate (time) {
          return time.getTime() > Date.now();
        },
      },

      rules: {
        org_name: [
          { required: true, message: "请输入机构名称", trigger: "blur" },
          { max: 20, message: "最大字符不超过20", trigger: "blur" },
        ],
        address: [
          { required: true, message: "请输入机构名称", trigger: "blur" },
          { max: 50, message: "最大字符不超过50", trigger: "blur" },
        ],
        phone_num: [
          { required: true, message: "请输入申请人手机号", trigger: "blur" },
          {
            pattern:
              /^(13[0-9]|14[01456879]|15[0-35-9]|16[2567]|17[0-8]|18[0-9]|19[0-35-9])\d{8}$/,
            message: "请输入正确的手机号",
            trigger: "blur",
          },
        ],
        setup_date: [
          { required: true, message: "请填入机构成立日期", trigger: "blur" },
        ],
      },
    };
  },

  methods: {
    MakeEmpty () {
      this.form = {
        org_name: "",
        address: "",
        phone_num: "",
        setup_date: "",
        upload_time: "",
      };
      this.file_data = '';
      this.fileList = [];
    },
    dateToString (
      currentDate //返回类型为string
    ) {
      let currentYear = currentDate.getFullYear();
      let currentMonth = currentDate.getMonth() + 1;
      let currentDay = currentDate.getDate();

      if (currentMonth < 10) currentMonth = "0" + currentMonth;
      if (currentDay < 10) currentDay = "0" + currentDay;
      let str = currentYear + "-" + currentMonth + "-" + currentDay;
      return str;
    },
    // 文件状态改变时的钩子，添加文件、上传成功和上传失败时都会被调用
    onUploadChange (file, fileList) {
      console.log(file);
      this.fileList = fileList;
      this.file_data = file;
      this.picture_amount += 1;
      const isIMAGE = (file.raw.type === 'image/jpeg' || file.raw.type === 'image/png' || file.raw.type === 'image/gif');
      const isLt1M = file.size / 1024 / 1024 < 1;

      if (!isIMAGE) {
        this.$message.error('上传文件只能是图片格式!');
        this.picture_amount -= 1;
        // 从文件列表中删除最后一个元素
        this.fileList.pop();
        return false;
      }
      if (!isLt1M) {
        this.$message.error('上传文件大小不能超过 1MB!');
        this.picture_amount -= 1;
        // 从文件列表中删除最后一个元素
        this.fileList.pop();
        return false;
      }
    },
    handleRemove (file, fileList) {
      setTimeout(() => {
        this.fileList = fileList;
      }, 1000);
    },
    handlePictureCardPreview (file) {
      this.dialogImageUrl = file.url;
      this.dialogVisible = true;
    },
    FormIsFull () {
      for (let i in this.form)
        if (this.form[i] == "") {
          console.log(i);
          return false;
        }
      if (this.fileList.length == 0)
        return false;
      return true;
    },
    onSubmit () {

      const today = new Date();
      this.form.upload_time = this.dateToString(today);
      //如果信息没填满
      if (!this.FormIsFull()) {
        this.$message({
          type: "info",
          message: "机制审核单未填写完整！",
        });
        return false;
      }
      this.$alert("提交中...", "机构入驻申请提交", {
        confirmButtonText: "正在努力提交中",
        confirmButtonLoading: true,
      });
      //构造JSON文件
      let data = {
        ID: "123", //随便写一个，到后端重新生成
        name: this.form.org_name,
        address: this.form.address,
        establishedTime: this.form.setup_date,
        principalPhone: this.form.phone_num,
        uploadTime: this.form.upload_time,
        auditTime: "",
        isDone: "待审核",
      };

      console.log(data);
      postOrg(data)
        .then((res) => {
          console.log(res);
          //发送照片
          getWaitingOrgDetail()
            .then((res) => {
              let list = JSON.parse(res.data);
              const id = list[list.length - 1].ID;
              this.uploadPhoto(id);
            })
            .catch((err) => {
              console.log(err);
            });
        })
        .catch((err) => {
          console.log(err);
          this.$msgbox.close();
          this.$message({
            type: "info",
            message: "资质审核单提交失败",
          });
          this.toLogin();
          return false;
        });
      return true;
    },
    //返回login界面
    toLogin () {
      //退出管理员账号
      //清空local
      window.localStorage.removeItem("refresh_token");
      window.localStorage.removeItem("role");
      //清空cookies
      this.$cookies.remove("token");
      this.$cookies.remove("role");
      this.$cookies.remove("USERID");
      console.log("成功退出临时管理员账号！")

      this.$router.push({ name: "Login" });
    },
    uploadPhoto (org_id) {
      axios
        .request({
          headers: {
            // 指定传输数据为二进制类型，比如图片、mp3、文件
            "Content-Type": "multipart/form-data",
            TokenValue: this.access_token,
          },
          url: "/API/institution/" + org_id + "/photo",
          method: "post",
          data: this.file_data,
        })
        .then((res) => {
          console.log(res);
          this.$msgbox.close();
          this.$message({
            type: "success",
            message: "资质审核单提交成功！",
          });
          console.log("图片上传成功！");
          console.log(org_id);
          this.toLogin();
        })
        .catch((err) => {
          console.log(err);
          this.$msgbox.close();
          this.$message({
            type: "success",
            message: "资质审核单提交成功！",
          });
          console.log("图片上传失败！");
          this.toLogin();
        });
    },
  },
  mounted: async function () {
    this.MakeEmpty();
    this.access_token = this.$cookies.get("token");
    setToken(this.access_token);
    
    this.headers = {
      TokenValue: this.access_token,
    };
  },
};
</script>

// lang选择less语法，scoped限制该样式只在本文件使用，不影响其他组件
<style lang="less" scoped>
.ApplyIns {
  .head_box {
    display: flex;
    flex-direction: row;
    justify-content: center;
    align-items: center;
    .head_text {
    }
    .backButton {
      display: block;
      margin-left: 80px;
      float: right; // 靠右
    }
  }
  .form_box {
    .el_form {
    }
  }
}
.disabled /deep/ .el-upload--picture-card {
  display: none !important;
}
</style>
