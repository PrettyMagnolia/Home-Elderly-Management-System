
<template>
  <div class="views">
    <!-- 展示服务的简介卡片  -->
    <div class="CareServices-Row">
      <!-- 1张卡 -->
      <el-col
        :span="10"
        v-for="(care_service, array_index) in care_services_data"
        :key="care_service.service_uid"
        v-loading="loading_intro"
      >
        <!-- 卡片 -->
        <el-card
          shadow="hover"
          class="service-card"
          @click.native="CheckCardButtonClick($event, array_index)"
          :body-style="{ padding: '0px' }"
        >
          <!-- 卡片内容 -->
          <div class="card-content">
            <div class="card-img">
              <img :src="care_service.service_logo" style="width: 100%" />
            </div>
            <div class="card-info"></div>
          </div>
        </el-card>
      </el-col>
    </div>

    <!-- 弹窗对话框 -->
    <el-dialog
      class="serviceDialog"
      title="服务详情"
      :visible.sync="dialogVisible"
    >
      <el-tabs
        type="card"
        @tab-click="TabsClick"
        class="card_box"
        v-loading="loading_dialog"
      >
        <el-tab-pane label="简介" class="intro_box">
          <div class="dialog-content" v-if="selected_card_index != -1">
            <div class="dialog-img-box">
              <img
                :src="care_services_data[selected_card_index].service_logo"
                alt="ImgError"
                class="dialog-img"
              />
            </div>

            <div class="dialog-info">
              <div>
                {{ care_services_data[selected_card_index].service_name }}
              </div>
              <div>
                {{ care_services_data[selected_card_index].service_type }}
              </div>
              <div>
                {{ care_services_data[selected_card_index].detail_intro }}
              </div>
            </div>
          </div>
        </el-tab-pane>

        <el-tab-pane label="服务单项">
          <el-table :data="service_items_table" stripe style="width: 100%">
            <el-table-column prop="name" label="服务项目" width="180">
            </el-table-column>
            <el-table-column prop="per_price" label="价格" width="180">
            </el-table-column>
            <el-table-column prop="descr" label="描述"> </el-table-column>
          </el-table>
        </el-tab-pane>

        <el-tab-pane label="评价" class="eva_box">
          <!-- 若干条评价内容 -->
          <el-row
            class="eva_item"
            v-for="(eva_item, eva_index) in category_Eva"
            :key="eva_index"
          >
            <el-col :span="15" class="left_text_box">
              <el-rate
                v-model="eva_item.rank"
                disabled
                text-color="#ff9900"
                class="star"
                show-score
              >
              </el-rate>
              <div class="text">
                {{ eva_item.message }}
              </div>
            </el-col>
            <el-col :span="10" class="right_info_box">
              <div class="phone">{{ eva_item.client_phone }}</div>
              <div class="time">{{ eva_item.time }}</div>
            </el-col>
          </el-row>
        </el-tab-pane>
      </el-tabs>

      <!-- 底部的slot插槽 -->
      <span slot="footer" class="dialogFooter">
        <el-button type="primary" @click="EnterPlaceOrderButtonClick()"
          >我要服务</el-button
        >
        <el-button @click="dialogVisible = false">取消</el-button>
      </span>
    </el-dialog>
  </div>
</template>

