<template>
  <div>
    <h3>修改个人信息</h3>

    <el-input
      class="input-item"
      v-model="form.name"
      :disabled="false"
      style="width: 20%; margin-bottom: 20px; margin-right: 5%"
    >
      <template slot="prepend">名字</template>
    </el-input>

    <el-input
      class="input-item"
      v-model="form.cardID"
      :disabled="false"
      style="width: 50%; margin-bottom: 20px; margin-right: 10%"
    >
      <template slot="prepend">身份证号</template>
    </el-input>

    <el-input
      class="input-item"
      :disabled="false"
      v-model="form.gender"
      style="width: 20%; margin-bottom: 20px; margin-right: 5%"
    >
      <template slot="prepend">性别</template>
    </el-input>

    <el-input
      class="input-item"
      :disabled="false"
      v-model="form.phone"
      style="
        width: 50%;
        margin-bottom: 20px;
        margin-right: 10%;
        label-width: 1000px;
      "
    >
      <template slot="prepend">电话</template>
    </el-input>

    <el-input
      class="input-item"
      :disabled="false"
      v-model="form.age"
      style="width: 20%; margin-bottom: 20px; margin-right: 5%"
    >
      <template slot="prepend">年龄</template>
    </el-input>

    <el-input
      class="input-item"
      :disabled="false"
      v-model="form.field"
      style="
        width: 50%;
        margin-bottom: 20px;
        margin-right: 10%;
        label-width: 1000px;
      "
    >
      <template slot="prepend">专业领域</template>
    </el-input>

    <br />
    <el-cascader
      placeholder="社区名字/可输入"
      :options="society_options"
      filterable
      class="cascader"
      :show-all-levels="false"
      v-model="chosen_society"
      style="width: 500px"
    ></el-cascader>

    <h4>个人经历</h4>
    <el-input
      type="textarea"
      class="input-item"
      :disabled="false"
      v-model="form.history"
      style="
        width: 50%;
        margin-bottom: 20px;
        margin-right: 10%;
        label-width: 1000px;
      "
    >
    </el-input>
    <!-- 头像上传 -->
    <!-- 
          action:上传的地址，默认为自动上传，由于需求为点击上传按钮后手动上传，故为空即可
          list-type:文件列表的类型
          auto-upload:是否自动上传
          on-change:文件状态改变时的钩子，添加文件、上传成功和上传失败时都会被调用，由于手动上传，并不会出发上传成功和上传失败钩子，故可仅作为添加文件钩子
          on-remove:文件列表移除文件时的钩子
          on-preview:点击文件列表中已上传的文件时的钩子
          class:判断是否显示后续update框，当前设置为文件列表中的文件数量为一时就不再显示
          file-list:上传的文件列表
         -->
    <div class="upload_box" style="margin-bottom: 30px">
      <div class="el_upload">
        <h4>头像上传更新</h4>
        <el-upload
          action=""
          list-type="picture-card"
          :auto-upload="false"
          :on-change="onUploadChange"
          :on-remove="handleRemove"
          :on-preview="handlePictureCardPreview"
          :class="{ disabled: picture_amount == 1 }"
          :file-list="fileList"
        >
          <i class="el-icon-plus"></i>
        </el-upload>
        <el-dialog :visible.sync="img_dialogVisible">
          <img width="100%" :src="dialogImageUrl" alt="" />
        </el-dialog>
      </div>
      <div class="show_avator">
        <h4>当前头像</h4>
        <img
          v-if="image_show"
          :src="image_url"
          alt="ImgError"
          class="avator_img"
        />
      </div>
    </div>

    <el-button type="primary" v-on:click="save" style="margin-right: 20px"
      >保存</el-button
    >
    <el-button v-on:click="exit">返回</el-button>
  </div>
</template>

