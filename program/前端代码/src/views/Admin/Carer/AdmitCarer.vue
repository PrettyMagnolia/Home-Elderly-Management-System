
<template>
  <div class="views">
    <div style="margin-bottom:30px">
      <h1 class=head>医护人员资质提交</h1>
      <el-button
        type="primary"
        icon="el-icon-arrow-left"
        @click="Back()"
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
          label="申请人ID"
          prop="user_id"
        >
          <el-input
            v-model="form.user_id"
            placeholder="请在代码中手动传入用户ID！！！请从cookie中获取ID！"
            :disabled="true"
          ></el-input>
        </el-form-item>

        <el-form-item
          label="身份"
          prop="role"
        >
          <el-input
            v-model="form.role"
            placeholder="请在代码中手动传入用户ID！！！请从cookie中获取ID！"
            :disabled="true"
          ></el-input>
        </el-form-item>

        <!--start of if-else-->
        <div v-if="form.role=='护工'">
          <el-button
            type="primary"
            icon="el-icon-search"
            @click="viewAllInstitution()"
            class=searchButton
            circle
          ></el-button>
          <el-form-item
            label="所属机构编号"
            prop="institution_id"
          >
            <el-input
              v-model="form.institution_id"
              placeholder="请通过按钮选择"
              :disabled="true"
            ></el-input>

          </el-form-item>

          <el-form-item
            label="所属机构名称"
            prop="institution_name"
          >
            <el-input
              v-model="form.institution_name"
              placeholder="请通过按钮选择"
              :disabled="true"
            ></el-input>

          </el-form-item>
        </div>
        <div v-else-if="form.role=='医生'">
          <el-form-item
            label="所属社区编号"
            prop="institution_id"
          >
            <el-input
              v-model="form.institution_id"
              placeholder="该医生没有社区"
              :disabled="true"
            ></el-input>

          </el-form-item>

          <el-form-item
            label="所属社区名称"
            prop="institution_name"
          >
            <el-input
              v-model="form.institution_name"
              placeholder="该医生没有社区"
              :disabled="true"
            ></el-input>
          </el-form-item>
        </div>
        <div v-else>
          传入的ID不属于医生或护工，或医生的社区属性不满足外码约束！
        </div>
        <!--end of if-else-->
        <el-form-item
          label="资质名"
          prop="qualify_name"
        >
          <el-input
            v-model="form.qualify_name"
            placeholder="请输入内容"
          ></el-input>
        </el-form-item>

        <el-form-item
          label="资质说明"
          prop="proof"
        >
          <el-input
            v-model="form.proof"
            placeholder="请输入内容"
          ></el-input>
        </el-form-item>

        <el-form-item
          label="资质授权日期"
          prop="qualify_date"
        >
          <div class="block">
            <span class="demonstration"></span>
            <el-date-picker
              v-model="form.qualify_date"
              type="date"
              placeholder="选择日期"
              :picker-options="pickerOptions"
            >
            </el-date-picker>

          </div>
        </el-form-item>

        <el-form-item label="资质证件照">
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

        <el-form-item align=center>
          <el-button
            type="primary"
            @click="onSubmit()"
          >提交</el-button>
          <el-button @click="Cancel()">取消</el-button>
        </el-form-item>
      </el-form>
    </div>

    <!--搜索机构的弹窗-->
    <el-dialog
      class="serviceDialog"
      title="请选择您的机构"
      :visible.sync="dialogVisible"
      width=1050px
      append-to-body
    >
      <el-card style="width:1000px;">
        <el-input
          placeholder="请输入搜索关键字"
          v-model="input"
          class="input-with-select"
        >
          <el-button
            slot="append"
            icon="el-icon-search"
            @click="postKeyAndFetch()"
          ></el-button>
        </el-input>

        <!--信息展示-->
        <div style="width:1000px;">

          <!-- table -->
          <el-table
            :data="show_table"
            class="order_table"
            height="500"
            width="1000"
            v-loading="loading"
            stripe
          >
            <el-table-column
              prop="ID"
              label="机构编号"
              width="180"
            ></el-table-column>
            <el-table-column
              prop="name"
              label="机构名称"
              width="180"
            ></el-table-column>
            <el-table-column
              prop="address"
              label="机构地址"
              width="180"
            ></el-table-column>

            <el-table-column>
              <template slot-scope="scope">
                <el-button
                  @click.native.prevent="ChooseThis(scope.row)"
                  type="primary"
                  size="small"
                >
                  确认
                </el-button>
              </template>
            </el-table-column>
          </el-table>

        </div>
      </el-card>

    </el-dialog>
  </div>
</template>

