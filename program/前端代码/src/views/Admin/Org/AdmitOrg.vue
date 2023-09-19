
<template>
  <div class="views">
    <div style="margin-bottom:30px">
      <h1 class=head>机构入驻申请</h1>
      <el-button
        type="primary"
        icon="el-icon-arrow-left"
        @click="toConsole()"
        class=backButton
      >返回</el-button>
    </div>

    <div style="float:left;">
      <!-- tab -->
      <el-form
        :model="form"
        :rules="rules"
        ref="form"
        label-width="200px"
        class="demo-ruleForm"
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
              :class="{disabled:picture_amount == 1}"
              :file-list="fileList"
            >
              <i class="el-icon-plus"></i>
            </el-upload>
          </div>
        </el-form-item>

        <el-form-item align=center>
          <el-button
            type="primary"
            @click="onSubmit()"
          >提交</el-button>
          <el-button @click="Cancel()">取消</el-button>
        </el-form-item>
      </el-form>

      <el-button @click="uploadPhoto(1)">测试上传图片</el-button>
    </div>
  </div>

</template>

<script>
import { AccessTokenFailed } from '../../../api/data.js'
import { getWaitingOrgDetail, postOrg } from '../../../api/data.js'
import axios from "../../../api/axios";
export default {
  name: "AdmitOrg",
  components: {},
  data () {
    return {
      flag: true,
      //order_table: [],
      access_token: this.$cookies.get("token"),
      dialogVisible: false,
      selected_index: 0,
      //表单输入的内容
      form: {
        org_name: '',
        address: '',
        phone_num: '',
        setup_date: '',
        upload_time: '',
      },

      // 查看放大图的url
      dialogImageUrl: '',
      // 上传文件数据
      file_data: null,
      // 从后端获取的图片url
      image_url: null,
      // 是否展示获取的图片
      image_show: false,
      // 当前上传的图片数量
      picture_amount: 0,
      // 上传的文件列表
      fileList: [

      ],

      //只能选择当前日期之前的作为生日
      pickerOptions: {
        disabledDate (time) {
          return time.getTime() > Date.now();
        }
      },

      rules: {
        org_name: [
          { required: true, message: '请输入机构名称', trigger: 'blur' },
          { max: 20, message: '最大字符不超过20', trigger: 'blur' },
        ],
        address: [
          { required: true, message: '请输入机构名称', trigger: 'blur' },
          { max: 50, message: '最大字符不超过50', trigger: 'blur' },
        ],
        phone_num: [
          { required: true, message: '请输入申请人手机号', trigger: 'blur' },
          { pattern: /^(13[0-9]|14[01456879]|15[0-35-9]|16[2567]|17[0-8]|18[0-9]|19[0-35-9])\d{8}$/, message: '请输入正确的手机号', trigger: 'blur' }
        ],
        setup_date: [
          { required: true, message: '请填入机构出生日期', trigger: 'blur' },
        ],



      },
    }
  },

  methods: {
    MakeEmpty () {
      this.form = {
        org_name: '',
        address: '',
        phone_num: '',
        setup_date: '',
        upload_time: '',
      };
      this.file_data = null;
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
    // 文件状态改变时的钩子，添加文件、上传成功和上传失败时都会被调用
    onUploadChange (file) {
      console.log(file);
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
    handleRemove () {
      this.picture_amount -= 1;
    },
    handlePictureCardPreview (file) {
      this.dialogImageUrl = file.url;
      this.dialogVisible = true;
    },
    FormIsFull () {
      for (let i in this.form)
        if (this.form[i] == '') {
          console.log(i);
          return false;
        }
      return true;
    },
    onSubmit () {
      const today = new Date();
      this.form.upload_time = this.dateToString(today);
      //如果信息没填满
      if (!this.FormIsFull()) {
        this.$message({
          type: 'info',
          message: '机制审核单未填写完整！'
        });
        return false;
      }
      this.$msgbox('提交中...', '机构入驻申请提交', {
        confirmButtonText: '正在努力提交中',
        confirmButtonLoading: true,
      });
      //构造JSON文件
      let data = {
        ID: "123",//随便写一个，到后端重新生成
        name: this.form.org_name,
        address: this.form.address,
        establishedTime: this.form.setup_date,
        principalPhone: this.form.phone_num,
        uploadTime: this.form.upload_time,
        auditTime: '',
        isDone: '待审核',
      }

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
              if (err.message == "Request failed with status code 403" && this.flag) {
                this.flag = false;
                AccessTokenFailed();
              }
              console.log(err);
            })
        })
        .catch((err) => {
          if (err.message == "Request failed with status code 403" && this.flag) {
            this.flag = false;
            AccessTokenFailed();
          }
          console.log(err);
          this.$msgbox.close();
          this.$message({
            type: 'info',
            message: '资质审核单提交失败'
          });
          return false;
        })
      return true;
    },
    toConsole () {
      //要重新写！
    },
    Cancel () {
      //要重新写！
    },
    uploadPhoto (org_id) {
      axios.request({
        headers: {
          //TokenValue: this.access_token,
          // 指定传输数据为二进制类型，比如图片、mp3、文件
          'Content-Type': 'multipart/form-data'
        },
        url: "/API/report/" + "442022083000000033" + "/photo",
        method: "post",
        data: this.file_data,
      })
        .then((res) => {
          console.log(res);
          this.$msgbox.close();
          this.$message({
            type: 'success',
            message: '资质审核单提交成功！'
          });
          console.log("图片上传成功！");
          console.log(org_id);
        })
        .catch((err) => {
          if (err.message == "Request failed with status code 403" && this.flag) {
            this.flag = false;
            AccessTokenFailed();
          }
          console.log(err);
          this.$msgbox.close();
          this.$message({
            type: 'success',
            message: '资质审核单提交成功！'
          });
          console.log("图片上传失败！");
        });
    },

  },

};
</script>

// lang选择less语法，scoped限制该样式只在本文件使用，不影响其他组件
<style lang="less" scoped>
.head {
  float: left;
  margin-right: 800px;
}
.backButton {
  float: right; // 靠右
}

.avatar-uploader .el-upload {
  border: 1px dashed #d9d9d9;
  border-radius: 6px;
  cursor: pointer;
  position: relative;
  overflow: hidden;
}
.avatar-uploader .el-upload:hover {
  border-color: #409eff;
}
.avatar-uploader-icon {
  font-size: 28px;
  color: #8c939d;
  width: 178px;
  height: 178px;
  line-height: 178px;
  text-align: center;
}
.avatar {
  width: 178px;
  height: 178px;
  display: block;
}
</style>>
