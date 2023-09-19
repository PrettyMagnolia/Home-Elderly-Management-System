/*Client 通用的支付界面
creator,editor:yy
usage：
              //传递给付款页面的数据
              let bill_data={
                price:this.order_detail.total_price,
                bill_name:this.order_detail.service_name,
                backPageName:'CareServiceMine', //退出返回到哪个页面
              }
              //跳转到 付款页面
              this.$router.push({
                name: "ClientPayment",
                params: {bill_data:bill_data},
              });
上个页面需要传入 bill_data{price:'',bill_name:'',backPageName:'''}
*/

<template>
  <div class="CareOrderPaymentView">
    <!-- upper 支付提示 -->
    <div class="up_reminder_box" v-if="!error">
      <div class="reminder_icon">
        <i class="el-icon-success"></i>
      </div>
      <div class="reminder_text">
        <h2 class="greeting">欢迎来到付款界面！</h2>
        <div class="price_box">
          <span class="price_hints">{{ bill_data.bill_name }} 金额:</span>
          <span class="price_num">{{ bill_data.price }}</span>
        </div>
      </div>
    </div>

    <!-- bottom 支付 -->
    <div class="bottom_pay_box" v-if="!error">
      <h2>请选择支付方式</h2>

      <!-- 支付方式选择 -->
      <div class="choose_payWay">
        <div
          class="payWay_item"
          v-for="(pay_item, pay_index) in payWay_data"
          :key="pay_index"
          @click="onRadioChange(true, pay_index)"
        >
          <!-- 单选框 -->
          <el-radio
            v-model="payWay_data[pay_index].isChosen"
            :label="true"
            class="pay_radio"
            @click.native.prevent="() => {}"
            >{{ "" }}</el-radio
          >
          <img :src="pay_item.icon" :alt="pay_item.name" class="pay_img" />
        </div>
      </div>

      <!-- 支付按钮 -->
      <el-button
        type="primary"
        class="confirm_pay_button"
        @click="HandleConfirmPay"
        >确认支付</el-button
      >

      <!--退出按钮 -->
      <el-button class="exit_button" @click="HandleExit"> 退出 </el-button>
    </div>

    <!-- 支付弹窗 -->
    <el-dialog
      class="serviceDialog"
      title="线上支付"
      :visible.sync="PayDialogVisible"
      v-if="chosenIndex != -1"
    >
      <div class="onlinePay_box">
        <div class="onlinePay_text">
          {{ payWay_data[chosenIndex].pay_hint }}
        </div>
        <img
          class="onlinePay_QRCode"
          :src="payWay_data[chosenIndex].pay_QRCode"
        />
      </div>

      <div class="payDone">
        <!-- 支付按钮 -->
        <el-button type="primary" class="payDone_button" @click="HandlePayDone"
          >支付完成</el-button
        >
      </div>
    </el-dialog>
  </div>
</template>

