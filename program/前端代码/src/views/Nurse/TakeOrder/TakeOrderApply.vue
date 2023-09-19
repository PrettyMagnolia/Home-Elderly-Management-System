<!--
  护工模块接单页面
-->
<template>
  <div class="views">

    <el-card class="card-item">
      <el-row>
        <el-col :span="21" >
          <div class="condition-title">筛选条件</div>
        </el-col>
        <el-col :span="3">
          <el-button size="small" class="title-button" @click="ResetButtonClick"> &ensp;重置&ensp; </el-button>
          <el-button type="primary" size="small" class="title-button" @click="FilterButtonClick"> &ensp;筛选&ensp; </el-button>
        </el-col>
      </el-row>
      <!-- 搜索框部分 -->
      <div class="condition-body">
        <el-form :inline="true" class="condition-form" @submit.native.prevent>
          <el-form-item class="conditon-form-item1" label="关键词">
            <!-- 带输入建议的文本框 -->
            <!-- 默认的下拉框poppers是挂载到body上的，所以设置的css样式el-autocomplete-suggestion在App.vue中 -->
            <el-autocomplete
              class="inline-input"
              :trigger-on-focus="false"
              popper-class="el-autocomplete-suggestion"
              v-model="filter_key"
              :fetch-suggestions="ReturnInputPrompt"
              placeholder="请输入"
              size="small"
              @keyup.enter.native="submit"
            ></el-autocomplete>
          </el-form-item>

          <el-form-item class="conditon-form-item2" label="护理时间">
            <el-date-picker
              v-model="filter_time"
              size="small"
              type="datetimerange"
              range-separator="至"
              start-placeholder="开始日期"
              end-placeholder="结束日期"
              value-format="yyyy-MM-dd HH:mm:ss" >
            </el-date-picker>
          </el-form-item>
        </el-form>
      </div>
    </el-card>

    <!-- 申请列表部分 -->
    <el-card class="card-item">
      <!-- 表格主体 -->
      <el-table
        ref="singleTable"
        :data="page_care_orders"
        highlight-current-row
        style="width: 100%"
        @sort-change='handleSortChange'
        @filter-change="handleFilterChange"
        v-loading="loading">

        <!-- 索引列 -->
        <!-- 注意该索引和订单信息中的索引并不相同 -->
        <!-- 在进行排序操作后是通过订单信息中的索引属性来获取对应信息 -->
        <!-- 而该索引列只是为了方便用户查看 -->
        <el-table-column
        type="index"
        :index="indexMethod">
        </el-table-column>

        <!-- 姓名列 -->
        <el-table-column
          property="NAME"
          label="姓名"
          width="180">
        </el-table-column>

        <!-- 服务名称列 具有筛选功能-->
        <el-table-column
          property="TYPEID"
          label="服务类型"
          width="200"
          :filters="care_types"
          :filter-multiple="false"
          column-key="care_type"
        >
          <template slot-scope="scope">
            {{getCareTypeName(scope.row.TYPEID)}}
          </template>
        </el-table-column>

        <!-- 护理时间列 -->
        <el-table-column
          property="ORDER_TIME"
          label="护理时间"
          sortable="custom"  
          width="250">
        </el-table-column>

        <!-- 服务地址列 -->
        <el-table-column
          property="ADDRESS"
          label="服务地址">
        </el-table-column>

        <!-- 查看详情列 -->
        <el-table-column align="center">
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

      <!-- 分页栏 -->
      <div class="form-page">
        <el-pagination
          background
          layout="prev, pager, next"
          :page-size="page_size"
          :total="total_items"
          :current-page="current_page"
          @current-change="HandlePageChange"
        >
        </el-pagination>
      </div>
    </el-card>
    <!-- 弹窗对话框 -->
    <el-dialog
      class="serviceDialog"
      title="服务详情"
      :visible.sync="dialogVisible"
      v-if="dialogVisible"
    >
      <el-dialog
        width="30%"
        title="老人详细信息"
        :visible.sync="innerVisible"
        append-to-body>
        <el-descriptions class="margin-top" :column="2" border>
          <el-descriptions-item>
            <template slot="label">
              <i class="el-icon-user"></i>
              姓名
            </template>
            {{ elder_info.NAME }}
          </el-descriptions-item>
          <el-descriptions-item>
            <template slot="label">
              <i class="el-icon-time"></i>
              年龄
            </template>
            {{ elder_info.AGE }}
          </el-descriptions-item>
          <el-descriptions-item>
            <template slot="label">
              <i class="el-icon-view"></i>
              性别
            </template>
            {{ elder_info.GENDER }}
          </el-descriptions-item>
          <el-descriptions-item>
            <template slot="label">
              <i class="el-icon-phone-outline"></i>
              电话
            </template>
            {{ elder_info.PHONE }}
          </el-descriptions-item>
          <el-descriptions-item>
            <template slot="label">
              <i class="el-icon-menu"></i>
              身高
            </template>
            {{ elder_info.HEIGHT }} cm
          </el-descriptions-item>
          <el-descriptions-item>
            <template slot="label">
              <i class="el-icon-s-marketing"></i>
              体重
            </template>
            {{ elder_info.WEIGHT }} kg
          </el-descriptions-item>
        </el-descriptions>
        <el-descriptions class="margin-top" direction="vertical" :column="1" border>
          <el-descriptions-item>
            <template slot="label">
              <i class="el-icon-tickets"></i>
              既往病史
            </template>
            <el-table
              :data="elder_history_disease"
              border
              style="width: 100%">
              <el-table-column
                prop="NAME"
                label="疾病名称">
              </el-table-column>
              <el-table-column
                prop="STARTTIME"
                label="患病日期">
              </el-table-column>
            </el-table>
          </el-descriptions-item>
        </el-descriptions>
        <!-- 底部的slot插槽 -->
        <span slot="footer" class="dialogFooter">
          <el-button
          :style="{backgroundColor:bg_color, color: ft_color,}"
          @click="ClickFollowButton()"
            >{{content}}</el-button
          >
        </span>
      </el-dialog>
      <div class="dialog-content">
        <div class="dialog-info">

          <!-- 展示用户名和手机号 -->
          <el-descriptions class="margin-top" :column="2" border>
            <el-descriptions-item>
              <template slot="label">
                <i class="el-icon-user"></i>
                用户姓名
              </template>
              <template >
                <el-button type="text" @click="ClickElderDetailButton()">{{details.name}}</el-button>
              </template>
              
            </el-descriptions-item>

            <el-descriptions-item>
              <template slot="label">
                <i class="el-icon-mobile-phone"></i>
                手机号
              </template>
              {{ details.phone }}
            </el-descriptions-item>
          </el-descriptions>

          <!-- 展示服务地址 -->
          <el-descriptions class="margin-top" direction="vertical" :column="1" border>
            <el-descriptions-item>
              <template slot="label">
                <i class="el-icon-location-outline"></i>
                服务地址
              </template>
              {{ details.address }}
            </el-descriptions-item>
          </el-descriptions> 

          <!-- 展示服务名称、类型、时间、订单价格 -->
          <el-descriptions class="margin-top" :column="2" border>
            <el-descriptions-item>
              <template slot="label">
                <i class="el-icon-menu"></i>
                服务类型
              </template>
              {{ details.type }}
            </el-descriptions-item>

            <el-descriptions-item>
              <template slot="label">
                <i class="el-icon-time"></i>
                服务时间
              </template>
              {{ details.order_time }}
            </el-descriptions-item>

          </el-descriptions>
      
          <el-descriptions class="margin-top" direction="vertical" :column="1" border>
            <el-descriptions-item>
              <template slot="label">
                <i class="el-icon-zoom-in"></i>
                详细内容
              </template>
              <el-table
                :data="details.service_items"
                border
                style="width: 100%">
                <el-table-column
                  prop="NAME"
                  label="服务项目">
                </el-table-column>
                <el-table-column
                  prop="per_price"
                  label="单价">
                </el-table-column>
                <el-table-column
                  prop="COUNT"
                  label="个数">
                </el-table-column>
              </el-table>
            </el-descriptions-item>
          </el-descriptions>

          <el-descriptions class="margin-top" :column="2" border>
            <el-descriptions-item>
              <template slot="label">
                <i class="el-icon-money"></i>
                订单总价
              </template>
              {{ details.price }}元
            </el-descriptions-item>
            <el-descriptions-item>
              <template slot="label">
                <i class="el-icon-bell"></i>
                订单状态
              </template>
              <el-tag size="small">{{ details.state }}</el-tag>
            </el-descriptions-item>
            <el-descriptions-item>
              <template slot="label">
                <i class="el-icon-chat-line-round"></i>
                自理情况
              </template>
              <el-tag size="small">{{ details.situation }}</el-tag>
            </el-descriptions-item>

            <el-descriptions-item>
              <template slot="label">
                <i class="el-icon-s-operation"></i>
                需求
              </template>
              <el-tag size="small">{{ details.need }}</el-tag>
            </el-descriptions-item>
          </el-descriptions>
        </div>
      </div>

      <!-- 底部的slot插槽 -->
      <span slot="footer" class="dialogFooter">
        <el-button type="primary"  @click="EnterPlaceOrderButtonClick()" 
          >提交接单申请</el-button
        >
        <el-button @click="dialogVisible = false">取消</el-button>
      </span>
    </el-dialog>

  </div>
