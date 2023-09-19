/*举报单组件
creator:黄彦铭
editor:黄彦铭
需要提交举报单的模块都可以使用这个组件，提交之后的操作都由管理员模块来处理，你们的模块只需要跳出这样一张举报单
不管是哪个模块填写的举报单，最终传给管理员的数据都是那几个，区别只在于哪些信息由用户填写

selection参数用于选择按钮的样式，这个样式是你们告诉我然后我再写的
传入则代表这些参数在举报单中写死了，不传入则代表这个信息由用户填写
至于哪些信息需要用户填写，看各个模块的具体需求

组件有以下6个参数：
1.selection              选择按钮样式。这个样式你们选好了告诉我
2.activeID  activeName   active的意思是主动，这里传入发起举报人的ID和名字
3.passiveID passiveName  passive的意思被动，这里传入被举报人的ID和名字
4.orderID   orderType    订单ID，服务类型
*/

<template>
  <div>
    <el-button
      v-if="selection == 'doctor'"
      @click="dialogOpen()"
      size="mini"
      type="danger"
    >提交举报单</el-button>
    <el-button
      v-else-if="selection == 'doctorBan'"
      @click="dialogOpen()"
      size="mini"
      type="danger"
      disabled
    >提交举报单</el-button>
    <el-button
      v-else-if="selection == 'info'"
      @click="dialogOpen()"
      type="primary"
      icon="el-icon-edit"
    >提交举报单</el-button>
    <el-button
      v-else-if="selection == 'client'"
      @click="dialogOpen()"
      type="primary"
      icon="el-icon-edit"
    >提交举报单</el-button>
    <el-button
      v-else-if="selection == 'nurse'"
      @click="dialogOpen()"
      type="primary"
      icon="el-icon-edit"
    >提交举报单</el-button>
    <el-button
      v-else-if="selection == 'client_normal'"
      @click="dialogOpen()"
      type="warning"
      icon="el-icon-edit"
    >举报</el-button>
    <el-button
      v-else-if="selection == 'client_mini'"
      @click="dialogOpen()"
      type="warning"
      size="mini"
      icon="el-icon-edit"
    >举报</el-button>
    <el-button
      v-else
      @click="dialogOpen()"
      type="primary"
      icon="el-icon-edit"
    >提交举报单</el-button>

    <el-dialog
      class="serviceDialog"
      title="举报单提交"
      :visible.sync="my_dialogVisible"
      width="1000px"
      append-to-body
    >
      <div class="dialog-content">
        <el-form
          :model="form"
          :rules="rules"
          ref="form"
          label-width="200px"
          class="demo-ruleForm"
        >

          <!--       <el-form-item
            label="举报人ID"
            prop="my_passiveID"
          >
            <el-input
              v-model="form.my_activeID"
              placeholder="请输入内容"
              :disabled="bool_input.my_activeID"
            ></el-input>
          </el-form-item>

          <el-form-item
            label="举报人姓名"
            prop="my_passiveName"
          >
            <el-input
              v-model="form.my_activeName"
              placeholder="请输入内容"
              :disabled="bool_input.my_activeName"
            ></el-input>
          </el-form-item>
          -->

          <el-form-item
            label="被举报人ID"
            prop="my_passiveID"
          >
            <el-input
              v-model="form.my_passiveID"
              placeholder="请输入内容"
              :disabled="bool_input.my_passiveID"
            ></el-input>
          </el-form-item>

          <el-form-item
            label="被举报人姓名"
            prop="my_passiveName"
          >
            <el-input
              v-model="form.my_passiveName"
              placeholder="请输入内容"
              :disabled="bool_input.my_passiveName"
            ></el-input>
          </el-form-item>

          <el-form-item
            label="订单ID"
            prop="my_orderID"
          >
            <el-input
              v-model="form.my_orderID"
              placeholder="请输入内容"
              :disabled="bool_input.my_orderID"
            ></el-input>
          </el-form-item>

          <el-form-item
            label="举报原因"
            prop="my_orderType"
          >
            <el-input
              v-model="form.my_orderType"
              placeholder="请输入内容"
            ></el-input>
          </el-form-item>

          <el-form-item
            label="举报详述"
            prop="my_reportText"
          >
            <el-input
              type="textarea"
              :rows="5"
              v-model="form.my_reportText"
              placeholder="请输入内容"
            ></el-input>
          </el-form-item>

          <el-form-item
            label="举报截图"
            prop="pic"
          >
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
          </el-form-item>

        </el-form>
        <!-- 底部的slot插槽 -->
      </div>
      <span
        slot="footer"
        class="dialogFooter"
      >
        <el-button
          type="primary"
          @click="onSubmit()"
        >确认</el-button>
        <el-button @click="Cancel()">取消</el-button>
      </span>
    </el-dialog>
  </div>
</template>

