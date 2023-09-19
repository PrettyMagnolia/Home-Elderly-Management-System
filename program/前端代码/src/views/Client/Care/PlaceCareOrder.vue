/*
client 正式下订单的页面
creator,editor:yy 
 */
<template>
  <div class="placeCareOrder_box">
    <div class="order-header">下订单界面，护理订单</div>

    <!-- 分步表单 -->
    <div class="step_box" align-center="true">
      <el-steps :active="order_step" finish-status="success">
        <el-step title="选择服务"> </el-step>
        <el-step title="需求选择"></el-step>

        <el-step title="填写订单"></el-step>
        <el-step title="下单完成"></el-step>
      </el-steps>
    </div>

    <!-- 实际表单部分 -->
    <div class="order-form-box">
      <!-- step1 -->
      <el-form
        class="order-form"
        :model="form"
        label-width="80px"
        v-show="order_step == 0"
      >
        <!-- 服务类型 -->
        <el-form-item>
          <el-col :span="3">服务类型</el-col>
          <el-input
            class="input-item"
            v-model="form.category_name"
            :disabled="true"
            style="font-weight: bold"
          ></el-input>
        </el-form-item>

        <!-- 简介  -->
        <el-form-item>
          <el-col :span="3">简介</el-col>
          <el-col :span="16">
            <el-input
              class="textarea-item"
              type="textarea"
              autosize
              :disabled="true"
              placeholder="请输入内容"
              v-model="form.text_introduce.service_intro"
            >
            </el-input>
          </el-col>
        </el-form-item>

        <!-- 单项服务列表展示 -->
        <el-form-item>
          <el-table
            :data="careServiceDetail.service_items"
            stripe
            style="width: 100%"
          >
            <el-table-column prop="name" label="服务项目" width="120">
            </el-table-column>
            <el-table-column prop="descr" label="描述" width="200">
            </el-table-column>
            <el-table-column prop="per_price" label="价格" width="80">
            </el-table-column>

            <!-- 选择服务 -->
            <el-table-column label=" 购买" fixed="right">
              <template slot-scope="scope">
                <el-button
                  type="primary"
                  size="middle"
                  @click="ChooseOneSI(scope)"
                  >选择服务</el-button
                >
              </template>
            </el-table-column>
          </el-table>
        </el-form-item>

        <!-- 购物车 -->
        <el-form-item class="shopping_cart_box">
          <!-- 购物车头部 -->
          <div class="cart_header">
            <p>
              <i
                class="el-icon-shopping-cart-full"
                style="color: #ff6700; font-weight: 600"
              >
              </i>
              我的购物车
            </p>
            <span
              >温馨提示：产品是否购买成功，以最终下单为准哦，请尽快结算</span
            >
          </div>

          <ul class="cart_body_u">
            <!-- 购物车表头 -->
            <li class="body_header_li">
              <el-col :span="5">商品名称</el-col>
              <el-col :span="4">单价</el-col>
              <el-col :span="4">数量</el-col>
              <el-col :span="3">小计</el-col>
              <el-col :span="3"
                ><span v-html="'\u00a0\u00a0'"></span>操作</el-col
              >
            </li>
            <!-- 购物车表头END -->

            <!-- 购物车列表 -->
            <li
              class="product_list"
              v-for="(pro_item, meanlessIndex) in shopping_cart_N0"
              :key="meanlessIndex"
            >
              <!-- 名称 -->
              <el-col :span="5" class="col_item name">{{
                pro_item.name
              }}</el-col>
              <!-- 单价显示 -->
              <el-col :span="4" class="col_item price">{{
                pro_item.per_price
              }}</el-col>
              <!-- 个数和加减 -->
              <el-col :span="4" class="col_item num">
                <el-button
                  class="UDbutton"
                  style="padding: 0; margin: 0"
                  type="small"
                  icon="el-icon-caret-top"
                  @click="CartItemUp(pro_item.id)"
                ></el-button>
                {{ pro_item.num }}
                <el-button
                  class="UDbutton"
                  style="padding: 0; margin: 0"
                  type="small"
                  icon="el-icon-caret-bottom"
                  @click="CartItemDown(pro_item.id)"
                ></el-button>
              </el-col>
              <!-- 小计 -->
              <el-col :span="3" class="col_item subtotal">{{
                pro_item.subtotal
              }}</el-col>
              <!-- 删除该项 -->
              <el-col :span="3" class="col_item operate">
                <el-button
                  icon="el-icon-error"
                  type="warning"
                  size="small"
                  round
                  @click="DeleteItem(pro_item.id)"
                  >清除</el-button
                >
              </el-col>
              <br />
            </li>
            <!-- END 购物车列表 -->

            <!-- 底部价格合计 -->
            <li class="total_price_box">
              <div class="text_price">
                <span class="price_hint">合计:</span>
                <span class="price_num">{{ total_price }}元</span>
              </div>
            </li>
          </ul>
        </el-form-item>
      </el-form>

      <!--step2  -->
      <el-form
        class="order-form"
        :model="form"
        label-width="80px"
        v-show="order_step == 1"
        :inline="true"
      >
        <!-- 被护理人 自理情况 -->
        <div class="situation_box">
          <!-- head图片 -->
          <div class="head_img">
            <img :src="form.self_care_situation.head_img" alt="ImgErr" />
          </div>
          <!-- head文字 -->
          <div class="head_text">
            {{ form.self_care_situation.head_text }}
          </div>
          <!-- items -->
          <div class="situation_items">
            <el-col
              :class="{
                situation_item: true,
                selected_situation_item: st_item.selected,
              }"
              v-for="(st_item, st_index) in form.self_care_situation.items"
              :key="st_index"
              @click.native="ClickSelfCareItem(st_item)"
            >
              <div class="text_box">
                <div class="name">{{ st_item.name }}</div>
                <div class="descr">{{ st_item.descr }}</div>
              </div>

              <!-- checked 图标 -->
              <div class="icheck" v-show="st_item.selected">
                <i class="el-icon-circle-check"></i>
              </div>
              <div class="icheck" v-show="!st_item.selected">
                <i class="el-icon-circle-check"></i>
              </div>
            </el-col>
          </div>
        </div>

        <!-- 分割线 -->
        <div class="hr_divider">
          <hr class="divider1" />
        </div>

        <!-- 选择护理的需求方向 -->
        <div class="situation_box">
          <!-- head图片 -->
          <div class="head_img">
            <img :src="form.careDirect.head_img" alt="ImgErr" />
          </div>
          <!-- head文字 -->
          <div class="head_text">
            {{ form.careDirect.head_text }}
          </div>
          <!-- items -->
          <div class="situation_items">
            <el-col
              :class="{
                situation_item: true,
                selected_situation_item: st_item.selected,
              }"
              v-for="(st_item, st_index) in form.careDirect.items"
              :key="st_index"
              @click.native="ClickCareDirectItem(st_item)"
            >
              <div class="icon_box">
                <i :class="st_item.icon"></i>
              </div>
              <div class="text_box">
                <div class="name">{{ st_item.name }}</div>
                <div class="descr">{{ st_item.descr }}</div>
              </div>

              <!-- checked 图标 -->
              <div class="icheck" v-show="st_item.selected">
                <i class="el-icon-circle-check"></i>
              </div>
              <div class="icheck" v-show="!st_item.selected">
                <i class="el-icon-circle-check"></i>
              </div>
            </el-col>
          </div>
        </div>
      </el-form>

      <!--step3  -->
      <el-form
        class="order-form"
        :model="form"
        v-show="order_step == 2"
        :rules="rules"
        ref="form"
      >
        <!-- 时间 -->

        <el-form-item>
          <el-col :span="2">预约日期</el-col>

          <el-col :span="10">
            <el-form-item prop="work_time">
              <el-date-picker
                type="date"
                placeholder="选择日期"
                v-model="form.work_time"
                style="width: 206px"
              ></el-date-picker>
            </el-form-item>
          </el-col>
        </el-form-item>

        <!-- 联系电话 -->
        <el-form-item>
          <el-col :span="2">联系电话</el-col>
          <el-col :span="6">
            <el-form-item prop="number_phone">
              <el-input
                class="input-item"
                v-model="form.number_phone"
                placeholder="请输入联系电话"
              ></el-input>
            </el-form-item>
          </el-col>
        </el-form-item>

        <!-- 地址 -->
        <el-form-item>
          <el-col :span="2">地址</el-col>
          <el-col :span="6">
            <el-form-item prop="address">
              <el-input
                class="input-item"
                v-model="form.address"
                placeholder="请输入服务地址"
                style="width: 400px"
              ></el-input>
            </el-form-item>
          </el-col>
        </el-form-item>

        <!-- 价格 -->

        <el-form-item class="f_total_price_box">
          <el-col :span="2">价格</el-col>
          <span class="price_num">{{ total_price }}元</span>
        </el-form-item>
      </el-form>

      <!-- step 4  -->
      <el-form
        class="order-form"
        :model="form"
        v-show="order_step == 3"
        :rules="rules"
      >
        <span>成功提交了您的预约请求！</span>
        <br />
        <span>请等待护工接受预约订单</span>
        <br />
      </el-form>
    </div>

    <br />
    <br />

    <!-- 按钮盒子 -->
    <div class="button-box">
      <el-button
        class="step-button"
        v-on:click="back"
        v-show="order_step != 0 && order_step != 3"
        >上一步</el-button
      >

      <el-button class="step-button" v-on:click="exit" v-show="order_step == 0"
        >返回</el-button
      >
      <el-button
        class="step-button"
        v-on:click="exitToService"
        v-show="order_step == 3"
        type="primary"
        >退出</el-button
      >
      <el-button
        class="step-button"
        v-on:click="next"
        type="primary"
        v-show="order_step != 2 && order_step != 3"
        >下一步</el-button
      >

      <el-button
        class="step-button"
        v-on:click="formSubmit('form')"
        type="primary"
        v-show="order_step == 2"
        >提交</el-button
      >
    </div>
  </div>