<script>
import axios from "../../../api/axios";
import { AccessTokenFailed } from "../../../api/data.js";
export default {
  name: "CareServiceIntro",
  components: {},
  data() {
    return {
      AccessTokenFailed: false,
      access_token: null,
      headers: {},
      loading_dialog: true, //loading
      loading_intro: true,
      dialogVisible: false,
      //静态服务卡片数据
      care_services_data: [],
      //选中的服务的所有detail
      chosen_service_detail: {},
      //服务单项表格绑定的数据
      service_items_table: [],
      //当前打开的卡片下标
      selected_card_index: -1,

      //选中服务类型的评价展示
      category_Eva: [],
    };
  },
  methods: {
    //点击 查看详情按钮
    CheckCardButtonClick: async function (event, index) {
      this.loading_dialog = true;
      //index为data数组的下标
      //修改关注下标
      this.selected_card_index = index;
      this.dialogVisible = true;

      //选中服务的id
      let chosen_service_id =
        this.care_services_data[this.selected_card_index]["service_uid"];
      //向后端请求选中的服务的detail
      console.log("查看详情选中的 护理服务类型的id是", chosen_service_id);
      await axios
        .request({
          url: "/API/ServiceType/" + chosen_service_id,
          method: "get",
          headers: this.headers,
        })
        .then((res) => {
          var raw_detail = res.data.message[0];
          console.log("GET 这个护理服务的raw detail:", raw_detail);
          //分析并组合数据
          //护理单项的解析
          let raw_service_items = JSON.parse(raw_detail.service_items);
          let service_items = [];
          let service_items_table = [];
          raw_service_items.forEach((ele) => {
            let service_item = {
              id: ele.SERVICEID,
              name: ele.NAME,
              piece_price: ele.PIECE_PRICE, //单价
              unit: ele.UNIT, //计量单位
              per_price: ele.per_price,
              descr: ele.CONTENT, //描述
            };
            let less_item = {
              name: ele.NAME,
              per_price: ele.per_price,
              descr: ele.CONTENT, //描述
            };
            service_items_table.push(less_item);
            service_items.push(service_item);
          });
          let detail = {
            category_name: raw_detail.NAME,
            category_id: raw_detail.TYPEID,
            category_logo: raw_detail.PHOTO,
            text_introduce: {
              service_intro: raw_detail.INTRODUCE,
              caution: raw_detail.TIPS,
            },
            img_introduce: "",
            service_items: service_items,
          };
          this.service_items_table = service_items_table;
          this.chosen_service_detail = detail;
        })
        .catch((err) => {
          console.log(err);
          if (err.message.includes("403") && !this.AccessTokenFailed) {
            AccessTokenFailed();
            this.AccessTokenFailed = true;
          }
        });

      //请求这个护理类型的所有评论
      await axios
        .request({
          url: "/API/servicetype/" + chosen_service_id + "/evaluation",
          method: "get",
          headers: this.headers,
        })
        .then((res) => {
          let raw_data = res.data.message;
          console.log("evaluation raw_data", raw_data);
          this.category_Eva = [];
          raw_data.forEach((ele) => {
            let client_phone =
              ele.PHONE.substr(0, 3) + "****" + ele.PHONE.substr(-4, 4);
            let eva_line = {
              time: ele.TIME.split(" ")[0],
              rank: parseInt(ele.GRADE),
              message: ele.MESSAGE,
              client_phone: client_phone,
            };
            this.category_Eva.push(eva_line);
            console.log("ele.PHONE", ele.PHONE);
            console.log("client_phone", client_phone);
          });

          // console.log("category_Eva", this.category_Eva);
        })
        .catch((err) => {
          console.log(err);
          if (err.message.includes("403") && !this.AccessTokenFailed) {
            AccessTokenFailed();
            this.AccessTokenFailed = true;
          }
        });
      this.loading_dialog = false;
    },
    EnterPlaceOrderButtonClick: function () {
      //弹窗设置不可见
      this.dialogVisible = false;

      //跳转页面，下订单
      //传递选中的这个服务的所有detail
      this.$router.push({
        name: "PlaceCareOrder",
        params: {
          careServiceDetail: this.chosen_service_detail,
        },
      });
    },

    //服务类型详情卡片，点击评价展示
    TabsClick(tab) {
      //index==2 ,查看评价展示
      if (tab.index == 2) {
      }
    },
  },
  //挂载前,向后端请求数据
  mounted: async function () {
    this.loading_intro = true;
    //从cookie获取本次会话的使用token
    this.access_token = this.$cookies.get("token");
    this.headers = {
      TokenValue: this.access_token,
    };

    console.log("this.headers", this.headers);
    await axios
      .request({
        url: "/API/ServiceType",
        method: "get",
        headers: this.headers,
      })
      // await this.$http
      //   .get(
      //     "/API/ServiceType",
      //     {
      //       headers: {
      //         TokenValue: this.access_token,
      //       },
      //     }
      //   )
      .then((res) => {
        console.log("GET 首页服务类型简介 raw", res.data.message);
        this.care_services_data = [];
        res.data.message.forEach(async (ele) => {
          let line = {
            service_logo: ele.PHOTO,
            service_name: ele.NAME,
            service_uid: ele.TYPEID,
            service_url: ele.PHOTO,
          };
          // console.log("/API/file" + ele.PHOTO);

          //请求图片
          console.log(ele.PHOTO);
          await axios
            .request({
              url: "/API/file" + ele.PHOTO,
              responseType: "blob",
              method: "get",
              headers: this.headers,
            })
            .then((res) => {
              line.service_logo = window.URL.createObjectURL(res.data);
            })
            .catch((err) => {
              //加载出错，替换error图片
              console.log(err);
              line.service_logo = require("../../../assets/Img/ImgError.png");
            });

          this.care_services_data.push(line);
        });
      })
      .catch((err) => {
        console.log(err);
          if (err.message.includes("403") && !this.AccessTokenFailed) {
            AccessTokenFailed();
            this.AccessTokenFailed = true;
          }
      });
    this.loading_intro = false;
  },
};
</script>

// lang选择less语法，scoped限制该样式只在本文件使用，不影响其他组件
<style lang="less" scoped>
.views {
  .CareServices-Row {
    display: flex;
    flex-direction: row;
    justify-content: center;
    flex-wrap: wrap;
    .service-card {
      margin-right: 80px;
      margin-bottom: 40px;
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
    .card_box {
      //简介
      .intro_box {
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
      //评价
      .eva_box {
        display: flex;
        flex-direction: column;
        .eva_item {
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
          .right_info_box {
            display: flex;
            flex-direction: column;
            justify-content: center;
            align-items: center;
            text-align: right;
            .phone {
              color: #535353;
              font-size: 14px;
            }
            .time {
              font-size: 12px;
              color: #959595;
            }
          }
        }
      }
    }
  }
}
</style>>
