<template>
  <div class="views">
    <!-- tab -->
    <el-tabs v-model="activeName" type="card" class="tabs" @tab-click="HandleTabsClick">
      <el-tab-pane label="全部订单" name="all"></el-tab-pane>
      <el-tab-pane label="已申请" name="requested"></el-tab-pane>
      <el-tab-pane label="正在进行" name="progressing"></el-tab-pane>
      <el-tab-pane label="已完成" name="finished"></el-tab-pane>
      
    </el-tabs>
    
    <!-- 订单table -->

    <el-table
      ref="singleTable"
      :data="page_care_orders"
      highlight-current-row
      style="width: 100%"
      @sort-change='handleSortChange'
      @filter-change="handleFilterChange"
      v-loading="loading"
      >
    
      <!-- 索引列 -->
      <!-- 注意该索引和订单信息中的索引并不相同 -->
      <!-- 在进行排序操作后是通过订单信息中的索引属性来获取对应信息 -->
      <!-- 而该索引列只是为了方便用户查看 -->
      <el-table-column
      type="index"
      :index="indexMethod">
      </el-table-column>

      <!-- 订单编号列 -->
      <el-table-column 
      prop="RECORDID" 
      label="订单编号" 
      width="250"
      sortable="custom">
      </el-table-column>

      <!-- 姓名列  -->
      <el-table-column
        prop="NAME"
        label="申请人"
        width="180">
      </el-table-column>

      <!-- 服务名称列 具有筛选功能-->
      <el-table-column
        prop="TYPEID"
        label="服务类型"
        width="180"
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
        prop="ORDER_TIME"
        label="护理时间"
        width="250"
        sortable="custom" 
      ></el-table-column>

      <!-- 订单状态列 -->
      <el-table-column
        prop="STATUS"
        label="订单状态"
        width="150"
      >
        <template slot-scope="scope">
          <el-tag
          :type="getTagColor(scope.row.STATUS)"
          disable-transitions>
            {{ getOrderStatusName(scope.row.STATUS) }}
          </el-tag>
        </template>
      </el-table-column>
      <!-- 查看详情列 -->
      <el-table-column align="center"  fixed="right" class-name="el_table_column">
        <template slot-scope="scope">
          <div>
            <el-badge
              value="new"
              style="margin-top:10px;margin-right: 40px;"
              :hidden="scope.row.UPDATED !== '2'">
              <el-button
                @click="ViewDetailButtonClick(scope.row)"
                type="primary"
                size="mini">             
                查看详情
              </el-button>

            </el-badge>  
          </div>
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
            <template v-if="elder_info.HEIGHT!==null">
              {{ elder_info.HEIGHT }} cm
            </template>
          </el-descriptions-item>
          <el-descriptions-item>
            <template slot="label">
              <i class="el-icon-s-marketing"></i>
              体重
            </template>
            <template v-if="elder_info.HEIGHT!==null">
              {{ elder_info.WEIGHT }} kg
            </template>
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
        <!-- :column="2" 一行有2个 -->
        <el-descriptions class="margin-top" :column="2" border>
          <el-descriptions-item>
            <template slot="label">
              <i class="el-icon-user"></i>
              用户姓名
            </template>
            <el-button type="text" @click="ClickElderDetailButton()">{{details.name}}</el-button>
          </el-descriptions-item>

          <el-descriptions-item>
            <template slot="label">
              <i class="el-icon-mobile-phone"></i>
              手机号
            </template>
            {{ details.phone }}
          </el-descriptions-item>
        </el-descriptions>

        <el-descriptions
          class="margin-top"
          direction="vertical"
          :column="1"
          border
        >
          <el-descriptions-item>
            <template slot="label">
              <i class="el-icon-location-outline"></i>
              服务地址
            </template>
            {{ details.address }}
          </el-descriptions-item>
        </el-descriptions>
        <!-- 通用的服务介绍 -->
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

          <el-descriptions-item>
            <template slot="label">
              <i class="el-icon-money"></i>
              订单价格
            </template>
            {{ details.price }}
          </el-descriptions-item>

          <el-descriptions-item>
            <template slot="label">
              <i class="el-icon-bell"></i>
              订单状态
            </template>
            <el-tag  size="small" :type="getTagColor(details.stateID)">{{ details.state }}</el-tag>
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
        <!-- 通用的服务介绍 -->
        <el-descriptions class="margin-top" :column="2" border>

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
      <!-- 底部的slot插槽 -->
      <span slot="footer" class="dialogFooter">
        <el-button 
        type="danger" 
        v-if=" details.state === '已申请' || details.state === '申请失败'" 
        @click="CLickCancelRequestButton()" 
        :disabled="details.state === '申请失败'"
          >取消申请</el-button
        >
        <el-button @click="ClickCloseButton()">关闭</el-button>
      </span>
    </el-dialog>
  </div>