</template>

<script>
import axios from "axios";
import { AccessTokenFailed } from "../../../api/data.js";
export default {
  name: "PlaceCareOrder",
  components: {},
  data() {
    //自定义校验规则
    //上工日期
    var validateTime = (rule, value, callback) => {
      if (!value) {
        return callback(new Error("请选择服务的日期"));
      } else {
        var today = new Date(); //获取当前时间
        console.log("now : ", today);
        console.log("work time: ", this.form.work_time);

        // 服务时间在过去，显然不行
        if (this.form.work_time <= today) {
          return callback(new Error("预约日期需要是未来"));
        } else {
          let form_time =
            this.form.work_time.getFullYear() +
            "-" +
            (parseInt(this.form.work_time.getMonth()) + 1).toString() +
            "-" +
            this.form.work_time.getDate();
          this.form.form_time = form_time;
          console.log("填入表中的上工时间为：", this.form.form_time);
          return callback();
        }
      }
    };
    // var validateTime = (rule, value, callback) => {
    //   if (!value) {
    //     return callback(new Error("请选择服务的日期"));
    //   } else {
    //     if (this.form.time_start > this.form.time_end)
    //     {
    //       return callback(new Error("起始日期必须早于结束日期"));
    //     }
    //     else return callback();
    //   }
    // };

    //电话号码校验
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
      AccessTokenFailed: false,
      access_token: null,
      headers: {},

      orderData: {},
      order_step: 0,
      careServiceDetail: {}, //护理服务的所有detail

      service_detail: {},
      //表单模型
      form: {
        userID: "",
        category_id: "",
        category_name: "",
        text_introduce: {
          service_intro: "",
          caution: "",
        },
        service_items: [],
        shopping_cart: [],

        address: "", //服务地址，待填
        number_phone: "", //手机号，待填
        work_time: "", //上工时间，待填
        form_time: "", //上工时间的标准版
        place_time: "", //下单时间，今天
        //老人自理情况
        self_care_situation: {
          items: [
            {
              name: "完全自理",
              descr: "可以自行活动",
              selected: false,
            },
            {
              name: "失能",
              descr: "需要协助活动",
              selected: false,
            },
            {
              name: "失智",
              descr: "有认知症功能障碍",
              selected: false,
            },
            {
              name: "失能&失智",
              descr: "协助活动与认知障碍",
              selected: true,
            },
          ],
          head_img: require("../../../assets/Img/Client_PlaceCareOrder/self_care_situation.jpg"),
          head_text: "被护理人自理情况",
        },

        //护理需求方向
        careDirect: {
          items: [
            {
              name: "长期居家",
              icon: "el-icon-house",
              descr: "照顾老人很重要\n医疗知识少不了",
              selected: true,
            },
            {
              name: "医院陪护",
              icon: "el-icon-school",
              descr: "家里医院两头跑\n医院陪护刚刚好",
              selected: false,
            },
            {
              name: "短期照护",
              icon: "el-icon-time",
              descr: "突发状况很紧急\n临时看护我应急",
              selected: false,
            },
          ],
          head_img: require("../../../assets/Img/Client_PlaceCareOrder/careDirect.png"),
          head_text: "护理的需求方向",
        },
      },
      //表单检验规则
      rules: {
        work_time: [{ validator: validateTime, trigger: "blur" }],
        number_phone: [{ validator: validatePhone, trigger: "blur" }],
        address: [{ required: true, message: "请输入地址", trigger: "blur" }],
      },
    };
  },
  methods: {
    //step分步表单下一步
    next() {
      if (++this.order_step > 3) this.order_step = 0;

      //步骤1，选择服务，购物车
      if (this.order_step == 1 && this.shopping_cart_N0.length == 0) {
        this.order_step--;
        this.$message({
          type: "warning",
          message: "请至少选择一个服务",
        });
      }

      //步骤2，选择自理能力与护理需求
      if (this.order_step == 2) {
        //自理能力有没有选择
        if (!this.isSelfCareChosen) {
          this.order_step--;
          this.$message({
            type: "warning",
            message: "请选择老人的自理能力",
          });
        }

        //护理方向有没有选择
        if (!this.isCareDirectChosen) {
          this.order_step--;
          this.$message({
            type: "warning",
            message: "请选择一个护理方向",
          });
        }
      }
    },
    //step上一步
    back() {
      if (--this.order_step < 0) this.order_step = 0;
    },

    //退出到展示页面
    exit() {
      this.$router.push({
        name: "CareServiceIntro",
      });
    },
    exitToService() {
      this.$router.push({
        name: "CareServiceMine",
      });
    },

    //对服务单项，选中一个服务单项加入到cart
    ChooseOneSI(scope) {
      let chosen_id = this.form.service_items[scope.$index].id;

      this.form.shopping_cart.forEach((ele) => {
        if (ele.id == chosen_id) {
          ele.num++;

          console.log(chosen_id, "增加到", ele.num);
          ele.subtotal = ele.num * ele.piece_price;
        }
      });
    },

    //购物车的 up 按钮,要增加的护理服务id
    CartItemUp(id) {
      this.form.shopping_cart.forEach((ele) => {
        if (ele.id == id) {
          ele.num++;
          console.log(id, "增加到", ele.num);
          ele.subtotal = ele.num * ele.piece_price;
        }
      });
    },
    //购物车的 down 按钮,要减少的护理服务id
    CartItemDown(id) {
      this.form.shopping_cart.forEach((ele) => {
        if (ele.id == id) {
          ele.num--;
          console.log(id, "增加到", ele.num);
          ele.subtotal = ele.num * ele.piece_price;
        }
      });
    },
    //购物车，删除一项
    DeleteItem(id) {
      this.form.shopping_cart.forEach((ele) => {
        if (ele.id == id) {
          ele.num = 0;
          ele.subtotal = ele.num * ele.piece_price;
        }
      });
    },

    //自理能力，选择item
    ClickSelfCareItem(self_care_item) {
      this.form.self_care_situation.items.forEach((ele) => {
        if (ele == self_care_item) {
          ele.selected = !ele.selected;
        } else {
          ele.selected = false;
        }
      });
    },

    //护理需求，选择item
    ClickCareDirectItem(item) {
      this.form.careDirect.items.forEach((ele) => {
        if (ele == item) {
          ele.selected = !ele.selected;
        } else {
          ele.selected = false;
        }
      });
    },

    //表单提交
    formSubmit(formName) {
      //触发检查
      this.$refs[formName].validate((valid) => {
        if (valid) {
          this.$message({
            type: "success",
            message: "预约提交成功！",
          });
          this.order_step++;
          console.log("form", this.form);

          //后端接口，增加服务记录
          //situation和need
          var situation, need;
          this.form.self_care_situation.items.forEach((ele, i) => {
            if (ele.selected) {
              situation = i;
              return;
            }
          });
          this.form.careDirect.items.forEach((ele, i) => {
            if (ele.selected) {
              need = i;
              return;
            }
          });
          let shop_data = [];
          //遍历购物车，构建表单
          this.form.shopping_cart.forEach((ele) => {
            let shop_item = {};
            if (ele.num != 0) {
              shop_item = {
                serviceid: ele.id,
                count: ele.num,
              };
              shop_data.push(shop_item);
            }
          });
          //拼凑表单
          let submit = {
            typeid: this.form.category_id,
            start_time: this.form.place_time + " 0:00:00",
            order_time: this.form.form_time + " 0:00:00",
            address: this.form.address,
            status: "0",
            price: this.total_price,
            evaluationid: "",
            elderid: this.form.userID,
            nursingworkerid: "",
            situation: situation,
            need: need,
            serviceitems: shop_data,
          };
          //提交后端接口，新增服务记录
          axios
            .request({
              url: "/API/ServiceRecord",
              method: "post",
              data: submit,
              headers: this.headers,
            })
            .then((res) => {
              console.log("res", res);
              // console.log('res.data',res.data);
              // let res_json = JSON.parse("["+res.data+"]");
              // console.log("res_json", res_json);
            })
            .catch((err) => {
              console.log(err);
              if (err.message.includes("403") && !this.AccessTokenFailed) {
                AccessTokenFailed();
                this.AccessTokenFailed = true;
              }
            });
          console.log("submit: ", submit);
        } else {
          this.$message({
            type: "warning",
            message: "请填写正确的表单！",
          });

          return false;
        }
      });
    },

    //点击选项卡片
    OptionCardCick(index) {
      this.form.options[index].isSelected =
        !this.form.options[index].isSelected;
      console.log(this.form.options[index].isSelected);
    },
  },
  mounted: function () {
    this.access_token = this.$cookies.get("token");
    this.headers = {
      TokenValue: this.access_token,
    };

    //读取cookies内的老人id
    this.form.userID = this.$cookies.get("USERID");

    //获取上个页面传来的数据，保存
    this.careServiceDetail = this.$route.params.careServiceDetail;

    //构建表单
    this.form.category_id = this.careServiceDetail.category_id;
    this.form.category_name = this.careServiceDetail.category_name;
    this.form.text_introduce = this.careServiceDetail.text_introduce;
    this.form.service_items = this.careServiceDetail.service_items;
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
    let place_time = now.getFullYear() + "-" + month_str + "-" + day_str;
    this.form.place_time = place_time;

    this.careServiceDetail.service_items.forEach((ele) => {
      //购物车
      this.form.shopping_cart.push({
        id: ele.id,
        name: ele.name,
        piece_price: ele.piece_price, //100
        unit: ele.unit, //天
        per_price: ele.per_price, // 100/天
        num: 0, //个数
        subtotal: 0, //小计
      });
    });
  },
  computed: {
    //返回 个数不是0的服务单项
    shopping_cart_N0: {
      get: function () {
        var N0 = [];
        this.form.shopping_cart.forEach((ele) => {
          if (ele.num != 0) {
            N0.push(ele);
          }
        });
        return N0;
      },
    },

    //总价钱
    total_price: {
      get: function () {
        var tp = 0;
        this.form.shopping_cart.forEach((ele) => {
          tp += ele.subtotal;
        });
        return tp;
      },
    },

    //自理情况是否选择了
    isSelfCareChosen: {
      get: function () {
        let is = false;
        this.form.self_care_situation.items.forEach((ele) => {
          // 有一个选项被选中了
          if (ele.selected) {
            is = true;
          }
        });
        return is;
      },
    },
    //护理的需求方向是否选择了
    isCareDirectChosen: {
      get: function () {
        let is = false;
        this.form.careDirect.items.forEach((ele) => {
          // 有一个选项被选中了
          if (ele.selected) {
            is = true;
          }
        });
        return is;
      },
    },
  },
};
</script>

