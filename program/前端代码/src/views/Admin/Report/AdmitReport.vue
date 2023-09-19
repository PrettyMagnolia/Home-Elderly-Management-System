
<template>
  <div class="views">
    <div style="margin-bottom:30px">
      <h1 class=head>举报单提交</h1>
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
          label="订单ID"
          prop="order_id"
        >
          <el-input
            v-model="form.order_id"
            placeholder="请输入内容"
          ></el-input>
        </el-form-item>
        <el-form-item
          label="订单服务类型"
          prop="order_type"
        >
          <el-input
            v-model="form.order_type"
            placeholder="请输入内容"
          ></el-input>
        </el-form-item>
        <el-form-item
          label="举报原因"
          prop="report_type"
        >
          <el-input
            v-model="form.report_type"
            placeholder="请输入内容"
          ></el-input>
        </el-form-item>
        <el-form-item
          label="举报详述"
          prop="report_text"
        >
          <el-input
            v-model="form.report_text"
            placeholder="请输入内容"
          ></el-input>
        </el-form-item>

        <el-form-item label="举报截图">
          <div class="card-img">
            <!--照片上传-->
            <el-upload
              class="avatar-uploader"
              action="https://jsonplaceholder.typicode.com/posts/"
              :show-file-list="false"
              :on-success="handleAvatarSuccess"
              :before-upload="beforeAvatarUpload"
            >
              <img
                v-if="imageUrl"
                :src="imageUrl"
                class="avatar"
              >
              <i
                v-else
                class="el-icon-plus avatar-uploader-icon"
              ></i>
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
    </div>

  </div>
</template>

<script>
import { AccessTokenFailed } from '../../../api/data.js'
import { postReport } from '../../../api/data.js'
export default {
  name: "AdmitReport",
  components: {},
  data () {
    return {
      flag: true,
      order_table: [
        {
          id: "0",
          name: "张三",
          sex: "女",
          age: "53",
          date: "2022.2.26",
        },
        {
          id: "1",
          name: "李四",
          sex: "男",
          age: "62",
          date: "2022.5.6",
        },
        {
          id: "2",
          name: "王五",
          sex: "女",
          age: "51",
          date: "2022.7.2",
        },
      ],
      dialogVisible: false,
      selected_index: 0,

      //表单输入的内容
      form: {
        order_id: '',
        order_type: '',
        report_type: '',
        report_text: '',
      },

      //只能选择当前日期之前的作为生日
      pickerOptions: {
        disabledDate (time) {
          return time.getTime() > Date.now();
        }
      },

      rules: {
        order_id: [
          { required: true, message: '请输入订单ID', trigger: 'blur' },
        ],
        order_type: [
          { required: true, message: '请输入订单类型', trigger: 'blur' },
        ],
        report_type: [
          { required: true, message: '请选择举报原因', trigger: 'blur' },
        ],
        report_text: [
          { required: true, message: '请输入您的手机号', trigger: 'blur' },
          { max: 50, message: '举报详述最多不超过50个字符', trigger: 'blur' },
        ],

      },
    }
  },

  methods: {
    onSubmit () {
      this.$alert('这是一段内容', '标题名称', {
        confirmButtonText: '确定',
        callback: action => {
          this.$message({
            type: 'info',
            message: `action: ${action}`
          });
        }
      });
      //向后端传输数据
      postReport(this.form)
        .then((res) => {
          console.log(res);
        })
        .catch((err) => {
          if (err.message == "Request failed with status code 403" && this.flag) {
            this.flag = false;
            AccessTokenFailed();
          }
          console.log(err);
        })


    },
    toConsole () {
      //要重新写！
    },
    Cancel () {
      //要重新写！
    }
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
