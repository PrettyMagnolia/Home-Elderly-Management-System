<template>
  <div class="views">
    <el-table
      :data="qualify_data"
      style="width: 100%"
      v-loading="loading">
      <el-table-column
      type="index"
      width="50">
      </el-table-column>
      <el-table-column
        prop="TITLE"
        label="资质名称"
        width="300"
        sortable>
      </el-table-column>
      <el-table-column
        prop="GRANTTIME"
        label="资质授权日期"
        width="290"
        sortable>
      </el-table-column>
      <el-table-column
        prop="UPLOADTIME"
        label="资质提交日期"
        width="290"
        sortable>
      </el-table-column>
      <el-table-column
        prop="ISDONE"
        label="审核状态"
        width="120"
        :filter-multiple="false"
        :filters="filter_info"
        :filter-method="filterHandler">
        <template slot-scope="scope">
          <el-tag
          :type="getStateTagType(scope.row.ISDONE)"
          disable-transitions>
            {{ scope.row.ISDONE }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column align="center" fixed="right">
          <template slot-scope="scope">
            <el-button
              @click="ViewDetailButtonClick(scope.row)"
              type="primary"
              size="mini">
              查看详情
            </el-button>
          </template>
      </el-table-column>
    </el-table>
    <br/> 
    <el-button type="text" icon="el-icon-plus" @click="ClickAddButton()">添加新的资质证明</el-button>
    <!-- 弹窗对话框 -->
    <el-dialog
      class="serviceDialog"
      title="服务详情"
      :visible.sync="dialogVisible"
      v-if="dialogVisible"
    >
      <div class="dialog-content">
        <div class="dialog-info">

          <!-- 展示用户名和手机号 -->
          <el-descriptions class="margin-top" direction="vertical" :column="1" border>
            <el-descriptions-item>
              <template slot="label">
                <i class="el-icon-menu"></i>
                资质名称
              </template>
              <template >
                {{details.TITLE}}
              </template>

            </el-descriptions-item>

            <el-descriptions-item>
              <template slot="label">
                <i class="el-icon-edit-outline"></i>
                资质说明
              </template>
              {{ details.PROOF }}
            </el-descriptions-item>
          </el-descriptions>

          <!-- 展示服务地址 -->
          <el-descriptions class="margin-top" :column="2" border>
            <el-descriptions-item>
              <template slot="label">
                <i class="el-icon-alarm-clock"></i>
                资质授权日期
              </template>
              {{ details.GRANTTIME }}
            </el-descriptions-item>
            <el-descriptions-item>
              <template slot="label">
                <i class="el-icon-timer"></i>
                资质提交日期
              </template>
              {{ details.UPLOADTIME }}
            </el-descriptions-item>
            <el-descriptions-item>
              <template slot="label">
                <i class="el-icon-collection-tag"></i>
                所属机构ID
              </template>
              {{ details.INSTITUTIONID }}
            </el-descriptions-item>
            <el-descriptions-item>
              <template slot="label">
                <i class="el-icon-s-home"></i>
                所属机构名称
              </template>
              {{ details.INSTITUTIONNAME }}
            </el-descriptions-item>
          </el-descriptions> 

          <!-- 展示服务名称、类型、时间、订单价格 -->
          <el-descriptions class="margin-top" direction="vertical" :column="1" border>
            

            <el-descriptions-item>
              <template slot="label">
                <i class="el-icon-picture"></i>
                资质证件照
              </template>
              <img v-if="dialogVisible" :src="image_url" alt="" style="width: 100%" />
            </el-descriptions-item>

          </el-descriptions>
          <el-descriptions class="margin-top" :column="2" border>


            <el-descriptions-item>
              <template slot="label">
                <i class="el-icon-bell"></i>
                审核状态
              </template>
              <el-tag :type="getStateTagType(details.ISDONE)" size="small">{{ details.ISDONE }}</el-tag>
            </el-descriptions-item>

          </el-descriptions>
        </div>
      </div>

      <!-- 底部的slot插槽 -->
      <span slot="footer" class="dialogFooter">
        <el-button 
          type="primary"  
          @click="ClickResubmitButton()" 
          v-if="details.ISDONE==='审核不通过'"
          >重新提交</el-button
        >
        <el-button @click="dialogVisible = false">关闭</el-button>
      </span>
    </el-dialog>
  </div>
</template>

<script>
import axios from "../../../api/axios";
import { AccessTokenFailed } from "../../../api/data.js"
export default {
  name: "NurserQualification",
  data() {
    return {
      access_token:null,
      my_uid:null,
      my_info:null,
      my_ins_info:null,
      // 所有资质信息
      qualify_data:null,
      // 弹窗对话框是否显示
      dialogVisible:false,
      // 点击查看详情后的信息
      details:{},
      // 从后端获取的图片url
      image_url: null,
      loading:true,
      filter_info:[{text: '待审核', value: '待审核'}, {text: '审核通过', value: '审核通过'}, {text: '审核不通过', value: '审核不通过'}]
    };
  },
  methods: {
    // 从后端获取所有资质信息
    get_my_qualification(){
      return axios.request({
          headers: {
              TokenValue: this.access_token,
          },
          url:"/API/Qualifacation/"+ this.my_uid,
          method:'get',
      })
    },
    // 从后端获取对应资质的图片
    get_qualify_photo(photo_url){
      return axios.request({
          headers: {
              TokenValue: this.access_token,
          },
          url:"/API/file" + photo_url,
          method:'get',
          responseType: "blob",
      })
    },
    // 根据uid获取当前护工信息
    getNurseInfo () {
      return axios.request({
        headers: {
          TokenValue: this.access_token,
        },
        url: "/API/NursingWorker" + "/" + this.my_uid,
        method: "get",
      })
    },
    getInstitutionInfo () {
      return axios.request({
        headers: {
          TokenValue: this.access_token,
        },
        url: "/API/Institution" + "/" + this.my_info.INSTITUTIONID,
        methods: 'get'
      })
    },
    // 根据审核状态区分标签类型
    getStateTagType(state){
      if(state === "审核通过"){
        return "success";
      }
      else if(state === "审核不通过"){
        return "danger";
      }
      else if(state === "待审核"){
        return "primary";
      }
      else{
        console.log(state);
      }
    },
    // 点击查看详情按钮后的操作
    ViewDetailButtonClick(row){
      console.log(row);
      this.details = row;
      if(row.PHOTO !== null){
        this.get_qualify_photo(row.PHOTO)
        .then((res) =>{
          this.image_url = window.URL.createObjectURL(res.data);
          this.dialogVisible = true;
        })
        .catch((err) =>{

          console.log(err);
          this.image_url = require("../../../assets/Img/ImgError.png");
          this.dialogVisible = true;
        })
      }
      else{
        this.image_url = require("../../../assets/Img/ImgError.png");
        this.dialogVisible = true;
      }
    },
    ClickResubmitButton(){
      this.$router.push({
        name: "AddQualification",
        params:{
            resubmit:true,
            info:this.details, 
        }
      });
    },
    ClickAddButton(){
      this.$router.push({
        name: "AddQualification",
        params:{
            resubmit:false,
            info:this.my_ins_info,
        }
      });
    },
    filterHandler(value, row, column) {
      const property = column['property'];
      return row[property] === value;
    },
    // 初始化加载所有数据
    load_all_data(){
      this.get_my_qualification()
        .then((res) => {
          this.qualify_data = JSON.parse(res.data);
          console.log(this.qualify_data);
          this.getNurseInfo()
          .then(res =>{
            this.my_info = res.data.message[0];
            this.loading = false;
            if(this.my_info.INSTITUTIONID !== null){
              this.getInstitutionInfo()
              .then(res => {
                console.log(res);
                this.my_ins_info = JSON.parse(res.data)[0];
              })
              .catch(err => {
                console.log(err);
                if (err.message == "Request failed with status code 403") {
                  AccessTokenFailed();
                  this.loading = false;
                }
              })
            }
          })
          .catch(err => {
            console.log(err);
            if (err.message == "Request failed with status code 403") {
              AccessTokenFailed();
              this.loading = false;
            }
          })
        })
        .catch(err => {
          console.log(err);
          if (err.message == "Request failed with status code 403") {
            AccessTokenFailed();
            this.loading = false;
          }
        })
      },
      
    },
  mounted: function () {
    this.my_uid = this.$cookies.get("USERID");
    this.access_token = this.$cookies.get("token");
    this.load_all_data();
  },
};
</script>

<!-- lang选择less语法，scoped限制该样式只在本文件使用，不影响其他组件 -->
<style lang="less" scoped>

</style>>