<script>
import axios from "../../api/axios";
import { AccessTokenFailed } from "../../api/data.js"
export default {
  name: "ClientPayment",
  components: {},
  data() {
    return {
      access_token: null,
      headers: {},
      bill_data: {},
      error: false,
      payWay_data: [
        {
          name: "支付宝",
          icon: "https://s2.loli.net/2022/07/22/CqZTMGSPWfezXtl.png",
          pay_QRCode: "https://s2.loli.net/2022/07/22/HvW27wyUZxOEgmB.png",
          isChosen: false,
          pay_hint: "请扫描二维码支付",
        },
        {
          name: "微信支付",
          pay_QRCode: "https://s2.loli.net/2022/07/22/WXTV78lhP24b5L9.png",
          icon: "https://s2.loli.net/2022/07/22/xNLPUBEAe8fcHu5.png",
          isChosen: false,
          pay_hint: "请扫描二维码支付",
        },
      ],
      PayDialogVisible: false,
      chosenIndex: -1,
    };
  },
  methods: {
    //单选框改写
    onRadioChange(e, index) {
      this.payWay_data.forEach((ele, i) => {
        if (i == index) {
          this.payWay_data[index].isChosen =
            e === this.payWay_data[index].isChosen ? false : e;
          // console.log(this.payWay_data);
        } else {
          ele.isChosen = false;
        }
      });
    },

    //确认支付 按钮
    HandleConfirmPay() {
      this.payWay_data.forEach((ele, i) => {
        if (ele.isChosen) {
          this.PayDialogVisible = true;
          this.chosenIndex = i;
        }
      });
      if (!this.PayDialogVisible) {
        this.$message({
          message: "请选择一种交易方式",
          type: "warning",
        });
      }
    },
    //退出支付 按钮
    HandleExit() {
      let warn_text = "确认退出支付？";
      if (this.bill_data.type == "care") {
        warn_text = "确认退出支付？\n将取消与护工的确认";
      }
      this.$msgbox
        .confirm(warn_text, "注意", {
          confirmButtonText: "确定",
          cancelButtonText: "取消",
          type: "warning",
          center: true,
        })
        .then((action) => {
          if (action === "confirm") {
            this.$router.push({
              name: this.bill_data.backPageName,
              params: {},
            });
          }
        })
        .catch(() => {});
    },

    //支付完成 按钮
    async HandlePayDone() {
      if (this.bill_data.backPageName === "MedicalServiceHistory") {
        console.log("线上支付看这里");
        console.log(this.bill_data.serviceID);
        this.$http
          .put(
            "/API/medical/order?serviceid=" +
              this.bill_data.serviceID +
              "&orderstatus=已支付",null,{
                      headers:{
                        TokenValue: this.access_token
                      }
                    }
          )
          .then((result) => {
            this.$http
              .put(
                "/API/medical/order?serviceid=" +
                  this.bill_data.serviceID +
                  "&method=线上支付",null,{
                      headers:{
                        TokenValue: this.access_token
                      }
                    }
              )
              .then((result) => {})
              .catch((err) => {
                if(err.message === 'Request failed with status code 500' || err.message === 'Request failed with status code 403'){
                  AccessTokenFailed();
                }
                console.log(err);
              });
          })
          .catch((err) => {
            if(err.message === 'Request failed with status code 500' || err.message === 'Request failed with status code 403'){
              AccessTokenFailed();
            }
            console.log(err);
          });
      }
      //护理服务的确认
      if (this.bill_data.type === "care") {
        let record_ID = this.bill_data.care_recordID;
        let nurse_ID = this.bill_data.care_nurseID;
        let elder_ID = this.bill_data.care_elderID;
        let money = this.bill_data.price;
        console.log({
          recordid: record_ID,
          nurseid: nurse_ID,
          elderid: elder_ID,
          money: money,
        });
        await axios
          .request({
            url: "/API/servicerecord/nurse",
            method: "put",
            data: {
              recordid: record_ID,
              nurseid: nurse_ID,
              elderid: elder_ID,
              money: money,
            },
            headers: this.headers,
          })
          .then((res) => {
            console.log("确认护理记录", res);
            axios
              .request({
                url: "/API/servicerecord/" + record_ID + "/order",
                method: "put",
                headers: this.headers,
              })
              .then((res) => {
                console.log("确认付款", res);
              })
              .catch((err) => {
                console.log(err);
              });
          })
          .catch((err) => {
            console.log(err);
          });
      }
      this.$message({
        message: "支付成功！",
        type: "success",
      });
      this.PayDialogVisible = false;
      this.$router.push({
        name: this.bill_data.backPageName,
        params: {},
      });
    },
  },
  mounted() {
    this.access_token = this.$cookies.get("token");
    this.headers = {
      TokenValue: this.access_token,
    };

    //上个页面传来的 data
    this.bill_data = this.$route.params.bill_data;
    console.log("bill_data", this.bill_data);
  },
};
</script>

<style lang="less" scoped>
.CareOrderPaymentView {
  //upper 支付提示
  .up_reminder_box {
    margin-top: 25px;

    margin-bottom: 25px;
    padding: 20px;
    background-color: #f5f5f5;
    display: flex;
    flex-direction: row;
    .reminder_icon {
      font-size: 50px;
      color: #1677ff;
      display: flex;
      align-items: center;
      margin-right: 20px;
    }
    .reminder_text {
      .greeting {
      }
      .price_box {
        .price_hints {
          font-size: 20px;
          font-weight: bold;
          margin-bottom: 20px;
          margin-top: 20px;
        }
        .price_num {
          margin-left: 10px;
          font-size: 25px;
          font-weight: bold;
          color: #f65200;
        }
      }
    }
  }

  //bottom 支付选项
  .bottom_pay_box {
    .choose_payWay {
      display: flex;
      align-items: center;
      flex-direction: row;
      .payWay_item {
        display: flex;
        align-items: center;
        width: 100%;
        border: solid 1px #e5e5e5;
        border-radius: 5px;
        padding: 20px;
        margin-right: 30px;
        .pay_radio {
        }
        .pay_img {
          width: 40%;
        }
      }
    }
    .confirm_pay_button {
      margin-top: 30px;
      font-size: 20px;
    }
    .exit_button {
      margin-top: 30px;
      font-size: 20px;
    }
  }

  .serviceDialog {
    .onlinePay_box {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      .onlinePay_text {
        font-size: 25px;
        font-weight: bold;
      }
      .onlinePay_QRCode {
      }
    }
    .payDone {
      display: flex;

      align-items: center;
      justify-content: center;
    }
  }
}
</style>