</template>

<script>
import { AccessTokenFailed } from "../../../api/data.js"
import axios from "../../../api/axios";
// 引入原生axios 实现并行请求处理
import Axios from "axios";
export default {
  name: "TakeOrderMine",
  components: {},
  data() {
    return {
      innerVisible: false,
      elder_info:null,
      elder_history_disease:null,
      access_token:null,
      // 护工的ID
      my_uid:"",
      // 当前激活的tab名称
      activeName:'all',
      // 判断数据是否准备就绪（使得从后台获取好数据后再渲染DOM）
      loading:true,
      //显示弹窗
      dialogVisible: false, 
      // 护理类型
      care_types:[],
      // 当前筛选条件下的护理记录
      current_care_orders:[],
      // 所有护理记录
      all_care_orders:[],
      // 已申请护理记录
      requested_orders:[],
      // 已经被拒绝记录
      refused_orders:[],
      // 正在进行护理记录
      progressing_orders:[],
      // 已完成护理记录
      finished_orders:[],
      // 查看详情的表单信息
      details:{
        record_id:null,
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
      // 当前页面
      current_page:1,
      // 页面大小
      page_size:10,
      put_data:{},

      followed:false,
      content:"关注",
      bg_color:"#8080ff",
      ft_color:"#ffffff",
      tableData2:[],
    };
  },
  computed:{
    // 根据当前页面号生成对应页面中的数据
    // 当current_page和current_care_orders改变时都会自动更新
    page_care_orders: function() {
      return this.current_care_orders.slice(this.page_size*(this.current_page - 1),this.page_size*(this.current_page));
    },
    // 总的条目数
    total_items: function() {
      return this.current_care_orders.length;
    }
  },
  methods: {
    /* 后端请求相关方法 */
    // 从后端获取已申请订单
    get_requested_records() {
      return axios.request({
          headers: {
              TokenValue: this.access_token,
          },
          url:"/API/ServiceRecord",
          method:'get',
          params: {
            // 订单状态为0
            status: 0,
            // 该护工在等待队列中
            in_wait_nurse_id:this.my_uid
          },
      })
    },
    // 从后端获取正在进行中订单
    get_progressing_records(){
      return axios.request({
          headers: {
              TokenValue: this.access_token,
          },
          url:"/API/ServiceRecord",
          method:'get',
          params: {
              nursingworkerId:this.my_uid,
              status:1,
          },
      })
    },
    // 从后端获取已完成订单
    get_finished_records(){
      return axios.request({
          headers: {
              TokenValue: this.access_token,
          },
          url:"/API/ServiceRecord",
          method:'get',
          params: {
              nursingworkerId:this.my_uid,
              status:2,
          },
      })
    },
    // 从后端获取被拒绝的订单（需要在根据nursingworkingID进行筛选）
    get_refused_records(){
      return axios.request({
          headers: {
              TokenValue: this.access_token,
          },
          url:"/API/ServiceRecord",
          method:'get',
          params: {
              // 订单状态为1
              status: 1,
              // 该护工在等待队列中
              in_wait_nurse_id:this.my_uid
          },
      })
    },
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
    // 通过老人的ID从后端获取所有老人信息
    getElderInfoByID(id){
      return axios.request({
          headers: {
              TokenValue: this.access_token,
          },
          url: "/API/Elder" + "/" + id,
          method: 'get',
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
    // 修改update属性 实现取消护工的消息提醒
    put_update(record_ID){
      return axios.request({
          headers: {
              TokenValue: this.access_token,
          },
          url:"/API/ServiceRecord/" + record_ID,
          method:'put',
          data:this.put_data
      })
    },

    /* 顶端tab相关方法 */
    //点击切换tab执行的操作
    HandleTabsClick(tab) {
      // 更改筛选条件
      let state = tab.name;
      if(state === "progressing"){
        this.current_care_orders = this.progressing_orders; 
      }
      else if(state === "requested"){
        this.current_care_orders = this.requested_orders; 
      }
      else if(state === "finished"){
        this.current_care_orders = this.finished_orders; 
      }
      else if(state === "all"){
        this.current_care_orders = this.all_care_orders; 
      }
    },


    /* 表格主体相关方法 */
    // 生成表格中每行的索引的方法
    indexMethod(index) {
      return index + (this.current_page - 1)*this.page_size + 1;
    },
    // 根据订单状态返回Tag标签的颜色
    getTagColor(state){
      if(state === "0"){
        return "success";
      }
      else if(state === "1"){
        return "primary";
      }
      else if(state === "2"){
        return "info";
      }
      else if(state === "3"){
        return "danger";
      }
      else{
        console.log(state);
      }
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
      else if(status === "3"){
        return "申请失败";
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

    //查看详情 按钮
    ViewDetailButtonClick(row) {
      // 修改detail对象的内容
      console.log(row);
      this.details.record_id = row.RECORDID;
      this.details.address = row.ADDRESS;
      this.details.type = this.getCareTypeName(row.TYPEID);
      this.details.order_time = row.ORDER_TIME;
      this.details.price = row.PRICE;
      this.details.stateID = row.STATUS;
      this.details.state = this.getOrderStatusName(row.STATUS);
      this.details.service_items = JSON.parse(row.service_items);
      this.details.situation = this.getSituationName(row.SITUATION);
      this.details.need = this.getNeedName(row.NEED);
      this.details.updated = row.UPDATED;
      this.getElderInfoByID(row.ELDERID)
      .then((res)=>{
        // 修改detail对象的内容
        this.elder_info = res.data.message[0];
        this.details.name = this.elder_info.NAME;
        this.details.phone = this.elder_info.PHONE;
        // 使弹窗可见
        this.dialogVisible = true;
        console.log(this.details);
        if(this.details.updated === '2'){
          this.put_data = {
            typeid: row.TYPEID,
            start_time: row.START_TIME,
            order_time: row.ORDER_TIME,
            address: row.ADDRESS,
            status: row.STATUS,
            price: row.PRICE,
            evaluationid: row.EVALUATIONID,
            elderid: row.ELDERID,
            nursingworkerid: row.NURSINGWORKERID,
            situation: row.SITUATION,
            need: row.NEED,
          }
        }
      })
      .catch((err)=>{
        console.log(err);
        if (err.message == "Request failed with status code 403" && !this.AccessTokenFailed) {
          AccessTokenFailed();
          this.AccessTokenFailed = true;
        }
      });  
    },
    // 对表格进行排序（对象是当前页面中数据）排序项为时间和订单编号
    handleSortChange(info){
      console.log("排序时传入的列数据info:",info);
      if(info.prop === "ORDER_TIME"){  // 对时间进行排序
        if(info.order === "ascending"){  //升序排列
          this.current_care_orders.sort(function(a,b){
            let flag = a.ORDER_TIME > b.ORDER_TIME ? 0 : -1;
            return flag;
          });
        }
        else{
          this.current_care_orders.sort(function(a,b){
            let flag = b.ORDER_TIME > a.ORDER_TIME ? 0 : -1;
            return flag;
          });
        }
      }
      else if(info.prop === "RECORDID"){  //对订单编号进行排序
        if(info.order === "ascending"){  //升序排列
          this.current_care_orders.sort(function(a,b){
            let flag = a.RECORDID > b.RECORDID ? 0 : -1;
            return flag;
          });
        }
        else{
          this.current_care_orders.sort(function(a,b){
            let flag = b.RECORDID > a.RECORDID ? 0 : -1;
            return flag;
          });
        }
      }
    },
    // 对表格进行筛选（对象是当前页面中数据）筛选项为护理类型
    handleFilterChange(info){
      // 要筛选的服务类型
      let tmp_type = info.care_type;
      let state = this.activeName;
      let wait_filter_data;
      if(state === "progressing"){
        wait_filter_data = this.progressing_orders; 
      }
      else if(state === "requested"){
        wait_filter_data = this.requested_orders; 
      }
      else if(state === "finished"){
        wait_filter_data = this.finished_orders; 
      }
      else if(state === "all"){
        wait_filter_data = this.all_care_orders; 
      }
      if(tmp_type.length === 0){
        // 需要全部数据
        this.current_care_orders = wait_filter_data;
      }
      else{
        this.current_care_orders = wait_filter_data.filter(function(a){
          return a.TYPEID === tmp_type[0];
        });
      }
    },

    /* 分页栏相关方法 */
    // 在页面改变时进行的操作
    HandlePageChange(currentPage) {
      // 更改当前页面
      this.current_page = currentPage;
    },
    
    /* 弹窗对话框相关方法 */
    // 点击取消申请按钮后的操作
    CLickCancelRequestButton(){
      // 取消申请
      //弹窗设置不可见
      this.dialogVisible = false;
      // 弹出确认信息
      this.$msgbox
      .confirm("是否取消接单申请", "提示", {
        confirmButtonText: "确定操作",
        cancelButtonText: "撤销操作",
        type: "info",
        center: true,
      }).then(() => {
        // 向后端传输取消接单信息
        axios
        .request({
          headers: {
              TokenValue: this.access_token,
          },
          url: "/API/servicerecord/nurse",
          method: "delete",
          data:{
            recordid:this.details.record_id,
            nurseid:this.my_uid
          }
        })
        .then((res) => {
          console.log(res);
          this.$message({
            type: 'success',
            message: '取消申请成功，注意之后请确认好时间后再提交接单申请!'
          });
          // 刷新页面
          this.refresh_data();
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
          message: '操作已撤销!'
        });          
      });
    },
    // 点击关闭按钮后的操作
    ClickCloseButton(){
      // 如果用户选择了该护工接单
      if(this.details.updated === '2' && this.details.state === "正在进行"){
        // 更改updated
        this.put_update(this.details.record_id)
        .then((res)=>{
          console.log(res);
          this.refresh_data();
        })
        .catch((err)=>{
          console.log(err);
          if (err.message == "Request failed with status code 403" && !this.AccessTokenFailed) {
            AccessTokenFailed();
            this.AccessTokenFailed = true;
          }
        });  
      }
      // 关闭弹窗
      this.dialogVisible = false
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
          if (err.message == "Request failed with status code 403" && !this.AccessTokenFailed) {
            AccessTokenFailed();
            this.AccessTokenFailed = true;
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
          if (err.message == "Request failed with status code 403" && !this.AccessTokenFailed) {
            AccessTokenFailed();
            this.AccessTokenFailed = true;
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
    
    /* 其他方法 */
    // 初始化加载所有数据
    load_all_data(){
      // 获取该护工所有的订单信息
      Axios.all([this.get_requested_records(), this.get_progressing_records(),this.get_finished_records(),this.get_care_type(),this.get_refused_records()])
      .then(Axios.spread((resp1, resp2, resp3,resp4,resp5) => {	// spread是将各请求结果拆分返回
        // 四个个请求现在都执行完成
        // 加载每种类型的订单数据
        this.requested_orders = resp1.data.message;
        this.progressing_orders = resp2.data.message;
        this.finished_orders = resp3.data.message;
        this.refused_orders = resp5.data.message.map(e => {
          if(e.NURSINGWORKERID !== this.my_uid){
            // 申请失败
            e.STATUS = "3";
            return e;
          }
        });
        this.requested_orders.push(...this.refused_orders);

        this.all_care_orders.push(...this.requested_orders);
        this.all_care_orders.push(...this.progressing_orders);
        this.all_care_orders.push(...this.finished_orders);

        // 加载服务类型数据
        let tmp_type = resp4.data.message;
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

        // 判断是否从首页跳转过来的，如果是则默认展示正在进行中的订单
        if(this.$route.params.fromHomePage){
          this.activeName = "progressing";
          this.current_care_orders = this.progressing_orders;
        }
        else{
          this.activeName = "all";
          this.current_care_orders = this.all_care_orders;
        }

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
      Axios.all([this.get_requested_records(), this.get_progressing_records(),this.get_finished_records(),this.get_refused_records()])
      .then(Axios.spread((resp1, resp2, resp3, resp4) => {	// spread是将各请求结果拆分返回
        // 加载每种类型的订单数据
        let changed = false;
        if(this.requested_orders.toString() !== resp1.data.message.toString()){
          changed = true;
          this.requested_orders = resp1.data.message;
        }
        if(this.progressing_orders.toString() !== resp2.data.message.toString()){
          changed = true;
          this.progressing_orders = resp2.data.message;
        }
        if(this.finished_orders.toString() !== resp3.data.message.toString()){
          changed = true;
          this.finished_orders = resp3.data.message;
        }
        let refused = resp4.data.message.map(e => {
          if(e.NURSINGWORKERID !== this.my_uid){
            // 申请失败
            e.STATUS = "3";
            return e;
          }
        });
        if(this.refused_orders.toString() !== refused.toString){
          changed = true;
          this.refused_orders = refused;
        }
        if(changed){
          this.all_care_orders.length = 0;
          this.requested_orders.push(...this.refused_orders);
          this.all_care_orders.push(...this.requested_orders);
          this.all_care_orders.push(...this.progressing_orders);
          this.all_care_orders.push(...this.finished_orders);
          let state = this.activeName;
          if(state === "progressing"){
            this.current_care_orders = this.progressing_orders; 
          }
          else if(state === "requested"){
            this.current_care_orders = this.requested_orders; 
          }
          else if(state === "finished"){
            this.current_care_orders = this.finished_orders; 
          }
          else if(state === "all"){
            this.current_care_orders = this.all_care_orders; 
          }
        }
      }))
      .catch(err => {
        console.log(err);
        if (err.message == "Request failed with status code 403" && !this.AccessTokenFailed) {
          AccessTokenFailed();
          this.AccessTokenFailed = true;
        }
      });
    },

    
  },
  mounted: function () {
    this.my_uid = this.$cookies.get("USERID");
    this.access_token = this.$cookies.get("token");
    this.load_all_data();

    // // 每隔5秒向后端重新请求一遍数据（轮询）
    // window.setInterval(() => {
    //   setTimeout(() => {
    //     this.refresh_data();
    //   }, 0)
    // }, 5000);

  }
  }
</script>

//  lang选择less语法，scoped限制该样式只在本文件使用，不影响其他组件
<style lang="less" scoped>
.views {
  display: flex;
  flex-direction: column;
  justify-content: center;
  .form-page {
    margin-top: 20px;
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
  }
  .el_table_column{
    display: flex;
    flex-direction: row;
    justify-content: center;
    .badge_box{
      padding: 20px;
      .badge-badge{
        margin-top: 10px;
        margin-right: 40px;
      }
    }
  }
  
  //表格
  .order_table {
    width: 100%;

    //三种状态的颜色
    // /deep/.placed-row {
    //   background-color: #aed581;
    // }
    // /deep/.underway-row {
    //   background-color: #b3e5fc;
    // }
    // /deep/.completed-row {
    //   background: #fcf5e6;
    // }
  }

  // 弹窗对话框
  .serviceDialog {
    //主要内容
    .dialog-content {
      //确认的nurse box
      .access_hint{
        font-size:30px;
        font-weight: bold;
      }
      //接单的若干nurses box
      .accessible_nurses_box {
        display: flex;
        flex-direction: row;
        justify-content: center;
        flex-wrap: wrap;
        border: 3px #EBEEF5 solid;
        .nurse_card_col {
          .nurse_card {
            margin-right: 30px;
            margin-left: 30px;
            margin-top:20px;
            margin-bottom:20px;

            .card-content {
              .card-img {
                width: 100%;
              }
              .card-info {
                padding-left: 14px;
              }
            }
          }
        }
      }
    }
  }
}
</style>>