<script>
var forbiddenEdit = true;
import axios from "../../../api/axios";
export default {
  name: "NurseModifyInfor",
  components: {},
  data() {
    //电话号码
    var validatePhone = (rule, value, callback) => {
      if (!value) {
        return callback(new Error("请输入电话号码"));
      } else {
        const reg = /^1[3|4|5|7|8][0-9]\d{8}$/;
        const isPhone = reg.test(value);

        value = Number(value); //转换为数字
        if (typeof value === "number" && !isNaN(value)) {
          //判断是否为数字
          value = value.toString(); //转换成字符串
          if (value.length < 0 || value.length > 12 || !isPhone) {
            //判断是否为11位手机号
            callback(new Error("手机号码格式如:138xxxx8754"));
          } else {
            callback();
          }
        } else {
          callback(new Error("请勿输入字符  "));
        }
      }
    };
    return {
      //社区，多级选择
      society_options: [],
      //选择的社区数据
      chosen_society: [],
      userid: "",
      //表单模型
      form: {},
      //表单检验规则
      rules: {
        phone: [{ validator: validatePhone, trigger: "blur" }],
        address: [{ required: true, message: "请输入地址", trigger: "blur" }],
        name: [{ required: true, message: "请输入姓名", trigger: "blur" }],
        gender: [{ required: true, message: "请输入性别", trigger: "blur" }],

        age: [{ required: true, message: "请输入年龄", trigger: "blur" }],
      },

      // 展示放大图的对话框是否显示
      img_dialogVisible: false,
      // 查看放大图的url
      dialogImageUrl: "",
      // 上传文件数据
      file_data: null,
      // 从后端获取的图片url
      image_url: null,
      // 是否展示获取的图片
      image_show: true,
      // 当前上传的图片数量
      picture_amount: 0,
      // 上传的文件列表
      fileList: [],
    };
  },
  methods: {
    //返回
    exit() {
      this.$msgbox
        .confirm("确定退出个人资料修改页面?", "注意", {
          confirmButtonText: "确定退出",
          cancelButtonText: "取消",
          type: "warning",
          center: true,
        })
        .then((action) => {
          if (action === "confirm") {
            //退出界面
            this.$router.push({
              name: "DoctorInfor",
            });
          } else {
            //无事发生
          }
        })
        .catch((err) => {
          console.log(err);
          // console.log("this.file_data", this.file_data);
        });
    },
    //提交表单信息，更新个人信息
    save() {
      //msgbox确认
      this.$msgbox
        .confirm("确定更新个人信息?", "注意", {
          confirmButtonText: "确定",
          cancelButtonText: "取消",
          type: "warning",
          center: true,
        })
        .then((action) => {
          if (action === "confirm") {
            //确认更新
            //
            //提交头像
            if (this.file_data != null) {
              console.log("上传的头像数据", this.file_data);
              let _url = "/API/Doctor/" + this.userid + "/photo";
              // let _url = "/API/nursingworker/" + "112022082200000001" + "/photo";

              axios
                .request({
                  headers: {
                    // 指定传输数据为二进制类型，比如图片、mp3、文件
                    "Content-Type": "multipart/form-data",
                  },
                  url: _url,
                  method: "post",
                  data: this.file_data,
                })
                .then((res) => {
                  console.log(_url, res);
                })
                .catch((err) => {
                  console.log(err);
                });
            }
            //提交完整表单
            var put_data = {
              name: this.form.name,
              pwd: this.form.pwd,
              idcardno: this.form.cardID,
              age: this.form.age,
              phone: this.form.phone,
              field: this.form.field,
              history: this.form.history,
              communityid: this.chosen_society[2],
              gender: this.form.gender,
            };
            // console.log("this.form", this.form);
            console.log("put_data", put_data);
            axios
              .request({
                url: "/API/Doctor",
                method: "put",
                data: put_data,
                params: {
                  id: this.userid,
                },
              })
              .then((res) => {
                console.log("put更新医生信息", res);
                //成功message
                this.$message({
                  type: "success",
                  message: "个人资料更新成功",
                });
                //退出界面

                this.$router.push({
                  name: "DoctorInfor",
                });
              })
              .catch((err) => {
                console.log(err);
              });
          } else {
            //无事发生
          }
        })
        .catch((err) => {
          console.log(err);
          // console.log("this.file_data", this.file_data);
        });
    },
    handleRemove(file, fileList) {
      this.picture_amount -= 1;
      this.file_data = null;
    },
    handlePictureCardPreview(file) {
      this.dialogImageUrl = file.url;
      this.img_dialogVisible = true;
    },
    // 文件状态改变时的钩子，添加文件、上传成功和上传失败时都会被调用
    onUploadChange(file, fileList) {
      console.log("onUploadChange", file);
      this.file_data = file;
      this.picture_amount += 1;
      const isIMAGE =
        file.raw.type === "image/jpeg" ||
        file.raw.type === "image/png" ||
        file.raw.type === "image/gif";
      const isLt1M = file.size / 1024 / 1024 < 1;

      if (!isIMAGE) {
        this.$message.error("上传文件只能是图片格式!");
        this.picture_amount -= 1;
        // 从文件列表中删除最后一个元素
        this.fileList.pop();
        return false;
      }
      if (!isLt1M) {
        this.$message.error("上传文件大小不能超过 1MB!");
        this.picture_amount -= 1;
        // 从文件列表中删除最后一个元素
        this.fileList.pop();
        return false;
      }
    },
  },
  mounted: async function () {
    this.userid = this.$cookies.get("USERID");
    // console.log("this.userid", this.userid);
    //向后端请求医生信息
    await axios
      .request({
        url: "/API/Doctor/id",
        method: "get",
        params: {
          doctorid: this.userid,
        },
      })
      .then((res) => {
        let raw_data = res.data[0];
        console.log("/API/Doctor/id", raw_data);
        this.form = {
          age: raw_data.AGE,
          community_id: raw_data.COMMUNITYID,
          field: raw_data.FIELD,
          gender: raw_data.GENDER,
          history: raw_data.HISTORY,
          cardID: raw_data.IDCARDNO,
          name: raw_data.NAME,
          phone: raw_data.PHONE,

          pwd: raw_data.PASSWORD,
          ban_time: raw_data.BANTIME,
          photo: raw_data.PHOTO,
        };
      })
      .catch((err) => {
        console.log(err);
      });
    if (this.form.photo == null) {
      //本来就无头像
      this.image_url = require("../../../assets/Img/default_avator.png");
    } else {
      //有头像
      //请求图片资源
      console.log("/API/file" + this.form.photo);
      await axios
        .request({
          url: "/API/file" + this.form.photo,
          responseType: "blob",
          method: "get",
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

    //请求社区信息,构造options
    var society_data = {};
    await axios
      .request({ url: "/API/community", method: "get" })
      .then((res) => {
        console.log("/API/community", res);
        //处理数据
        res.data.forEach((ele) => {
          if (ele.CITY in society_data) {
          } else {
            society_data[ele.CITY] = {};
          }
          if (ele.BLOCK in society_data[ele.CITY]) {
            society_data[ele.CITY][ele.BLOCK].push({
              name: ele.NAME,
              id: ele.COMMUNITYID,
            });
          } else {
            society_data[ele.CITY][ele.BLOCK] = [];
          }
        });
        // console.log("society_data", society_data);
      })
      .catch((err) => {
        console.log(err);
      });

    //组装选择器
    let i = 0,
      j = 0;
    for (var city_name in society_data) {
      this.society_options.push({
        value: city_name,
        label: city_name,
        children: [],
      });
      j = 0;
      for (var block_name in society_data[city_name]) {
        this.society_options[i].children.push({
          value: block_name,
          label: block_name,
          children: [],
        });
        society_data[city_name][block_name].forEach((ele) => {
          try {
            this.society_options[i].children[j].children.push({
              value: ele.id,
              label: ele.name,
            });
          } catch (error) {
            console.log(i, ".", j, ";", city_name, block_name, ele.name);
          }
        });
        j++;
      }
      i++;
    }
  },
};
</script>

// lang选择less语法，scoped限制该样式只在本文件使用，不影响其他组件
<style lang="less" scoped>
.disabled /deep/ .el-upload--picture-card {
  display: none !important;
}
.upload_box {
  display: flex;
  .el_upload {
    margin-right: 70px;
  }
  .show_avator {
    .avator_img {
      width: 200px;
    }
  }
}
</style>>