<script>
import { AccessTokenFailed } from '../../../api/data.js'
import { getCommunityCard, postQualification } from '../../../api/data.js'
import { getWaitingOrgDetail, getDoctorCard } from '../../../api/data.js'
import axios from "../../../api/axios"
export default {
  name: "AdmitCarer",
  components: {},
  data () {
    return {
      flag: true,
      access_token: this.$cookies.get("token"),
      order_table: [],
      show_table: [],
      dialogVisible: false,
      selected_index: 0,

      //表单输入的内容
      form: {
        user_id: '',
        qualify_name: '',
        proof: '',
        qualify_date: '',

        institution_id: '',
        institution_name: '',

        role: '',
      },
      //分页数据
      query: {
        pageNum: 1,
        pageSize: 10,
        total: 0
      },
      input: '',

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
      loading: true,

      //只能选择当前日期之前的作为生日
      pickerOptions: {
        disabledDate (time) {
          return time.getTime() > Date.now();
        }
      },

      rules: {
        user_id: [
          { required: true, message: '在代码中手动传入！！！！！', trigger: 'blur' },
        ],
        role: [
          { required: true, message: '在代码中手动传入！！！！！', trigger: 'blur' },
        ],
        institution_id: [
          { required: true, message: '请在按钮中选择', trigger: 'blur' },
        ],
        institution_name: [
          { required: true, message: '请在按钮中选择', trigger: 'blur' },
          { min: 2, message: '名字不少于2个字符', trigger: 'blur' },
          { max: 20, message: '最大字符不超过20', trigger: 'blur' },
        ],
        identify_id: [
          { required: true, message: '请输入您的身份证号', trigger: 'blur' },
        ],

        qualify_name: [
          { required: true, message: '请填入您的资质名称', trigger: 'blur' },
        ],

        qualify_date: [
          { required: true, message: '请填入您的资质授权日期', trigger: 'blur' },
        ],

        proof: [
          { required: true, message: '请填入资质说明', trigger: 'blur' },
          { max: 20, message: '最大字符不超过20', trigger: 'blur' },
        ],

      },
    }
  },

  methods: {
    MakeEmpty () {
      for (let i in this.form)
        if (i != 'user_id')
          this.form[i] = '';

      this.file_data = '';
      this.fileList = [];
    },
    FormIsFull () {//返回值为bool类型
      for (let i in this.form)
        if (this.form[i] == '')
          return false;
      return true;
    },
    ChooseThis (row) {
      this.form.institution_id = row.ID;
      this.form.institution_name = row.name;
      this.dialogVisible = false;
    },
    viewAllInstitution () {
      this.dialogVisible = true;
    },
    getRole (id) {
      if (id[0] == '0' && id[1] == '0')
        return "老人";
      else if (id[0] == '1' && id[1] == '0')
        return "医生";
      else if (id[0] == '1' && id[1] == '1')
        return "护工";
      else if (id[0] == '2' && id[1] == '2')
        return "管理员";
      else
        return "未知角色";
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
      if (!this.FormIsFull()) {
        this.$message({
          type: 'info',
          message: '申请单未填写完整！'
        });
        return false;
      }

      this.$msgbox('提交中...', '机构入驻申请提交', {
        confirmButtonText: '正在努力提交中',
        confirmButtonLoading: true,
      });
      let data = {
        workerid: this.form.user_id,
        role: '护工',
        title: this.form.qualify_name,
        proof: this.form.proof,
        granttime: this.form.qualify_date,
        institutionid: this.form.institution_id,
        institutionname: this.form.institution_name,
        adminitorid: '',
        isdone: '待审核',
        audittime: '',
      }

      postQualification(data)
        .then((res) => {
          console.log(res);
          this.uploadPhoto(data.workerid, data.title);
          console.log("资质申请提交成功！");
        })
        .catch((err) => {
          if (err.message == "Request failed with status code 403" && this.flag) {
            this.flag = false;
            AccessTokenFailed();
          }
          console.log(err);
          console.log("资质申请提交失败！");
        })

    },
    Back () {
      //要重新写！Back函数的调用已经写好了(在图片上传后返回上一级页面)，这里只要写跳到哪个页面就好了
      //your code here...

    },
    //得到符合条件的信息
    postKeyAndFetch () {
      this.show_table = this.searchResource();
    },
    searchResource () {
      //函数返回值为筛选后的列表
      const search = this.input;
      let result = this.order_table.slice(0);//深拷贝

      if (search)//有内容才执行关键字筛选
      {
        result = result.filter(data => {
          return Object.keys(data).some(key => {
            return String(data[key]).toLowerCase().indexOf(search) > -1
          })
        })

      }

      //分页参数的更改
      this.query.total = result.length;//总页数设置

      return result;
    },
    //切换当前页显示的数据条数，执行方法
    handleSizeChange (val) {
      //console.log(`每页 ${val} 条`);
      this.query.pageSize = val;
      this.getPageData();
    },
    //切换页数，执行方法
    handleCurrentChange (val) {
      //console.log(`当前页: ${val}`);
      this.query.pageNum = val;
      this.getPageData();
    },
    getPageData () {
      this.postKeyAndFetch();
      const DataAll = this.show_table.slice(0);//深拷贝
      //每次执行方法，将展示的数据清空
      this.show_table = [];
      //第一步：当前页的第一条数据在总数据中的位置
      let strlength = (this.query.pageNum - 1) * this.query.pageSize + 1;
      //第二步：当前页的最后一条数据在总数据中的位置
      let endlength = this.query.pageNum * this.query.pageSize;
      //第三步：此判断很重要，执行时机：当分页的页数在最后一页时，进行重新筛选获取数据时
      //获取本次表格最后一页第一条数据所在的位置的长度
      let resStrLength = 0;
      if (DataAll.length % this.query.pageSize == 0) {
        resStrLength = (parseInt(DataAll.length / this.query.pageSize) - 1) * this.query.pageSize + 1
      } else {
        resStrLength = parseInt(DataAll.length / this.query.pageSize) * this.query.pageSize + 1
      }
      //如果上一次表格的最后一页第一条数据所在的位置的长度 大于 本次表格最后一页第一条数据所在的位置的长度，则将本次表格的最后一页第一条数据所在的位置的长度 为最后长度
      if (strlength > resStrLength) {
        strlength = resStrLength
      }
      //第四步：此判断很重要，当分页的页数切换至最后一页，如果最后一页获取到的数据长度不足最后一页设置的长度，则将设置的长度 以 获取到的数据长度 为最后长度
      if (endlength > DataAll.length) {
        endlength = DataAll.length;
      }
      //第五步：循环获取当前页数的数据，放入展示的数组中
      for (let i = strlength - 1; i < endlength; i++) {
        this.show_table.push(DataAll[i]);
      }
      //数据的总条数
      this.query.total = DataAll.length;
    },
    uploadPhoto (qua_id, title) {
      console.log(title);//
      axios.request({
        headers: {
          TokenValue: this.access_token,
          // 指定传输数据为二进制类型，比如图片、mp3、文件
          'Content-Type': 'multipart/form-data'
        },
        url: '/API/Qualifacation/' + qua_id + '/' + title + '/photo',
        method: "post",
        data: this.file_data,
      })
        .then((res) => {
          console.log(res);
          console.log("图片上传成功！");
          this.$msgbox.close();
          this.Back();
        })
        .catch((err) => {
          if (err.message == "Request failed with status code 403" && this.flag) {
            this.flag = false;
            AccessTokenFailed();
          }
          console.log(err);
          console.log("图片上传失败！");
          this.$msgbox.close();
          this.Back();
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
    this.MakeEmpty();//一开始时先清空表格，但保留用户ID
    //输入用户ID
    this.form.user_id = this.$cookies.get("USERID");
    //this.form.user_id = "112022082200000001";
    this.form.role = this.getRole(this.form.user_id);
    if (this.form.role == '医生') {
      getDoctorCard(this.form.user_id)
        .then((res) => {
          let communityid = res.data[0].COMMUNITYID;

          getCommunityCard(communityid)
            .then((res) => {
              console.log(res);
              let communityname = res.data[0];
              this.form.institution_id = communityid;
              this.form.institution_name = communityname;
              console.log("医生的社区信息获取成功！");
            })
            .catch((err) => {
              console.log(err);
              if (err.message == "Request failed with status code 403" && this.flag) {
                this.flag = false;
                AccessTokenFailed();
              }
            })
        })
        .catch((err) => {
          if (err.message == "Request failed with status code 403" && this.flag) {
            this.flag = false;
            AccessTokenFailed();
          }
          console.log(err);
        })
    }

    //初始时拉取所有机构的信息
    getWaitingOrgDetail()
      .then((res) => {
        let orgList = JSON.parse(res.data);
        orgList.forEach(item => {
          if (item.ISDONE == "审核通过") {
            this.order_table.push({
              ID: item.ID,
              name: item.NAME,
              address: item.ADDRESS
            })
          }
        })
        this.show_table = this.order_table.slice(0);

        this.query.total = this.order_table.length;//总页数设置
        this.loading = false;
      })
      .catch((err) => {
        if (err.message == "Request failed with status code 403" && this.flag) {
          this.flag = false;
          AccessTokenFailed();
        }
        console.log(err);
        this.loading = false;
      })

  },
  watch: {
    input: {
      handler (newVal, oldVal) {
        newVal; oldVal;
        this.postKeyAndFetch();
      }
    }
  }

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
.searchButton {
  position: relative;
  top: 40px;
  left: 30px;
}
.disabled /deep/ .el-upload--picture-card {
  display: none !important;
}
</style>