<script>
import { setToken, AccessTokenFailed } from '../api/data.js'
import { postReport, getWaitingReportDetail } from "../api/data.js"
import axios from "../api/axios"
export default {
  name: "ReportBox",
  data () {
    return {
      access_token: this.$cookies.get("token"),
      my_dialogVisible: false,
      form: {
        my_activeID: this.activeID,
        my_activeName: this.activeName,
        my_passiveID: this.passiveID,
        my_passiveName: this.passiveName,

        my_orderID: this.orderID,
        my_orderType: this.orderType,
        //my_reportType: "",
        my_reportText: "",
        my_reportTime: new Date(),
      },
      bool_input: {
        my_activeID: false,
        my_activeName: false,
        my_passiveID: false,
        my_passiveName: false,
        my_orderID: false,
        my_orderType: false,
        my_reportType: false,
        my_reportID: false,
        my_reportText: false,
        my_reportTime: false,
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
      fileList: [],

      rules: {
        my_activeID: [
          { required: true, message: "请输入举报人ID", trigger: "blur" },
        ],
        my_activeName: [
          { required: true, message: "请输入举报人姓名", trigger: "blur" },
        ],
        my_passiveID: [
          { required: true, message: "请输入被举报人ID", trigger: "blur" },
        ],
        my_passiveName: [
          { required: true, message: "请输入被举报人姓名", trigger: "blur" },
        ],
        my_orderID: [
          { required: true, message: "请输入订单ID", trigger: "blur" },
        ],
        my_orderType: [
          { required: true, message: "请输入举报原因", trigger: "blur" },
        ],
        my_reportText: [
          { required: true, message: "请输入举报详述", trigger: "blur" },
          { max: 50, message: "举报详述最多不超过50个字符", trigger: "blur" },
        ],
      },
    };
  },
  //先执行props传参，再执行上面form中的初始化，然后再执行mounted
  props: [
    "selection",
    "activeID",
    "activeName",
    "passiveID",
    "passiveName",
    "orderID",
    "orderType",
  ],


  watch: {
    dialogVisible: {
      immediate: true,
      deep: true,
      handler: function (newVal, oldVal) {
        this.my_dialogVisible = newVal;
        console.log(newVal);
        console.log(oldVal);
        //this.MakeFormEmpty();
      },
    },
  },

  methods: {
    dialogOpen () {
      this.my_dialogVisible = true;
    },
    MakeFormEmpty () {
      const form = this.form;
      const bool_input = this.bool_input;

      for (let i in form) {
        //清空用户填写的部分
        if (bool_input[i] == false) {
          this.form[i] = "";
        }
      }
      this.file_data = null;
      this.fileList = [];
    },
    FormIsFull () {
      const obj = this.form;
      for (let i in obj) {
        if (obj[i] == "") {
          console.log(i);
          return false;
        }
      }
      if (this.fileList.length == 0)
        return false;
      return true;
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
    onSubmit () {
      const today = new Date();
      this.form.my_reportTime = this.dateToString(today);
      //表没填满就不让提交
      if (!this.FormIsFull()) {
        this.$message({
          type: "info",
          message: "举报单未填写完整！",
        });
        return false;
      }
      this.$msgbox('提交中...', '举报单提交', {
        confirmButtonText: '正在努力提交中',
        confirmButtonLoading: true,
      });

      //构造JSON文件
      let data = {
        ID: "001",//随便写一个，到后端重新生成
        imformerID: this.form.my_activeID,
        imformerName: this.form.my_activeName,
        defendantID: this.form.my_passiveID,
        defendantName: this.form.my_passiveName,
        orderID: this.form.my_orderID,
        type: this.form.my_orderType,
        reportTime: this.form.my_reportTime,
        auditTime: "",
        isReal: "",
        punitiveMeasure: "",
        reportText: this.form.my_reportText,
        isDone: "待审核",
      };

      //post给后端
      postReport(data)
        .then((res) => {
          console.log(res);

          this.my_dialogVisible = false;
          //this.MakeFormEmpty();

          getWaitingReportDetail()
            .then((res) => {
              let list = JSON.parse(res.data);
              //上传图片
              let report_id = list[list.length - 1].ID;
              this.uploadPhoto(report_id);
            })

        })
        .catch((err) => {
          console.log(err);
          this.$msgbox.close();
          this.$message({
            type: "info",
            message: "举报单提交失败",
          });
          AccessTokenFailed();
        });
    },
    Cancel () {
      this.my_dialogVisible = false;
      this.$message({
        type: "info",
        message: "提交已取消",
      });

      //this.MakeFormEmpty();
    },
    uploadPhoto (report_id) {
      axios.request({
        headers: {
          TokenValue: this.access_token,
          // 指定传输数据为二进制类型，比如图片、mp3、文件
          'Content-Type': 'multipart/form-data'
        },
        url: "/API/report/" + report_id + "/photo",
        method: "post",
        data: this.file_data,
      })
        .then((res) => {
          console.log(this.access_token);
          console.log(res);
          console.log("图片上传成功！");
          this.$msgbox.close();
          this.$message({
            type: "success",
            message: "举报单提交成功",
          });
          this.dialogVisible = false;
        })
        .catch((err) => {
          console.log(err);
          console.log("图片上传失败！");
          this.$msgbox.close();
          this.$message({
            type: "success",
            message: "举报单提交成功",
          });
          this.dialogVisible = false;
        });
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
  },
  mounted: function () {
    var access_token = this.$cookies.get('token');
    setToken(access_token);
    this.access_token = access_token;

    this.bool_input.my_activeID =
      typeof this.activeID == "undefined" ? false : true;
    this.bool_input.my_activeName =
      typeof this.activeName == "undefined" ? false : true;
    this.bool_input.my_passiveID =
      typeof this.passiveID == "undefined" ? false : true;
    this.bool_input.my_passiveName =
      typeof this.passiveName == "undefined" ? false : true;
    this.bool_input.my_orderID =
      typeof this.orderID == "undefined" ? false : true;
    this.bool_input.my_orderType =
      typeof this.orderType == "undefined" ? false : true;

    //获取当前时间
    this.form.my_reportTime = new Date();

    this.MakeFormEmpty();
  },
};
</script>


<style lang="less" scoped>
.disabled /deep/ .el-upload--picture-card {
  display: none !important;
}
</style>