
/*
功能：Client 护理模块 “我的护理”页面；展示Client的所有 护理服务订单
creator、editor:yy 
 */
<template>
  <div class="views">
    <!-- tab -->
    <el-tabs type="card" class="tabs" @tab-click="HandleTabsClick">
      <el-tab-pane label="全部订单"></el-tab-pane>

      <el-tab-pane label="正在进行"></el-tab-pane>

      <el-tab-pane label="已发布"></el-tab-pane>

      <el-tab-pane label="已完成"></el-tab-pane>
    </el-tabs>

    <!-- 订单table -->
    <el-table
      v-loading="loading_simpleList"
      :key="orders_page"
      :data="orders_page"
      class="order_table"
      stripe
      @sort-change="tableSortChange"
      highlight-current-row
      row-class-name="order_table_col"
    >
      <!-- 可展开列表,查看对应的服务项目 -->
      <el-table-column type="expand">
        <template slot-scope="props">
          <el-form label-position="left" inline class="demo-table-expand">
            <el-form-item label=" ">
              <el-table
                :data="props.row.service_items"
                stripe
                style="width: 100%"
                border
              >
                <el-table-column prop="name" label="服务项目" width="180">
                </el-table-column>
                <el-table-column prop="per_price" label="单价" width="180">
                </el-table-column>
                <el-table-column prop="num" label="个数"> </el-table-column>
              </el-table>
            </el-form-item>
          </el-form>
        </template>
      </el-table-column>

      <el-table-column
        column-key="categoryName"
        prop="categoryName"
        label="服务类型"
        width="180"
        :filters="[
          { text: '老年保健', value: '老年保健' },
          { text: '失能介护', value: '失能介护' },
          { text: '家庭保洁', value: '家庭保洁' },
          { text: '家电清洗', value: '家电清洗' },
          { text: '生活照护', value: '生活照护' },
        ]"
        :filter-method="filterHandler"
      >
      </el-table-column>

      <el-table-column label="服务状态" width="120">
        <template slot-scope="scope">
          <el-tag :type="StateTagName(scope.row.state_cn)">{{
            scope.row.state_cn
          }}</el-tag>
        </template>
      </el-table-column>

      <el-table-column
        prop="place_time"
        label="下单时间"
        width="180"
        sortable="custom"
      ></el-table-column>

      <el-table-column
        prop="work_time"
        label="预约时间"
        width="140"
        sortable="custom"
      ></el-table-column>

      <el-table-column
        prop="total_price"
        label="价格"
        width="140"
        sortable="custom"
      ></el-table-column>

      <el-table-column
        prop="nurse_name"
        label="负责护工"
        width="140"
        sortable="custom"
      ></el-table-column>

      <el-table-column
        align="center"
        class-name="el_table_column"
        fixed="right"
      >
        <template slot-scope="scope">
          <div class="badge_box">
            <el-badge
              value="new"
              :class="{
                'badge-badge': true,
              }"
              :hidden="!scope.row.updated"
            >
              <el-button
                @click.native.prevent="viewDetailButtonClick(scope.row)"
                type="primary"
                size="mini"
                class="checkDetail"
              >
                查看详情
              </el-button>
            </el-badge>
          </div>

          <div class="other_buttons">
            <el-button
              @click.native.prevent="TableCancel(scope.row)"
              type="danger"
              size="mini"
              v-if="scope.row.state == '0'"
              class="button"
            >
              取消订单
            </el-button>
            <el-button
              @click.native.prevent="TableTernimate(scope.row)"
              type="danger"
              size="mini"
              v-if="scope.row.state == '1'"
              class="button"
            >
              中断服务
            </el-button>
            <el-button
              @click.native.prevent="TableConfirmComplete(scope.row)"
              type="success"
              size="mini"
              v-if="scope.row.state == '1'"
              class="button"
            >
              确认服务完成
            </el-button>

            <ReportBox
              :orderID="scope.row.id"
              :passiveID="scope.row.nurse_id"
              :passiveName="scope.row.nurse_name"
              :activeID="userID"
              activeName="匿名"
              selection="client_mini"
              class="button report"
              v-if="scope.row.state == '2'"
            ></ReportBox>
          </div>
        </template>
      </el-table-column>
    </el-table>

    <!-- 分页 -->
    <div class="pagination_box">
      <el-pagination
        class="pagination"
        background
        layout="prev, pager, next"
        :page-size="page_size"
        :total="total_num"
        :current-page="current_page"
        @current-change="HandlePageChange"
      >
      </el-pagination>
    </div>

    <!-- 弹窗对话框 -->
    <div class="loading_mask">
      <el-dialog
        class="serviceDialog"
        title="服务详情"
        v-if="dialogVisible"
        :visible.sync="dialogVisible"
      >
        <!-- 底部的slot插槽  按钮区 -->
        <span
          slot="footer"
          class="dialogFooter"
          v-loading="loading_dialogContent"
        >
          <el-button
            type="success"
            @click="ConfirmServiceCompleteButtonClick"
            v-if="order_detail.order_state == '正在进行'"
            class="button complete"
            >确认服务完成</el-button
          >
          <el-button
            type="danger"
            @click="TernimateButtonClick"
            v-if="order_detail.order_state == '正在进行'"
            class="button terminate"
            >中断服务</el-button
          >

          <ReportBox
            :orderID="order_detail.client_id"
            :passiveID="order_detail.nurse_intro.id"
            :passiveName="order_detail.nurse_intro.name"
            :activeID="userID"
            activeName="匿名"
            selection="client_normal"
            class="button report"
            v-if="order_detail.order_state == '已完成'"
          ></ReportBox>
          <el-button
            type="primary"
            v-if="order_detail.order_state == '已发布'"
            @click="ConfirmNurseOrderButtonClick"
            class="button confirm"
            >确认订单</el-button
          >

          <el-button
            type="danger"
            v-if="order_detail.order_state == '已发布'"
            @click="CancelNurseOrderButtonClick"
            class="button cancel"
            >取消订单</el-button
          >

          <el-button @click="dialogVisible = false">关闭</el-button>
        </span>
        <div class="dialog-content" v-loading="loading_dialogContent">
          <!-- 已完成：评价表格 -->
          <div class="comment_box" v-if="order_detail.order_state == '已完成'">
            <CommentBox
              :comment_data="comment_data"
              v-on:CommentBoxValueToParent="GetCommentBoxValue"
              v-if="comment_data.CanUseComment == true"
            ></CommentBox>

            <div
              class="show_evaluation"
              v-if="comment_data.CanUseComment == false && evaluation_data"
            >
              <el-col :span="15" class="left_text_box">
                <el-rate
                  v-model="evaluation_data.grade"
                  disabled
                  text-color="#ff9900"
                  class="star"
                  show-score
                >
                </el-rate>
                <div class="text">
                  {{ evaluation_data.message }}
                </div>
              </el-col>
              <el-col :span="10" class="right_info_box">
                <div class="time">{{ evaluation_data.time }}</div>
              </el-col>
            </div>
          </div>

          <!-- :column="2" 一行有2个 -->
          <el-descriptions class="margin-top" :column="2" border>
            <el-descriptions-item>
              <template slot="label">
                <i class="el-icon-user"></i>
                用户名
              </template>
              {{ order_detail.client_name }}
            </el-descriptions-item>

            <el-descriptions-item>
              <template slot="label">
                <i class="el-icon-mobile-phone"></i>
                手机号
              </template>
              {{ order_detail.client_phone }}
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
              {{ order_detail.order_address }}
            </el-descriptions-item>

            <el-descriptions-item>
              <template slot="label">
                <i class="el-icon-office-building"></i>
                联系地址
              </template>
              {{ order_detail.contact_address }}
            </el-descriptions-item>
          </el-descriptions>

          <!-- 通用的服务介绍 -->
          <el-descriptions class="margin-top" :column="2" border>
            <el-descriptions-item>
              <template slot="label">
                <i class="el-icon-reading"></i>
                服务类型
              </template>
              {{ order_detail.category_name }}
            </el-descriptions-item>

            <el-descriptions-item>
              <template slot="label">
                <i class="el-icon-time"></i>
                服务时间
              </template>
              {{ order_detail.work_time }}
            </el-descriptions-item>

            <el-descriptions-item>
              <template slot="label">
                <i class="el-icon-money"></i>
                订单价格
              </template>
              {{ order_detail.total_price }}
            </el-descriptions-item>

            <el-descriptions-item>
              <template slot="label">
                <i class="el-icon-view"></i>
                订单状态
              </template>
              <el-tag type="success">{{ order_detail.order_state }}</el-tag>
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
                <i class="el-icon-document"></i>
                服务内容
              </template>

              <el-table
                :data="order_detail.service_items"
                stripe
                style="width: 100%"
              >
                <el-table-column prop="name" label="服务项目" width="180">
                </el-table-column>
                <el-table-column prop="per_price" label="单价" width="180">
                </el-table-column>
                <el-table-column prop="num" label="个数"> </el-table-column>
              </el-table>
            </el-descriptions-item>
          </el-descriptions>

          <el-descriptions class="margin-top" :column="2" border>
            <el-descriptions-item>
              <template slot="label">
                <i class="el-icon-location-outline"></i>
                自理能力
              </template>
              <el-tag>{{ order_detail.self_care_situation }}</el-tag>
            </el-descriptions-item>

            <el-descriptions-item>
              <template slot="label">
                <i class="el-icon-office-building"></i>
                护理需求方向
              </template>
              <el-tag> {{ order_detail.careDirect }}</el-tag>
            </el-descriptions-item>
          </el-descriptions>

          <el-divider></el-divider>

          <!-- 正在进行、已完成：确认接单的护工信息-->
          <div
            v-if="
              order_detail.order_state == '正在进行' ||
              order_detail.order_state == '已完成'
            "
            class="confirmed_nurse_box"
          >
            <el-descriptions
              class="margin-top"
              direction="vertical"
              :column="2"
              :colon="false"
              border
            >
              <el-descriptions-item>
                <template slot="label">
                  <i class="el-icon-user"></i>
                  护工头像
                </template>
                <img
                  :src="nurse_intro.avatar"
                  style="width: 400px; height: 400px"
                />
              </el-descriptions-item>

              <!-- 嵌套 -->
              <el-descriptions-item>
                <template slot="label">
                  <i class="el-icon-user"></i>
                  护工信息
                </template>
                <el-descriptions class="margin-top" :column="1">
                  <el-descriptions-item label="护工名字">
                    {{ nurse_intro.name }}
                  </el-descriptions-item>
                  <el-descriptions-item label="年龄">
                    {{ nurse_intro.age }}
                  </el-descriptions-item>
                  <el-descriptions-item label="性别">
                    {{ nurse_intro.gender }}
                  </el-descriptions-item>
                </el-descriptions>
                <!-- button -->
                <el-button
                  style="margin-top: 20px"
                  type="info"
                  icon="el-icon-tickets"
                  @click="CheckNurseDetail(nurse_intro)"
                  >查看详细信息</el-button
                >
              </el-descriptions-item>
            </el-descriptions>
          </div>

          <!--已发布：接单护工的列表  -->

          <div
            v-if="order_detail.order_state == '已发布' && hasWaitingNurse"
            class="access_hint"
          >
            现在请求接单的护工如下<br />
            请选择一位作为您的受理护工
          </div>

          <div
            v-if="order_detail.order_state == '已发布' && !hasWaitingNurse"
            class="access_hint"
          >
            暂无护工接单<br />
            请耐心等待(*´∀`)
          </div>

          <div
            v-if="order_detail.order_state == '已发布'"
            class="accessible_nurses_box"
          >
            <!-- 卡片col -->
            <el-col
              :span="7"
              v-for="(nurse_simple, nurse_array_index) in accessible_nurses"
              :key="nurse_simple.id"
              class="nurse_card_col"
            >
              <!-- 卡片 -->
              <el-card
                shadow="hover"
                class="nurse_card"
                :body-style="{ padding: '0px' }"
              >
                <!-- 卡片内容 -->
                <div class="card-content">
                  <div class="card-img">
                    <img class="waiting_img" :src="nurse_simple.avatar" />
                  </div>

                  <div class="card-info">
                    <p>
                      年龄:<span>{{ nurse_simple.age }}</span>
                    </p>
                    <p>
                      性别:<span>{{ nurse_simple.gender }}</span>
                    </p>
                    <p>
                      护工姓名:<span>{{ nurse_simple.name }}</span>
                    </p>
                    <!-- 卡片底下的按钮盒子 -->
                    <p class="info-buttons-box">
                      <!-- 选中按钮 -->
                      <el-button
                        icon="el-icon-check"
                        circle
                        :class="{
                          'card-active': nurse_simple.isChecked,
                        }"
                        @click.native="ChooseNurseClick(nurse_array_index)"
                      ></el-button>

                      <!-- 查看，跳转到护工的个人中心 -->
                      <el-button
                        icon="el-icon-search"
                        circle
                        @click.native="CheckNurseDetail(nurse_simple)"
                      ></el-button>
                    </p>
                  </div>
                </div>
              </el-card>
            </el-col>
          </div>

          <el-descriptions></el-descriptions>
        </div>
      </el-dialog>
    </div>
  </div>
</template>

<script>
import CommentBox from "../../../components/CommentBox.vue";
import axios from "../../../api/axios";
import ReportBox from "../../../components/ReportBox.vue";
import { AccessTokenFailed } from "../../../api/data.js";
export default {
  name: "CareServiceMine",
  components: { CommentBox, ReportBox },
  data() {
    return {
      AccessTokenFailed: false,
      access_token: null,
      headers: {},
      loading_simpleList: true, //加载
      loading_dialogContent: true,
      loading_detail: true,
      userID: "", //static用户ID
      order_detail: [], //某个订单的详情
      dialogVisible: false, //显示弹窗
      selected_order_id: "-1145", //选中的订单编号
      star_comment_value: "", //评价的全数据

      //category的筛选器
      category_filter: [
        { text: "服务类型1", value: "服务类型1" },
        { text: "服务类型2", value: "服务类型2" },
        { text: "服务类型3", value: "服务类型3" },
        { text: "服务类型4", value: "服务类型4" },
      ],

      //护工的简介
      nurse_intro: {},
      //接单的护工列表，等待确认
      accessible_nurses: [],
      hasWaitingNurse: false,
      // 使用评价组件，传入配置数据
      comment_data: {
        all_star_text: ["服务评分"],
        anonym: false,
        head_text: "订单评价",
        CanUseComment: true, //评价组件是否可用
      },
      //若已经评价过，请求之前评价的信息，展示
      evaluation_data: {},
      //一开始就把所有数据请求到，
      //全部的订单数据
      orders_all: [],
      stateFilter_orders_all: [], //state过滤过的
      //分页的订单数据
      orders_page: [],

      page_size: 10, //分页size,恒定不变
      total_num: -1, //总的数据条数

      total_page: -1, //这种分页请求下，一共有多少页面
      current_page: 1, //当前页数,不会随着pagi自动改变

      sort_key: "total_price", //排序关键词
      sort_order: "descend", //排序顺序,descend,ascend
      sort_state: "all", //订单state分类,all,running,posted,done
    };
  },
  methods: {
    //服务类型表头的筛选的函数
    //
    //

    //筛选器
    filterHandler(value, row, column) {
      const property = column["property"];
      return row[property] === value;
    },
    //打印分页筛选的相关参数
    PrintPageParams() {
      console.log("_______打印分页参数_______");
      console.log("每页size：", this.page_size);
      console.log("总的数据条数：", this.total_num);
      console.log("一共有多少页面：", this.total_page);
      console.log("当前页码：", this.current_page);
      console.log("排序 关键词：", this.sort_key);
      console.log("排序 顺序：", this.sort_order);
      console.log("分类 状态", this.sort_state);
      console.log("_______打印分页参数_______");
    },
    //分页的事件和函数
    //
    //工具函数
    //根据this当前的参数，从持有的所有订单数据中进行选择
    //需要进行排序、筛选
    //得到一页的数据，存入orders_page,列表渲染这个page
    GeneratePage() {
      this.PrintPageParams();
      let temp_page = [];

      //根据参数，直接把全部的原数据排好序
      //先筛选state
      switch (this.sort_state) {
        case "all":
          this.stateFilter_orders_all = this.orders_all;
          break;
        case "running":
          this.stateFilter_orders_all = this.orders_all.filter((item) => {
            return item.state == 1;
          });
          break;
        case "posted":
          this.stateFilter_orders_all = this.orders_all.filter((item) => {
            return item.state == 0;
          });
          break;
        case "done":
          this.stateFilter_orders_all = this.orders_all.filter((item) => {
            return item.state == 2;
          });
          break;
      }

      //筛选后，可能要改总条目数和页数
      //修改总条目数
      if (this.sort_state != "all") {
        this.total_num = this.stateFilter_orders_all.length;
      } else {
        this.total_num = this.orders_all.length;
      }
      //总页数
      this.total_page = Math.ceil(this.total_num / this.page_size);

      //根据关键词排序
      //价格
      console.log("当前stateFilter_orders：", this.stateFilter_orders_all);
      if (this.sort_key == "total_price") {
        // 降序
        if (this.sort_order == "descend") {
          this.stateFilter_orders_all.sort(function (a, b) {
            return b.total_price - a.total_price;
          });
        } else {
          // 升序
          this.stateFilter_orders_all.sort(function (a, b) {
            return a.total_price - b.total_price;
          });
        }
      } //下单时间
      else if (this.sort_key == "place_time") {
        // 降序
        if (this.sort_order == "descend") {
          this.stateFilter_orders_all.sort(function (a, b) {
            //解析得到年月日
            let a_temp = a.place_time.split("-");
            let b_temp = b.place_time.split("-");
            let a_date = [];
            let b_date = [];
            a_temp.forEach((ele) => {
              a_date.push(parseInt(ele));
            });
            b_temp.forEach((ele) => {
              b_date.push(parseInt(ele));
            });
            if (a_date[0] > b_date[0]) return -1;
            else if (a_date[0] < b_date[0]) return 1;
            else {
              if (a_date[1] > b_date[1]) return -1;
              else if (a_date[1] < b_date[1]) return 1;
              else {
                if (a_date[2] > b_date[2]) return -1;
                else if (a_date[2] < b_date[2]) return 1;
                else return 0;
              }
            }
          });
        } else {
          // 升序
          this.stateFilter_orders_all.sort(function (a, b) {
            //解析得到年月日
            let a_temp = a.place_time.split("-");
            let b_temp = b.place_time.split("-");
            let a_date = [];
            let b_date = [];
            a_temp.forEach((ele) => {
              a_date.push(parseInt(ele));
            });
            b_temp.forEach((ele) => {
              b_date.push(parseInt(ele));
            });

            if (a_date[0] > b_date[0]) return 1;
            else if (a_date[0] < b_date[0]) return -1;
            else {
              if (a_date[1] > b_date[1]) return 1;
              else if (a_date[1] < b_date[1]) return -1;
              else {
                if (a_date[2] > b_date[2]) return 1;
                else if (a_date[2] < b_date[2]) return -1;
                else return 0;
              }
            }
          });
        }
      } //上工时间
      else if (this.sort_key == "work_time") {
        // 降序
        if (this.sort_order == "descend") {
          this.stateFilter_orders_all.sort(function (a, b) {
            //解析得到年月日
            let a_temp = a.work_time.split("-");
            let b_temp = b.work_time.split("-");
            let a_date = [];
            let b_date = [];
            a_temp.forEach((ele) => {
              a_date.push(parseInt(ele));
            });
            b_temp.forEach((ele) => {
              b_date.push(parseInt(ele));
            });
            if (a_date[0] > b_date[0]) return -1;
            else if (a_date[0] < b_date[0]) return 1;
            else {
              if (a_date[1] > b_date[1]) return -1;
              else if (a_date[1] < b_date[1]) return 1;
              else {
                if (a_date[2] > b_date[2]) return -1;
                else if (a_date[2] < b_date[2]) return 1;
                else return 0;
              }
            }
          });
        } else {
          // 升序
          this.stateFilter_orders_all.sort(function (a, b) {
            //解析得到年月日
            let a_temp = a.work_time.split("-");
            let b_temp = b.work_time.split("-");
            let a_date = [];
            let b_date = [];
            a_temp.forEach((ele) => {
              a_date.push(parseInt(ele));
            });
            b_temp.forEach((ele) => {
              b_date.push(parseInt(ele));
            });

            if (a_date[0] > b_date[0]) return 1;
            else if (a_date[0] < b_date[0]) return -1;
            else {
              if (a_date[1] > b_date[1]) return 1;
              else if (a_date[1] < b_date[1]) return -1;
              else {
                if (a_date[2] > b_date[2]) return 1;
                else if (a_date[2] < b_date[2]) return -1;
                else return 0;
              }
            }
          });
        }
      }

      if (this.total_num < this.page_size) {
        //不够size，直接返回全部
        temp_page = this.stateFilter_orders_all;
      } else {
        //够size，根据当前页号取10个
        let left = this.page_size * (this.current_page - 1);
        let right = this.page_size * this.current_page;
        temp_page = this.stateFilter_orders_all.slice(left, right);
      }

      this.orders_page = temp_page;
    },
    //currentPage 改变时
    HandlePageChange(currentPage) {
      this.current_page = currentPage;
      this.GeneratePage();
    },

    //排序事件
    tableSortChange(column) {
      //解析，排序的key和order
      //设置排序的参数
      this.sort_key = column.prop;
      if (column.order === "descending") {
        this.sort_order = "descend";
      } else {
        this.sort_order = "ascend";
      }

      this.GeneratePage();
    },

    //赋予不同的订单状态state不同的type名字
    StateTagName(state_name) {
      let tag_name;
      switch (state_name) {
        case "已发布":
          tag_name = "success";
          break;
        case "正在进行":
          tag_name = "";
          break;
        case "已完成":
          tag_name = "info";
          break;
      }
      return tag_name;
    },

    //查看详情 按钮
    async viewDetailButtonClick(row) {
      console.log("row", row);
      this.loading_dialogContent = true;
      //设置当前查看的护理记录ID
      this.dialogVisible = true;
      this.selected_order_id = row.id;
      console.log("查看的护理记录的ID：", this.selected_order_id);
      //根据护理记录ID，请求detail
      await axios
        .request({
          url: "/API/ServiceRecord/" + this.selected_order_id,
          method: "get",
          headers: this.headers,
        })
        .then((res) => {
          var raw_data = res.data.message[0];
          console.log("该护理记录详情的raw data", res);
          //解析数据
          let order_state, self_care_situation, careDirect;
          switch (raw_data.STATUS) {
            case "0":
              order_state = "已发布";
              break;
            case "1":
              order_state = "正在进行";
              break;
            case "2":
              order_state = "已完成";
              break;
          }
          switch (raw_data.NEED) {
            case "0":
              careDirect = "长期居家";
              break;
            case "1":
              careDirect = "医院陪护";
              break;
            case "2":
              careDirect = "短期照护";
              break;
          }
          switch (raw_data.SITUATION) {
            case "0":
              self_care_situation = "完全自理";
              break;
            case "1":
              self_care_situation = "失能";
              break;
            case "2":
              self_care_situation = "失智";
              break;
            case "3":
              self_care_situation = "失能&失智";
              break;
          }
          let client_id = raw_data.ELDERID;
          console.log("raw_data.EVALUATIONID", raw_data.EVALUATIONID);

          //是否评论过了
          if (raw_data.EVALUATIONID != null) {
            //评论过
            this.comment_data.CanUseComment = false;
            //请求这个评论的内容，展示
            axios
              .request({
                url: "/API/Evaluation/" + raw_data.EVALUATIONID,
                method: "get",
                headers: this.headers,
              })
              .then((res) => {
                this.evaluation_data = {
                  grade: parseFloat(res.data.message[0].GRADE),
                  message: res.data.message[0].MESSAGE,
                  time: res.data.message[0].TIME,
                };
                console.log("evaluation_data", this.evaluation_data);
              })
              .catch((err) => {
                console.log(err);
              });
          } else {
            //没评论过
            this.comment_data.CanUseComment = true;
          }

          //取老人ID，再请求老人信息
          axios
            .request({
              url: "/API/Elder/" + client_id,
              method: "get",
              headers: this.headers,
            })
            .then(async (res) => {
              // console.log("老人ID", client_id, "的信息", res.data.message[0]);
              //解析护工的数据
              //正式确认的护工
              try {
                var nurse_intro_json = JSON.parse(raw_data.nurse_intro)[0];
                var nurse_intro = {};
                //请求正式护工的头像
                let avatar;
                if (nurse_intro_json.PHOTO == null) {
                  //本来就无头像
                  avatar = require("../../../assets/Img/default_avator.png");
                } else {
                  //有头像
                  //请求图片资源
                  console.log("/API/file" + nurse_intro_json.PHOTO);
                  await axios
                    .request({
                      url: "/API/file" + nurse_intro_json.PHOTO,
                      responseType: "blob",
                      method: "get",
                      headers: this.headers,
                    })
                    .then((res) => {
                      console.log("请求后端头像,", res);
                      avatar = window.URL.createObjectURL(res.data);
                    })
                    .catch((err) => {
                      //加载出错，替换error图片
                      console.log(err);
                      avatar = require("../../../assets/Img/ImgError.png");
                    });
                }

                if (nurse_intro_json != null) {
                  nurse_intro = {
                    avatar: avatar,
                    photo: nurse_intro_json.PHOTO,
                    name: nurse_intro_json.NAME,
                    age: nurse_intro_json.AGE,
                    gender: nurse_intro_json.GENDER,
                    id: nurse_intro_json.USERID,
                  };
                }
              } catch (error) {
                nurse_intro_json = {};
              }

              //解析接单的等待护工
              let accessible_nurses = [];
              try {
                var nurse_wait_json = JSON.parse(raw_data.nurse_wait);
                // console.log("nurse_wait_json", nurse_wait_json);
                nurse_wait_json.forEach(async (ele) => {
                  //请求护工头像
                  let avatar;
                  if (ele.PHOTO == null) {
                    //本来就无头像
                    avatar = require("../../../assets/Img/default_avator.png");
                  } else {
                    //有头像
                    //请求图片资源
                    console.log("/API/file" + ele.PHOTO);
                    await axios
                      .request({
                        url: "/API/file" + ele.PHOTO,
                        responseType: "blob",
                        method: "get",
                        headers: this.headers,
                      })
                      .then((res) => {
                        console.log("请求后端头像,", res);
                        avatar = window.URL.createObjectURL(res.data);
                      })
                      .catch((err) => {
                        //加载出错，替换error图片
                        console.log(err);
                        avatar = require("../../../assets/Img/ImgError.png");
                      });
                  }

                  let waiting_nurse = {
                    id: ele.USERID,
                    name: ele.NAME,
                    gender: ele.GENDER,
                    avatar: avatar,
                    photo: ele.PHOTO,
                    age: ele.AGE,
                    isChecked: false,
                  };
                  this.hasWaitingNurse = true;
                  accessible_nurses.push(waiting_nurse);
                });
              } catch (error) {
                nurse_wait_json = [];
              }
              //无接单护工
              if (accessible_nurses.length == 0) {
                this.hasWaitingNurse = false;
              }
              //解析护理单项
              let service_items_json = JSON.parse(raw_data.service_items);
              console.log("service_items_json", service_items_json);
              let service_items = [];
              service_items_json.forEach((ele) => {
                let item = {
                  id: ele.SERVICEID,
                  name: ele.NAME,
                  per_price: ele.per_price,
                  piece_price: ele.per_price.split("/")[0],
                  unit: ele.per_price.split("/")[1],
                  num: ele.COUNT,
                };
                service_items.push(item);
              });

              //拼凑order detail
              var order_detail = {
                record_ID: raw_data.RECORDID,
                client_name: res.data.message[0].NAME,
                client_id: raw_data.ELDERID,
                client_phone: res.data.message[0].PHONE,
                order_address: raw_data.ADDRESS,
                contact_address: res.data.message[0].ADDRESS,
                category_name: raw_data.NAME,
                category_id: raw_data.TYPEID,
                work_time: raw_data.ORDER_TIME,
                place_time: raw_data.START_TIME,
                total_price: raw_data.PRICE,
                order_state: order_state, //正在进行，已发布，已完成
                order_state_i: raw_data.STATUS,
                service_items: service_items,
                //自理能力
                self_care_situation_i: raw_data.SITUATION,
                self_care_situation: self_care_situation,

                //护理需求方向
                careDirect_i: raw_data.NEED,
                careDirect: careDirect,

                //接单的护工列表，等待确认
                accessible_nurses: accessible_nurses,

                //正式接单的护工的简介
                nurse_intro: nurse_intro,
                nurse_id: raw_data.NURSINGWORKERID,
                //评价ID
                evaluation_id: raw_data.EVALUATIONID,
                //是否小红点
                updated: raw_data.UPDATED === null ? false : true,
              };

              //this保存 order_detail
              this.order_detail = order_detail;
              //this.接单的护工列表
              this.accessible_nurses = order_detail.accessible_nurses;
              this.nurse_intro = order_detail.nurse_intro;
              console.log("updated", this.order_detail.updated);

              //updated置空,整个put
              if (this.order_detail.updated) {
                let put_data = {
                  typeid: this.order_detail.category_id,
                  start_time: this.order_detail.place_time,
                  order_time: this.order_detail.work_time,
                  address: this.order_detail.order_address,
                  status: this.order_detail.order_state_i,
                  price: this.order_detail.total_price,
                  evaluationid: this.order_detail.evaluation_id,
                  elderid: this.order_detail.client_id,
                  nursingworkerid: this.order_detail.nurse_id,
                  situation: this.order_detail.self_care_situation_i,
                  need: this.order_detail.careDirect_i,
                };
                console.log("updated置空,put_data", put_data);
                axios
                  .request({
                    url: "/API/ServiceRecord/" + this.order_detail.record_ID,
                    method: "put",
                    data: put_data,
                    headers: this.headers,
                  })
                  .then((res) => {
                    console.log("Put", res);
                  })
                  .catch((err) => {
                    console.log(err);
                  });
              }
            })
            .catch((err) => {
              console.log(err);
            });
        })
        .catch((err) => {
          console.log(err);
          if (err.message.includes("403") && !this.AccessTokenFailed) {
            AccessTokenFailed();
            this.AccessTokenFailed = true;
          }
        });
      this.loading_dialogContent = false;
    },

    //tab点击事件,@tab-click自动传入小tab，有index下标可用
    HandleTabsClick(tab) {
      let state;
      switch (parseInt(tab.index)) {
        case 0:
          state = "all";
          break;
        case 1:
          state = "running";
          break;
        case 2:
          state = "posted";
          break;
        case 3:
          state = "done";
          break;
        default:
          state = "all";
          break;
      }
      this.sort_state = state;
      this.GeneratePage();
    },

    //接单的护工列表，点击 选中一个
    ChooseNurseClick(nurse_array_index) {
      this.accessible_nurses.forEach((element, i) => {
        element.isChecked = i == nurse_array_index ? !element.isChecked : false;
      });
    },

    // 查看护工详细信息 按钮，
    CheckNurseDetail(nurse_intro) {
      // console.log(nurse_intro);
      let nurse_id = nurse_intro.id;
      // console.log("跳转到护工的个人信息展示页面", nurse_id);
      this.$router.push({
        name: "NurseUserInfor",
        params: {
          nurse_id: nurse_id,
        },
      });
    },

    //正在进行,【终止服务】按钮
    TernimateButtonClick() {
      this.$msgbox
        .confirm("此操作将直接终止本次服务, 是否确认?", "注意", {
          confirmButtonText: "确定",
          cancelButtonText: "取消",
          type: "warning",
          center: true,
        })
        .then((action) => {
          if (action === "confirm") {
            console.log("向后端请求，终止目标服务");
            let record_ID = this.selected_order_id;
            axios
              .request({
                url: "/API/ServiceRecord/" + record_ID,
                method: "delete",
                headers: this.headers,
              })
              .then((res) => {
                console.log("/API/ServiceRecord/", res);
                this.GetAllRecord();
              })
              .catch((err) => {
                console.log(err);
                if (err.message.includes("403") && !this.AccessTokenFailed) {
                  AccessTokenFailed();
                  this.AccessTokenFailed = true;
                }
              });
            this.dialogVisible = false;
          } else {
            console.log("cancel");
          }

          this.$message({
            type: "success",
            message: "服务已终止!",
          });
        })
        .catch(() => {
          this.$message({
            type: "info",
            message: "已取消",
          });
        });
    },
    //正在进行,【确认服务完成】按钮
    ConfirmServiceCompleteButtonClick() {
      this.$msgbox
        .confirm("您确认本次服务已经完成？", "注意", {
          confirmButtonText: "确认完成",
          cancelButtonText: "取消",
          type: "warning",
          center: true,
        })
        .then((action) => {
          if (action === "confirm") {
            //确认完成护理记录
            //正在进行-》已完成
            console.log("this.order_detail", this.order_detail);
            var put_data = {
              typeid: this.order_detail.category_id,
              start_time: this.order_detail.place_time,
              order_time: this.order_detail.work_time,
              address: this.order_detail.order_address,
              status: "2",
              price: this.order_detail.total_price,
              evaluationid: this.order_detail.evaluation_id,
              elderid: this.order_detail.client_id,
              nursingworkerid: this.order_detail.nurse_intro.id,
              situation: this.order_detail.self_care_situation_i,
              need: this.order_detail.careDirect_i,
            };
            console.log("put_data", put_data);
            axios
              .request({
                url: "/API/ServiceRecord/" + this.order_detail.record_ID,
                method: "put",
                data: put_data,
                headers: this.headers,
              })
              .then((res) => {
                // console.log(res);
                this.$message({
                  type: "success",
                  message: "确认服务完成",
                });
                this.dialogVisible = false;
                this.GetAllRecord();
              })
              .catch((err) => {
                console.log(err);
                if (err.message.includes("403") && !this.AccessTokenFailed) {
                  AccessTokenFailed();
                  this.AccessTokenFailed = true;
                }
              });
          } else {
            console.log("cancel");
          }
        })
        .catch(() => {
          this.$message({
            type: "info",
            message: "已取消",
          });
        });
    },
    //已发布，【确认订单】按钮
    ConfirmNurseOrderButtonClick() {
      this.$msgbox
        .confirm(
          "您将确认订单，以选择的这位护工作为受理人，是否确认?",
          "注意",
          {
            confirmButtonText: "确定",
            cancelButtonText: "取消",
            type: "warning",
            center: true,
          }
        )
        .then((action) => {
          if (action === "confirm") {
            //传递订单ID，选择的护工ID
            console.log("向后端请求，确认订单，选择护工");
            var chosen_nurse = null;
            this.accessible_nurses.forEach((ele) => {
              if (ele.isChecked) {
                chosen_nurse = ele;
              }
            });
            if (chosen_nurse == null) {
              this.$message({
                type: "warning",
                message: "请选择一位护工",
              });
            } else {
              //后端确认护理记录
              let nurse_ID = chosen_nurse.id;
              let record_ID = this.selected_order_id;

              //
              // axios
              //   .request({
              //     url: "/API/servicerecord/nurse",
              //     method: "put",
              //     data: {
              //       recordid: record_ID,
              //       nurseid: nurse_ID,
              //     },
              //   })
              //   .then((res) => {
              //     console.log("确认护理记录", res);
              //   })
              //   .catch((err) => {
              //     console.log(err);
              //   });
              // 确认完成，跳转到支付界面\
              this.$msgbox
                .confirm(
                  "即将跳转支付界面\n支付成功后才可正式确认护工",
                  "注意",
                  {
                    confirmButtonText: "跳转支付",
                    cancelButtonText: "取消",
                    type: "warning",
                    center: true,
                  }
                )
                .then((action) => {
                  if (action === "confirm") {
                    this.dialogVisible = false;
                    this.$message({
                      type: "success",
                      message: "正在确认",
                    });
                    //传递给付款页面的数据
                    let bill_data = {
                      price: this.order_detail.total_price,
                      bill_name: this.order_detail.category_name,
                      backPageName: "CareServiceMine", //退出返回到哪个页面
                      care_recordID: record_ID,
                      care_nurseID: nurse_ID,
                      care_elderID: this.userID,
                      type: "care",
                    };
                    //跳转到 付款页面
                    this.$router.push({
                      name: "ClientPayment",
                      params: { bill_data: bill_data },
                    });
                  }
                })
                .catch(() => {});
            }
          } else {
            console.log("cancel");
          }
        })
        .catch(() => {
          this.$message({
            type: "info",
            message: "已取消",
          });
        });
    },
    //已发布，【取消订单】按钮
    CancelNurseOrderButtonClick() {
      this.$msgbox
        .confirm("此操作将取消本次服务, 您确定吗?", "注意", {
          confirmButtonText: "是",
          cancelButtonText: "否",
          type: "warning",
          center: true,
        })
        .then((action) => {
          if (action === "confirm") {
            console.log("向后端请求，取消这个订单");

            let record_ID = this.selected_order_id;
            axios
              .request({
                url: "/API/ServiceRecord/" + record_ID,
                method: "delete",
                headers: this.headers,
              })
              .then((res) => {
                console.log("/API/ServiceRecord/", res);
                this.GetAllRecord();
              })
              .catch((err) => {
                console.log(err);
                if (err.message.includes("403") && !this.AccessTokenFailed) {
                  AccessTokenFailed();
                  this.AccessTokenFailed = true;
                }
              });
            this.dialogVisible = false;
          } else {
            console.log("cancel");
          }

          this.$message({
            type: "success",
            message: "服务订单已取消",
          });
        })
        .catch(() => {
          this.$message({
            type: "info",
            message: "操作已取消",
          });
        });
    },

    //接受来自CommentBox 子组件的数据
    async GetCommentBoxValue(data) {
      console.log("comment data in parent:", data);

      //下单时间
      let now = new Date();
      let month = now.getMonth() + 1;
      let day = now.getDate();
      let month_str, day_str;
      if (month < 10) {
        month_str = "0" + month.toString();
      } else {
        month_str = month.toString();
      }
      if (day < 10) {
        day_str = "0" + day.toString();
      } else {
        day_str = day.toString();
      }
      let place_time =
        now.getFullYear() +
        "-" +
        month_str +
        "-" +
        day_str +
        " " +
        now.getHours() +
        ":" +
        now.getMinutes() +
        ":" +
        now.getSeconds();
      let evaluation_post = {
        id: this.userID,
        time: place_time,
        grade: data.star_value[0].value,
        message: data.textarea,
      };
      console.log("this.selected_order_id", this.selected_order_id);
      console.log("evaluation_post", evaluation_post);

      await axios
        .request({
          url: "/API/servicerecord/" + this.selected_order_id + "/evaluation",
          method: "post",
          data: evaluation_post,
          headers: this.headers,
        })
        .then(async (res) => {
          console.log(res);
          this.$message({
            type: "success",
            message: "评论成功！",
          });

          this.loading_dialogContent = true;

          this.dialogVisible = true;

          console.log("查看的护理记录的ID：", this.selected_order_id);
          //根据护理记录ID，请求detail
          await axios
            .request({
              url: "/API/ServiceRecord/" + this.selected_order_id,
              method: "get",
              headers: this.headers,
            })
            .then((res) => {
              var raw_data = res.data.message[0];
              console.log("该护理记录详情的raw data", res);
              //解析数据
              let order_state, self_care_situation, careDirect;
              switch (raw_data.STATUS) {
                case "0":
                  order_state = "已发布";
                  break;
                case "1":
                  order_state = "正在进行";
                  break;
                case "2":
                  order_state = "已完成";
                  break;
              }
              switch (raw_data.NEED) {
                case "0":
                  careDirect = "长期居家";
                  break;
                case "1":
                  careDirect = "医院陪护";
                  break;
                case "2":
                  careDirect = "短期照护";
                  break;
              }
              switch (raw_data.SITUATION) {
                case "0":
                  self_care_situation = "完全自理";
                  break;
                case "1":
                  self_care_situation = "失能";
                  break;
                case "2":
                  self_care_situation = "失智";
                  break;
                case "3":
                  self_care_situation = "失能&失智";
                  break;
              }
              let client_id = raw_data.ELDERID;
              console.log("raw_data.EVALUATIONID", raw_data.EVALUATIONID);

              //是否评论过了
              if (raw_data.EVALUATIONID != null) {
                //评论过
                this.comment_data.CanUseComment = false;
                //请求这个评论的内容，展示
                axios
                  .request({
                    url: "/API/Evaluation/" + raw_data.EVALUATIONID,
                    method: "get",
                    headers: this.headers,
                  })
                  .then((res) => {
                    this.evaluation_data = {
                      grade: parseFloat(res.data.message[0].GRADE),
                      message: res.data.message[0].MESSAGE,
                      time: res.data.message[0].TIME,
                    };
                    console.log("evaluation_data", this.evaluation_data);
                  })
                  .catch((err) => {
                    console.log(err);
                  });
              } else {
                //没评论过
                this.comment_data.CanUseComment = true;
              }

              //取老人ID，再请求老人信息
              axios
                .request({
                  url: "/API/Elder/" + client_id,
                  method: "get",
                  headers: this.headers,
                })
                .then(async (res) => {
                  // console.log("老人ID", client_id, "的信息", res.data.message[0]);
                  //解析护工的数据
                  //正式确认的护工
                  try {
                    var nurse_intro_json = JSON.parse(raw_data.nurse_intro)[0];
                    var nurse_intro = {};
                    //请求正式护工的头像
                    let avatar;
                    if (nurse_intro_json.PHOTO == null) {
                      //本来就无头像
                      avatar = require("../../../assets/Img/default_avator.png");
                    } else {
                      //有头像
                      //请求图片资源
                      console.log("/API/file" + nurse_intro_json.PHOTO);
                      await axios
                        .request({
                          url: "/API/file" + nurse_intro_json.PHOTO,
                          responseType: "blob",
                          method: "get",
                          headers: this.headers,
                        })
                        .then((res) => {
                          console.log("请求后端头像,", res);
                          avatar = window.URL.createObjectURL(res.data);
                        })
                        .catch((err) => {
                          //加载出错，替换error图片
                          console.log(err);
                          avatar = require("../../../assets/Img/ImgError.png");
                        });
                    }

                    if (nurse_intro_json != null) {
                      nurse_intro = {
                        avatar: avatar,
                        photo: nurse_intro_json.PHOTO,
                        name: nurse_intro_json.NAME,
                        age: nurse_intro_json.AGE,
                        gender: nurse_intro_json.GENDER,
                        id: nurse_intro_json.USERID,
                      };
                    }
                  } catch (error) {
                    nurse_intro_json = {};
                  }

                  //解析接单的等待护工
                  let accessible_nurses = [];
                  try {
                    var nurse_wait_json = JSON.parse(raw_data.nurse_wait);
                    // console.log("nurse_wait_json", nurse_wait_json);
                    nurse_wait_json.forEach(async (ele) => {
                      //请求护工头像
                      let avatar;
                      if (ele.PHOTO == null) {
                        //本来就无头像
                        avatar = require("../../../assets/Img/default_avator.png");
                      } else {
                        //有头像
                        //请求图片资源
                        console.log("/API/file" + ele.PHOTO);
                        await axios
                          .request({
                            url: "/API/file" + ele.PHOTO,
                            responseType: "blob",
                            method: "get",
                            headers: this.headers,
                          })
                          .then((res) => {
                            console.log("请求后端头像,", res);
                            avatar = window.URL.createObjectURL(res.data);
                          })
                          .catch((err) => {
                            //加载出错，替换error图片
                            console.log(err);
                            avatar = require("../../../assets/Img/ImgError.png");
                          });
                      }

                      let waiting_nurse = {
                        id: ele.USERID,
                        name: ele.NAME,
                        gender: ele.GENDER,
                        avatar: avatar,
                        photo: ele.PHOTO,
                        age: ele.AGE,
                        isChecked: false,
                      };
                      accessible_nurses.push(waiting_nurse);
                    });
                  } catch (error) {
                    nurse_wait_json = {};
                  }

                  //解析护理单项
                  let service_items_json = JSON.parse(raw_data.service_items);
                  console.log("service_items_json", service_items_json);
                  let service_items = [];
                  service_items_json.forEach((ele) => {
                    let item = {
                      id: ele.SERVICEID,
                      name: ele.NAME,
                      per_price: ele.per_price,
                      piece_price: ele.per_price.split("/")[0],
                      unit: ele.per_price.split("/")[1],
                      num: ele.COUNT,
                    };
                    service_items.push(item);
                  });

                  //拼凑order detail
                  var order_detail = {
                    record_ID: raw_data.RECORDID,
                    client_name: res.data.message[0].NAME,
                    client_id: raw_data.ELDERID,
                    client_phone: res.data.message[0].PHONE,
                    order_address: raw_data.ADDRESS,
                    contact_address: res.data.message[0].ADDRESS,
                    category_name: raw_data.NAME,
                    category_id: raw_data.TYPEID,
                    work_time: raw_data.ORDER_TIME,
                    place_time: raw_data.START_TIME,
                    total_price: raw_data.PRICE,
                    order_state: order_state, //正在进行，已发布，已完成
                    order_state_i: raw_data.STATUS,
                    service_items: service_items,
                    //自理能力
                    self_care_situation_i: raw_data.SITUATION,
                    self_care_situation: self_care_situation,

                    //护理需求方向
                    careDirect_i: raw_data.NEED,
                    careDirect: careDirect,

                    //接单的护工列表，等待确认
                    accessible_nurses: accessible_nurses,

                    //正式接单的护工的简介
                    nurse_intro: nurse_intro,
                    nurse_id: raw_data.NURSINGWORKERID,
                    //评价ID
                    evaluation_id: raw_data.EVALUATIONID,
                    //是否小红点
                    updated: raw_data.UPDATED === null ? false : true,
                  };

                  //this保存 order_detail
                  this.order_detail = order_detail;
                  //this.接单的护工列表
                  this.accessible_nurses = order_detail.accessible_nurses;
                  this.nurse_intro = order_detail.nurse_intro;
                  console.log("updated", this.order_detail.updated);

                  //updated置空,整个put
                  if (this.order_detail.updated) {
                    let put_data = {
                      typeid: this.order_detail.category_id,
                      start_time: this.order_detail.place_time,
                      order_time: this.order_detail.work_time,
                      address: this.order_detail.order_address,
                      status: this.order_detail.order_state_i,
                      price: this.order_detail.total_price,
                      evaluationid: this.order_detail.evaluation_id,
                      elderid: this.order_detail.client_id,
                      nursingworkerid: this.order_detail.nurse_id,
                      situation: this.order_detail.self_care_situation_i,
                      need: this.order_detail.careDirect_i,
                    };
                    console.log("updated置空,put_data", put_data);
                    axios
                      .request({
                        url:
                          "/API/ServiceRecord/" + this.order_detail.record_ID,
                        method: "put",
                        data: put_data,
                        headers: this.headers,
                      })
                      .then((res) => {
                        console.log("Put", res);
                      })
                      .catch((err) => {
                        console.log(err);
                      });
                  }
                })
                .catch((err) => {
                  console.log(err);
                });
            })
            .catch((err) => {
              console.log(err);
            });
          this.loading_dialogContent = false;
        })
        .catch((err) => {
          console.log(err);
          if (err.message.includes("403") && !this.AccessTokenFailed) {
            AccessTokenFailed();
            this.AccessTokenFailed = true;
          }
        });
    },
    //获取本用户的所有护理记录
    async GetAllRecord() {
      this.loading_simpleList = true;
      //读取cookies内的老人id
      this.userID = this.$cookies.get("USERID");
      //请求所有的列表数据
      //该userID老人，对应的订单数据
      await axios
        .request({
          url: "/API/ServiceRecord",
          method: "get",
          params: {
            elderID: this.userID,
          },
          headers: this.headers,
        })
        .then((res) => {
          console.log("/API/ServiceRecord", res);
          //目标数据数组
          var orders_all = [];
          var raw_data = res.data.message;
          //处理每个订单数据
          raw_data.forEach((ele) => {
            let state_cn;
            switch (ele.STATUS) {
              case "0":
                state_cn = "已发布";
                break;
              case "1":
                state_cn = "正在进行";
                break;
              case "2":
                state_cn = "已完成";
                break;
              default:
                state_cn = "123";
                break;
            }
            //解析护理单项
            let service_items_json = JSON.parse(ele.service_items);
            // console.log("service_items_json", service_items_json);

            //解析护工名字
            let nurse_name;
            if (ele.NURSINGWORKERID == null) {
              console.log('nurse_name = "暂无"');
              nurse_name = "暂无";
            } else {
              try {
                var nurse_intro_json = JSON.parse(ele.nurse_intro)[0];
                nurse_name = nurse_intro_json.NAME;
              } catch (error) {
                nurse_name = "名字显示错误";
              }

              // console.log("nurse_intro_json", nurse_intro_json);
            }
            let service_items = [];
            service_items_json.forEach((ele) => {
              let item = {
                id: ele.SERVICEID,
                name: ele.NAME,
                per_price: ele.per_price,
                num: ele.COUNT,
              };
              service_items.push(item);
            });
            let order_ = {
              id: ele.RECORDID,
              categoryName: JSON.parse(ele.typename)[0].NAME,
              categoryID: ele.TYPEID,
              state: ele.STATUS,
              state_cn: state_cn,
              work_time: ele.ORDER_TIME.split(" ")[0],
              place_time: ele.START_TIME.split(" ")[0],
              total_price: ele.PRICE,
              nurse_name: nurse_name,
              nurse_id: ele.NURSINGWORKERID,
              service_items: service_items,
              updated: ele.UPDATED === "1" ? true : false, //是否小红点
            };
            orders_all.push(order_);
          });
          //this全部的订单数据
          this.orders_all = orders_all;
          //总共有多少条数据
          this.total_num = raw_data.length;
          //一共有多少页面
          this.total_page = Math.ceil(this.total_num / this.page_size);
          //产生第一页
          this.GeneratePage();
        })
        .catch((err) => {
          console.log(err);
          if (err.message.includes("403") && !this.AccessTokenFailed) {
            AccessTokenFailed();
            this.AccessTokenFailed = true;
          }
        });
      this.loading_simpleList = false;
    },
    //列表按钮，【终止服务】
    TableTernimate(row) {
      console.log("row", row);
      this.selected_order_id = row.id;

      this.TernimateButtonClick();
    },
    //列表按钮，【取消服务】
    TableCancel(row) {
      this.selected_order_id = row.id;
      this.CancelNurseOrderButtonClick();
    },
    //列表按钮，【确认服务完成】
    TableConfirmComplete(row) {
      // console.log("row", row);

      this.$msgbox
        .confirm("您确认本次服务已经完成？", "注意", {
          confirmButtonText: "确认完成",
          cancelButtonText: "取消",
          type: "warning",
          center: true,
        })
        .then(async (action) => {
          if (action === "confirm") {
            //确认完成护理记录

            //先请求原版信息
            await axios
              .request({
                url: "/API/ServiceRecord/" + row.id,
                method: "get",
                headers: this.headers,
              })
              .then(async (res) => {
                let raw_data = res.data.message[0];
                // console.log("/API/ServiceRecord/" + row.id, raw_data);

                //正在进行-》已完成

                var put_data = {
                  typeid: raw_data.TYPEID,
                  start_time: raw_data.START_TIME,
                  order_time: raw_data.ORDER_TIME,
                  address: raw_data.ADDRESS,
                  status: "2",
                  price: raw_data.PRICE,
                  evaluationid: raw_data.EVALUATIONID,
                  elderid: raw_data.ELDERID,
                  nursingworkerid: raw_data.NURSINGWORKERID,
                  situation: raw_data.SITUATION,
                  need: raw_data.NEED,
                };
                // console.log("put_data", put_data);
                //PUT更新
                await axios
                  .request({
                    url: "/API/ServiceRecord/" + row.id,
                    method: "put",
                    data: put_data,
                    headers: this.headers,
                  })
                  .then((res) => {
                    this.$message({
                      type: "success",
                      message: "确认服务完成",
                    });
                    this.GetAllRecord();
                  })
                  .catch((err) => {
                    console.log(err);
                  });
              })
              .catch((err) => {
                console.log(err);
                if (err.message.includes("403") && !this.AccessTokenFailed) {
                  AccessTokenFailed();
                  this.AccessTokenFailed = true;
                }
              });
          } else {
            console.log("cancel");
          }
        })
        .catch(() => {
          this.$message({
            type: "info",
            message: "已取消",
          });
        });
    },
  },

  mounted: function () {
    this.access_token = this.$cookies.get("token");
    this.headers = {
      TokenValue: this.access_token,
    };

    this.GetAllRecord();
  },
};
</script>

// lang选择less语法，scoped限制该样式只在本文件使用，不影响其他组件
<style lang="less" >
.views {
  display: flex;
  flex-direction: column;
  justify-content: center;
  //标签
  .tabs {
  }

  //搜索框
  .search_input_box {
    .inline_input {
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

    .order_table_col {
      .el_table_column {
        display: flex;
        flex-direction: row;
        justify-content: center;
        .badge_box {
          .badge-badge {
            margin-top: 10px;
            margin-right: 40px;
            .checkDetail {
            }
          }
        }
        .other_buttons {
          .button {
            margin-top: 10px;
            margin-right: 40px;
          }
          .report {
            margin-left: 10px;
          }
        }
      }
    }
    /deep/.el-table__body-wrapper::-webkit-scrollbar {
      width: 0;
    }
  }

  //分页
  .pagination_box {
    margin-top: 20px;
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    .pagination {
    }
  }

  // 弹窗对话框
  .serviceDialog {
    //主要内容
    .dialog-content {
      //展示评论
      .show_evaluation {
        display: flex;
        flex-direction: row;
        margin-bottom: 20px;
        border-bottom: 2px solid #f5f5f5;
        padding-bottom: 20px;
        .left_text_box {
          display: flex;
          flex-direction: column;
          .star {
          }
          .text {
            font: 12px Microsoft YaHei, Verdana, Geneva, sans-serif;
            -webkit-font-smoothing: antialiased;
            color: black;
            font-size: 15px;
            letter-spacing: 1px;
          }
        }
        .right_text_box {
          display: flex;
          flex-direction: column;
          justify-content: center;
          align-items: center;
          text-align: right;
          .time {
            font-size: 12px;
            color: #959595;
          }
        }
      }

      //确认的nurse box
      .confirmed_nurse_box {
      }

      .access_hint {
        font-size: 30px;
        font-weight: bold;
      }
      //接单的若干nurses box
      .accessible_nurses_box {
        display: flex;
        flex-direction: row;
        justify-content: center;
        flex-wrap: wrap;
        border: 3px #ebeef5 solid;
        .nurse_card_col {
          // 单张card
          .nurse_card {
            margin-right: 30px;
            margin-left: 30px;
            margin-top: 20px;
            margin-bottom: 20px;

            .card-content {
              width: 100%;
              .card-img {
                width: 200px;
                height: 200px;

                .waiting_img {
                  object-fit: cover;
                  width: 100%;
                  height: 100%;
                }
              }
              .card-info {
                padding-left: 14px;
                font-size: 20px;
                font-weight: bold;
                .card-active {
                  background-color: #73c132;
                  color: white;
                }
                .info-buttons-box {
                  display: flex;
                  flex-direction: row;
                  justify-content: center;
                }
              }
            }
          }
        }
      }

      //订单评价
      .comment_box {
        margin-bottom: 20px;
      }
    }

    //底部按钮
    .dialogFooter {
      display: flex;
      flex-direction: row;
      justify-content: right;
      align-items: center;
      .button {
        margin-right: 35px;
      }
    }
  }
}
</style>>