// lang选择less语法，scoped限制该样式只在本文件使用，不影响其他组件
<style lang="less" scoped>
.placeCareOrder_box {
  height: 100%;
  width: 100%;
  //标题
  .order-header {
    display: flex;
    justify-content: center;

    padding-bottom: 10px;

    font-family: "Microsoft YaHei";
    font-weight: bold;
    font-size: 25px;
  }

  //步骤
  .step_box {
    margin-bottom: 50px;
  }

  // 实际表单
  .order-form-box {
    display: flex;
    padding-right: 60px;
    .order-form {
      width: 100%;
      height: 100%;
      //购物车
      .shopping_cart_box {
        width: 100%;
        .cart_header {
        }
        //购物车的列表主体
        .cart_body_u {
          width: 100%;
          line-height: 55px; //行高
          padding-inline-start: 0px;
          padding: 20px;
          //每行
          li {
            list-style: none; //去掉li黑点
          }
          //表头
          .body_header_li {
            display: flex;
            flex-direction: row;
            background-color: #f5fafc;
            div {
            }
          }
          //列表主体
          .product_list {
            .col_item {
            }
            .num {
              .UDbutton {
              }
            }
          }

          //底部价格合计
          .total_price_box {
            display: flex;
            justify-content: right;
            .text_price {
              .price_hint {
                font-size: 25px;
              }
              .price_num {
                font-size: 35px;
                color: #fe6700;
              }
            }
          }
        }
      }

      //禁止输入的样式，样式穿透
      .input-item /deep/.el-input__inner:disabled,
      .textarea-item /deep/.el-textarea__inner:disabled {
        color: black;
        font-weight: bold;
      }
      .input-item {
        width: auto;
      }

      //自理能力 box
      .situation_box {
        display: flex;
        flex-direction: column;
        align-items: center;
        justify-content: center;

        .head_img {
        }
        .head_text {
          margin-top: 10px;
          font-size: 20px;
          font-weight: bold;
        }

        //通用的选项box样式
        .situation_items {
          display: flex;
          flex-direction: row;
          justify-content: center;
          align-items: center;
          .situation_item {
            //每个自理卡片box
            background-color: #f5f5f5;
            margin-left: 20px;
            margin-right: 20px;
            margin-top: 30px;
            margin-bottom: 30px;
            border-radius: 10px;

            display: flex;
            flex-direction: column;
            justify-content: center;
            align-items: center;
            //图标表示
            .icon_box {
              font-size: 30px;
            }
            //文字描述
            .text_box {
              display: flex;
              flex-direction: column;
              justify-content: center;
              align-items: center;
              padding: 10px;
              .name {
                font-weight: bold;
                margin-bottom: 10px;
                color: #036fa8;
              }
              .descr {
                font-family: "宋体";
                white-space: pre-line;
                font-weight: bold;
              }
            }
            //选中的图标
            .icheck {
              font-size: 30px;
              color: whitesmoke;
            }
          }
          .selected_situation_item {
            background-color: #74b1ff;
            color: white !important;
          }
        }
      }

      //分割线
      .hr_divider {
        .divider_1 {
          display: block;
          content: "";
          height: 30px;
          margin-top: -31px;
          border-style: solid;
          border-color: black;
          border-width: 0 0 1px 0;
          border-radius: 20px;
        }
        margin: 40px;
      }

      //显示总价格
      .f_total_price_box {
        .price_num {
          font-size: 30px;
          font-weight: bold;
          color: #fe6700;
        }
      }

      // .options-box {
      //   //卡片 未选中
      //   .option-card {
      //     margin-left: 10px;
      //     margin-bottom: 20px;
      //     background-color: #eae7e7;
      //     padding: 0;
      //   }

      //   //卡片选中
      //   .card-active {
      //     background-color: #03a9f4;
      //     color: white;
      //   }
      // }
    }
  }
  //按钮
  .button-box {
    display: flex;
    justify-content: center;
    .step-button {
    }
  }
}
</style>>