</template>

<script>
import {  AccessTokenFailed } from '../../../api/data.js'
import axios from "../../../api/axios"
import Axios from "axios";
import py from "pinyin"
export default {
  name: "TakeOrderApply",
  data() {
    return {
      AccessTokenFailed:false,
      innerVisible:false,
      access_token:null,
      elder_info:null,
      elder_history_disease:null,
      my_uid:"",
      my_info:{},
      // 判断数据是否准备就绪（使得从后台获取好数据后再渲染DOM）
      loading:true,
      // 弹窗对话框的可见性
      dialogVisible: false,
      // 护理类型
      care_types:[],
      // 所有护理记录（总记录）
      all_care_orders:[],
      all_length:null,
      // 当前筛选信息（关键词和服务时间）下的搜索记录
      current_care_orders:[],
      // 通过护理类型筛选时的备份记录
      // 查看详情的表单信息
      details:{
        index:null,
        name:null,
        phone:null,
        address:null,
        type:null,
        order_time:null,
        price:null,
        state:null,
        service_items:null,
        situation:null,
        need:null,
      },
      // datetimePicker组件绑定的数据
      filter_time:null,
      // input组件绑定的数据
      filter_key:null,
      // 当前页面
      current_page:1,
      // 页面大小
      page_size:10,
      Today:"",

      followed:false,
      content:"关注",
      bg_color:"#8080ff",
      ft_color:"#ffffff",
      tableData2:[],
    };
  },
  computed:{
    // 根据当前页面号生成对应页面中的数据
    // 当current_page和all_care_orders改变时都会自动更新
    page_care_orders: function() {
      return this.current_care_orders.slice(this.page_size*(this.current_page - 1),this.page_size*(this.current_page));
    },
    total_items: function() {
      return this.current_care_orders.length;
    }
  },
  methods: {
    /* 后端请求方法 */
    // 从后端获取所有服务类型
    get_care_type(){
      return axios.request({
          headers: {
              TokenValue: this.access_token,
          },
          url:"/API/ServiceType",
          method:'get',
      })
    },
    // 从后端获取所有能够接单的服务记录
    get_all_orders(){
      return axios.request({
          headers: {
              TokenValue: this.access_token,
          },
          url:"/API/ServiceRecord",
          method:'get',
          params: {
              status: 0,
              not_in_wait_nurse_id:this.my_uid
          },
      })
    },
    // 根据指定的老人ID从后端获取对应老人信息
    get_elder_info(id){
      return axios.request({
          headers: {
              TokenValue: this.access_token,
          },
          url:"/API/Elder" + "/" + id,
          method:'get',
      })
    },
    // 根据uid获取当前护工信息
    getNurseInfo(){
        return axios.request({
          headers: {
              TokenValue: this.access_token,
          },
          url: "/API/NursingWorker" + "/" + this.my_uid,
          method: "get",
        })
    },
    // 根据老人的ID获取所有既往病史
    get_elder_history_disease(id){
      return axios.request({
        headers: {
            TokenValue: this.access_token,
        },
        url: "/API/history/id",
        method: "get",
        params:{
          elderid:id
        },
      })
    },
    /* 顶部筛选栏相关方法 */
    // el-autocomplete组件返回输入提示
    ReturnInputPrompt(queryString, cb) {
      // 存放返回结果的数组
      let results = [];
      // 将查询字符分解并转为正则表达式，用于模糊搜索
      let queryStringArr = queryString.split("");
      let str = "(.*?)";
      let regStr = str + queryStringArr.join(str) + str;
      let reg = RegExp(regStr, "i"); // 以mh为例生成的正则表达式为/(.*?)m(.*?)h(.*?)/i

      // search_info目前只有地址和老人姓名信息
      this.current_care_orders.some(element => {
        Object.keys(element).some(key => {
          if(key !== "NAME" && key !== "ADDRESS")
            return false;
          let val = element[key];
          // 获取汉字的拼音，并进行扁平化
          let pyArr = py(val, {
            style: py.STYLE_NORMAL // 设置拼音风格设置为普通风格（不带声调），
          }).flat();
          let pyStr = pyArr.join("");

          // 拼音符合正则表达式 || 文字符合正则表达式
          if((reg.test(pyStr)) || (reg.test(val))){
            let re = false;
            results.some(element => {
              if(element.value === val){
                // 存在重复的
                re = true;
                return true;
              }
            });
            // 创建符合组件要求的对象（一定要有value值）
            if(!re){
              let obj = {value:val};
              results.push(obj);
            }
            // 跳出遍历
            return true;
          }
        })
      })
      // 调用 callback 返回建议列表的数据
      cb(results);
    },
    // 根据筛选栏中的关键词搜索数据（对象是全部数据）
    FilterDataByKey(){
      let queryString = this.filter_key;
      // 如果关键词为空，则返回所有数据
      if(queryString === ""){
        return this.all_care_orders;
      }
      let results = [];
      // 将查询字符分解并转为正则表达式，用于模糊搜索
      let queryStringArr = queryString.split("");
      let str = "(.*?)";
      let regStr = str + queryStringArr.join(str) + str;
      let reg = RegExp(regStr, "i"); // 以mh为例生成的正则表达式为/(.*?)m(.*?)h(.*?)/i

      this.all_care_orders.some(element => {
        Object.keys(element).some(key => {
          if(key !== "NAME" && key !== "ADDRESS")
            return false;
          let val = element[key];
          // 获取汉字的拼音，并进行扁平化
          let pyArr = py(val, {
            style: py.STYLE_NORMAL // 设置拼音风格设置为普通风格（不带声调），
          }).flat();
          let pyStr = pyArr.join("");

          // 拼音符合正则表达式 || 文字符合正则表达式
          if((reg.test(pyStr)) || (reg.test(val))){
            // 创建符合组件要求的对象（一定要有value值）
            results.push(element);
            // 跳出遍历
            return true;
          }
        })
      })
      return results;
    },
    // 根据筛选栏中的时间范围搜索数据（对象是全部数据）
    FilterDataByTime(){
      let start = this.filter_time[0];
      let end = this.filter_time[1];
      let results = this.all_care_orders.filter((e) => {
        return e.ORDER_TIME >= start && e.ORDER_TIME <= end
      })
      return results;
    },
    // 点击筛选框中的筛选按钮的操作
    FilterButtonClick(){
      if(this.filter_time === null && this.filter_key ===null){
        this.current_care_orders = this.all_care_orders;
      }
      else if(this.filter_time === null && this.filter_key !==null){
        // 根据关键词来进行筛选
        this.current_care_orders = this.FilterDataByKey();
      }
      else if(this.filter_key === null && this.filter_time ===null){
        // 根据时间进行筛选
        this.current_care_orders = this.FilterDataByTime();
      }
      else{
        // 根据时间进行筛选
        this.current_care_orders = this.FilterDataByTime();
        // 根据关键词进行筛选
        this.current_care_orders = this.FilterDataByKey();
      }
      this.current_care_orders_copy = this.current_care_orders; 
      this.setDefaultFilter();
    },
    // 点击筛选框中的重置按钮的操作
    ResetButtonClick(){
      // 清空搜索框中的内容
      this.filter_key = null;
      this.filter_time = null;
      // 重置数据
      this.current_care_orders = this.all_care_orders;
      this.current_care_orders_copy = this.current_care_orders;
    },
    // 设置默认的筛选条件为全部
    setDefaultFilter() {
      const column = this.$refs.singleTable.columns[2];
      column.filteredValue = [];
      this.$refs.singleTable.store.commit('filterChange', {
        column,
        values: column.filteredValue,
        silent: true
      })
    },
    
    /* 主体表格中的方法 */ 
    // 生成表格中每行的索引的方法
    indexMethod(index) {
      return index + (this.current_page - 1)*this.page_size + 1;
    },
    // 根据护理类型ID返回对应护理类型的名称
    getCareTypeName(typeID) {
      let typeName = "";
      this.care_types.forEach(element => {
        if(element.value == typeID){
          typeName = element.text;
          return;
        }
      });
      return typeName;
    },
    // 根据状态号返回状态名称
    getOrderStatusName(status){
      if(status === "0"){
        return "已申请";
      }
      else if(status === "1"){
        return "正在进行";
      }
      else if(status === "2"){
        return "已完成";
      }
      else{
        console.log(status);
      }
    },
    // 根据自理状态返回自理信息
    getSituationName(situation){
      if(situation === "0"){
        return "完全自理";
      }
      else if(situation === "1"){
        return "失能";
      }
      else if(situation === "2"){
        return "失智";
      }
      else if(situation === "3"){
        return "失能&失智";
      }
      else{
        console.log(situation);
      }
    },
    // 根据老人需求号返回需求信息
    getNeedName(need){
      if(need === "0"){
        return "长期居家";
      }
      else if(need === "1"){
        return "医院陪护";
      }
      else if(need === "2"){
        return "短期照护";
      }
      else{
        console.log(need);
      }
    },
    // 对表格进行筛选（对象是当前页面中数据）筛选项为护理类型
    handleFilterChange(info){
      let tmp_type = info.care_type;
      if(tmp_type.length === 0){
        // 需要全部数据
        this.current_care_orders = this.current_care_orders_copy;
      }
      else{
        this.current_care_orders = this.current_care_orders_copy.filter((e) => {
          return e.TYPEID === tmp_type[0];
        })
      }
    },
    // 对表格进行排序（对象是当前页面中数据）排序项为时间
    handleSortChange(info){
      console.log("排序时传入的列数据info:",info);
      if(info.prop === "ORDER_TIME"){  // 对时间进行排序
        if(info.order === "ascending"){  //升序排列
          this.all_care_orders.sort(function(a,b){
            let flag = a.ORDER_TIME > b.ORDER_TIME ? 0 : -1;
            return flag;
          });
        }
        else{
          this.all_care_orders.sort(function(a,b){
            let flag = b.ORDER_TIME > a.ORDER_TIME ? 0 : -1;
            return flag;
          });
        }
      }
    },
    // 点击查看详情按钮后的操作
    ViewDetailButtonClick(row) {
      console.log("传入的行信息row:",row);
      // 修改detail对象的内容
      this.details.record_id = row.RECORDID;
      this.details.address = row.ADDRESS;
      this.details.type = this.getCareTypeName(row.TYPEID);
      this.details.order_time = row.ORDER_TIME;
      this.details.price = row.PRICE;
      this.details.state = this.getOrderStatusName(row.STATUS);
      this.details.service_items = JSON.parse(row.service_items);
      this.details.situation = this.getSituationName(row.SITUATION);
      this.details.need = this.getNeedName(row.NEED);
      this.get_elder_info(row.ELDERID)
      .then((res)=>{
        // 修改detail对象的内容
        this. elder_info = res.data.message[0];
        console.log("老人信息",this.elder_info);
        this.details.name = this.elder_info.NAME;
        this.details.phone = this.elder_info.PHONE;

        // 使弹窗可见
        this.dialogVisible = true;
      })
      .catch((err)=>{
          console.log(err);
          if (err.message == "Request failed with status code 403" && !this.AccessTokenFailed) {
            AccessTokenFailed();
            this.AccessTokenFailed = true;
          }
      });  
    },

    /* 分页栏相关方法 */
    // 在页面改变时进行的操作
    HandlePageChange(currentPage) {
      // 更改当前页面
      this.current_page = currentPage;
    },

    /* 弹窗对话框相关方法 */
    // 点击接单按钮后的操作
    EnterPlaceOrderButtonClick() {
      //弹窗设置不可见
      this.dialogVisible = false;
      if(this.my_info.bantime >= this.Today || this.my_info.bantime !== null){
        // 账号被封禁中
        this.$message.error('账号封禁中，无法进行接单操作！');
        return;
      }
      else if(this.my_info.INSTITUTIONID === null){
        this.$message.error('在完成护工资质核验之前，无法进行接单操作！');
        return;
      }
      console.log(this.my_info.bantime,this.Today);
      // 弹出确认信息
      this.$msgbox
      .confirm("是否确认提交接单申请", "提示", {
        confirmButtonText: "确定",
        cancelButtonText: "取消",
        type: "info",
        center: true,
      }).then(() => {
        this.$message({
          type: 'success',
          message: '申请提交成功!'
        });
        console.log(this.details.record_id);
        // 向后端传输该护工接单数据
        axios
        .request({
          headers: {
              TokenValue: this.access_token,
          },
          url: "/API/servicerecord/nurse",
          method: "post",
          data:{
            recordid:this.details.record_id,
            nurseid:this.my_uid
          }
        })
        .then((res) => {
          console.log(res);
          // 刷新页面
          this.refresh_data();
          console.log("yes");
        })
        .catch((err) => {
          console.log(err);
          if (err.message == "Request failed with status code 403" && !this.AccessTokenFailed) {
            AccessTokenFailed();
            this.AccessTokenFailed = true;
          }
        });

      }).catch(() => {
        this.$message({
          type: 'info',
          message: '申请已取消!'
        });          
      });
    },
    ClickElderDetailButton(){
      this.get_elder_history_disease(this.elder_info.USERID)
      .then(res => {
        console.log("老人的既往病史",res);
        this.elder_history_disease = res.data;
      })
      .catch(err => {
        console.log(err);
        if (err.message == "Request failed with status code 403" && !this.AccessTokenFailed) {
          AccessTokenFailed();
          this.AccessTokenFailed = true;
        }
      })
      this.access_token = this.$cookies.get("token");
      this.headers = {
        TokenValue: this.access_token,
      };

        axios.request({
          url: "/API/Follow/followingID?followingID=" + this.elder_info.USERID,
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

        });
      this.innerVisible = true;
    },
    ClickFollowButton(){
      this.followed=!this.followed;
      if(!this.followed){
          this.content="关注"
          this.bg_color="#8080ff";
          this.ft_color="#ffffff"
          this.$http
        .delete(
          '/API/follow?FOLLOWERID='+this.$cookies.get("USERID")
          +'&FOLLOWINGID='+this.elder_info.USERID,
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
            FOLLOWINGID: this.elder_info.USERID,
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

      this.innerVisible = false;
    },
  
    /* 其他方法*/
    // 初始化加载所有数据
    load_all_data(){
      // 获取该护工所有的订单信息
      Axios.all([this.get_all_orders(),this.get_care_type(),this.getNurseInfo()])
      .then(Axios.spread((resp1, resp2,resp3) => {	// spread是将各请求结果拆分返回
        // 获得护工信息
        this.my_info = resp3.data.message[0];
        
        this.all_care_orders = resp1.data.message;
        this.current_care_orders = this.all_care_orders;
        this.current_care_orders_copy = this.current_care_orders;
        
        // 加载服务类型数据
        let tmp_type = resp2.data.message;
        // 根据typeID进行排序
        tmp_type.sort(function(a,b){
          return a.TYPEID - b.TYPEID;
        });
        tmp_type.forEach(element => {
          let obj = {
            value:element.TYPEID,
            text:element.NAME
          }
          this.care_types.push(obj);
        });
        // 数据已经准备就绪 可以显示表格
        this.loading = false;
      }))
      .catch(err => {
        console.log(err);
        if (err.message == "Request failed with status code 403" && !this.AccessTokenFailed) {
          AccessTokenFailed();
          this.AccessTokenFailed = true;
        }
      });
    },
    // 刷新页面 即从后端重新获取订单信息
    refresh_data(){
      this.get_all_orders()
      .then((res) => {
        console.log(res);
        let tmp_all_data = res.data.message;
        if(this.all_care_orders.toString() !== tmp_all_data.toString()){
          console.log("数据有更新");
          this.all_care_orders = tmp_all_data;
          this.current_care_orders = this.all_care_orders;
          this.current_care_orders_copy = this.current_care_orders;
        }
      })
      .catch(err => {
        console.log(err);
        if (err.message == "Request failed with status code 403" && !this.AccessTokenFailed) {
          AccessTokenFailed();
          this.AccessTokenFailed = true;
        }
      })
    },
    getToady(){
        const nowDate = new Date();
        const date = {
            year: nowDate.getFullYear(),
            month: nowDate.getMonth() + 1,
            date: nowDate.getDate(),
        }
        const newmonth = date.month>10?date.month:'0'+date.month
        const day = date.date>10?date.date:'0'+date.date
        this.Today = date.year + '-' + newmonth + '-' + day
        console.log(this.Today);
    },
  },
  mounted: function () {
    this.my_uid = this.$cookies.get("USERID");
    this.access_token = this.$cookies.get("token");
    this.getToady();
    this.load_all_data();
    // // 每隔5秒向后端重新请求一遍数据（轮询）
    // window.setInterval(() => {
    //   setTimeout(() => {
    //     this.refresh_data();
    //   }, 0)
    // }, 5000);
  },
};
</script>

<!-- lang选择less语法，scoped限制该样式只在本文件使用，不影响其他组件 -->
<style lang="less" scoped>

.views {
  .card-item{
    background:white;
    border-radius: 10px;
    margin-bottom: 15px;
    padding: 5px;
    
    .condition-title {
      color: #2b3b4e;
      font-size: 18px;
      font-weight: 700;
      line-height: 24px;
      margin-bottom: 15px;
      .title-button /deep/ {
        min-width: 80px;
      }
    }
    .condition-body{
      display: flex;
      margin-left: 60px;
      align-items: center;
      .condition-form{
        margin-bottom:0px;
        .conditon-form-item1 
        {
          margin-bottom:0px;
          /deep/ #input1{
            width: 300px;
          }
          .inline-input{
            width: 250px;
          }
        }
        
        .conditon-form-item2{
          margin-bottom: 0px;
          margin-right: 100px;
          margin-left: 100px;
        }
      }

    }
    .form-page {
      margin-top: 20px;
      display: flex;
      flex-direction: column;
      justify-content: center;
      align-items: center;
    }
  }
  .CareServices-Row {
    display: flex;
    flex-direction: row;
    justify-content: center;
    flex-wrap: wrap;
    .service-card {
      margin-right: 10px;
      margin-bottom: 10px;
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
  
  .serviceDialog {
    width: 100%;
    display: flex;
    flex-direction: column;

    //弹窗内容
    .dialog-content {
      display: flex;
      flex-direction: column;

      .dialog-img-box {
        display: flex;
        flex-direction: column;
      }
      .dialog-info {
        font-weight: bold;
        color: black;
        font-size: 30px;
      }
    }
  }
}

/deep/ .el-autocomplete-suggestion {
  width: auto!important;
  
}
</style>>
