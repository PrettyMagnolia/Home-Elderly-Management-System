/*
功能：用户 首页页面
creator：yy
editor：yy
 */
<template>
  <div class="client-index">
    <!-- 测试API用 -->
    <div class="test_api_button" v-if="false">
      <el-button @click="GET_test">GET test</el-button>
      <el-button @click="TOKEN_test">TOKEN test</el-button>
    </div>

    <!-- 顶部的走马灯 -->
    <el-carousel
      arrow="never"
      trigger="click"
      :height="bannerHeight + 'px'"
      class="topped-carousel"
    >
      <el-carousel-item
        v-for="item in slideShow_Images"
        :key="item"
        class="slide-item"
      >
        <img
          :src="item"
          alt="Img error"
          @load="SlideImgLoad"
          ref="SlideImg"
          class="slide-item-img"
        />
      </el-carousel-item>
    </el-carousel>
    <!-- <div>当前bannerHeight：{{ bannerHeight }}</div> -->

    <!-- 护理服务介绍 -->
    <div class="care-intro-box">
      <!-- img box -->
      <div class="care-img-box">
        <img
          :src="care_intro_data.care_intro_img"
          alt="Img Error"
          class="care-img-item"
        />
      </div>
      <!-- text box -->
      <div class="care-text-box">
        <!-- upper text -->
        <div class="text-upper">
          <h2>护理平台：无微不至</h2>
          <p>公司愿景·成为领先的智慧养老综合解决方案提供商</p>
        </div>
        <!-- lower text -->
        <div class="text-lower">
          <div class="lower-row">
            <el-row
              v-for="(itemRow, indexRow) in care_intro_data.care_text_lower"
              :key="indexRow"
              class="lower-el-row"
            >
              <el-col
                :span="12"
                v-for="(itemCol, indexCol) in itemRow"
                :key="indexCol"
                class="lower-el-col"
              >
                <img :src="require('../../../assets/Img/Client_Home/tick_icon.png')">
                <h3>{{ itemCol }}</h3>
              </el-col>
            </el-row>
          </div>
        </div>
      </div>
    </div>

    <!-- 介绍预约护理的流程 -->
    <div class="care-flow-box">
      <!-- header部分 -->
      <div class="flow_head_box">
        <h2 class="head_text">{{ care_flow_data.head_text }}</h2>
      </div>
      <!-- 具体的护理服务流程 -->
      <div class="flow_body_box">
        <el-col
          :span="10"
          class="body_item"
          v-for="(flow_item, flow_index) in care_flow_data.flow_data"
          :key="flow_index"
        >
          <div class="item_img">
            <img :src="flow_item.img" alt="ImgErro" />
          </div>

          <div class="item_text">
            <div class="text_name">
              {{ flow_item.step }}.{{ flow_item.name }}
            </div>
            <p class="text_descr">{{ flow_item.descr }}</p>
          </div>
        </el-col>
      </div>
    </div>

    <!-- 医疗服务介绍 -->
    <div class="care-intro-box">
      <!-- text box -->
      <div class="care-text-box">
        <!-- upper text -->
        <div class="text-upper">
          <h2>{{ medic_intro_data.text_up[0] }}</h2>
          <p>{{ medic_intro_data.text_up[1] }}</p>
        </div>
        <!-- lower text -->
        <div class="text-lower">
          <div class="lower-row">
            <el-row
              v-for="(itemRow, indexRow) in medic_intro_data.text_lower"
              :key="indexRow"
              class="lower-el-row"
            >
              <el-col
                :span="12"
                v-for="(itemCol, indexCol) in itemRow"
                :key="indexCol"
                class="lower-el-col"
              >
                <img :src="require('../../../assets/Img/Client_Home/locate_icon.png')">
                <h3>{{ itemCol }}</h3>
              </el-col>
            </el-row>
          </div>
        </div>
      </div>

      <!-- img box -->
      <div class="care-img-box">
        <img
          :src="medic_intro_data.img"
          alt="Img Error"
          class="care-img-item"
        />
      </div>
    </div>

    <!-- 介绍医疗服务的流程 -->
    <div class="care-flow-box">
      <!-- header部分 -->
      <div class="flow_head_box">
        <h2 class="head_text">{{ medic_flow_data.head_text }}</h2>
      </div>
      <!-- 具体的护理服务流程 -->
      <div class="flow_body_box">
        <el-col
          :span="10"
          class="body_item"
          v-for="(flow_item, flow_index) in medic_flow_data.flow_data"
          :key="flow_index"
        >
          <div class="item_img">
            <img :src="flow_item.img" alt="ImgErro" />
          </div>

          <div class="item_text">
            <div class="text_name">
              {{ flow_item.step }}.{{ flow_item.name }}
            </div>
            <p class="text_descr">{{ flow_item.descr }}</p>
          </div>
        </el-col>
      </div>
    </div>
  </div>
</template>

<script>
import { ClientBanTime } from "../../../api/data.js";
import { AccessTokenFailed } from "../../../api/data.js";
export default {
  name: "Home",
  components: {},
  data() {
    return {
      //轮播图高度
      bannerHeight: "",
      screenWidth: "",
      // 轮播图的图片源
      slideShow_Images: [require('../../../assets/Img/Client_Home/slideShow_1.png')],

      care_intro_data: {
        //care intro图片
        care_intro_img: require('../../../assets/Img/Client_Home/care_intro_img.jpg'),
        //care lower文字
        care_text_lower: [
          ["平台把关", "证件齐全"],
          ["海量护工\n在线预约", "全天派工"],
        ],
      },

      //护理服务的流程介绍
      care_flow_data: {
        head_text: "如何预约护工服务？",
        //护理服务的流程描述
        flow_data: [
          {
            step: 1,
            img: require('../../../assets/Img/Client_Home/care_flow_1.png'),

            name: "预约下单",
            descr:
              "用户在护理介绍页面自由选择所需的服务项目，填写信息后预约下单，完成服务订单的预约发布，但无需付费",
          },
          {
            step: 2,
            img: require('../../../assets/Img/Client_Home/care_flow_2.png'),
            name: "护工接单",
            descr:
              "平台护工查看并接受您发布的服务订单，同一时间段可能有多个护工接单，订单等待着您的确认",
          },
          {
            step: 3,
            img: require('../../../assets/Img/Client_Home/care_flow_3.png'),
            name: "确认订单",
            descr:
              "用户正式确认一位护工作为本订单的接单人，若有多个护工接单则由用户自由挑选，服务订单正式成立，此时用户进行付款，该护工负责本次服务",
          },
          {
            step: 4,
            img: require('../../../assets/Img/Client_Home/care_flow_4.png'),
            name: "享受服务",
            descr:
              "护工将按照您指定的服务时间和地址，上门提供优质服务。对服务或护工有任何不满，您有权随时终止服务或发起投诉",
          },
        ],
      },

      //医疗的介绍
      medic_intro_data: {
        img: require('../../../assets/Img/Client_Home/medic_intro.png'),
        text_up: ["社区医生：上门服务", "贴心的居家养老便捷方案引领者"],
        text_lower: [
          ["线上预约", "专业资格"],
          ["居家服务", "健康状态\n实时关注"],
        ],
      },
      //医疗服务流程
      medic_flow_data: {
        head_text: "医疗服务流程",
        flow_data: [
          {
            step: 1,
            img: require('../../../assets/Img/Client_Home/medic_flow_1.png'),
            name: "预约服务",
            descr:
              "用户在申请医生上门服务界面核对个人信息后，预约选择服务时间和社区医生，提交预约单",
          },
          {
            step: 2,
            img:  require('../../../assets/Img/Client_Home/medic_flow_2.png'),
            name: "医生受理",
            descr: "您可以在申请记录查看界面查看您的预约结果",
          },
          {
            step: 3,
            img: require('../../../assets/Img/Client_Home/medic_flow_3.png'),
            name: "上门服务",
            descr:
              "社区医生会按照您的预约时间提供上门服务，服务期间会依据您的健康状况提供健康报告与健康指导建议",
          },
          {
            step: 4,
            img: require('../../../assets/Img/Client_Home/medic_flow_4.png'),
            name: "服务评价",
            descr: "您可以在服务结束后对社区医生的服务做出评价",
          },
        ],
      },
    };
  },
  methods: {
    //GET for 135 API
    GET_test() {
      console.log("click 135 API");
      let _url = "/API/user";
      let params = {
        uname: "123",
        upassword: "123",
      };
      //GET
      this.$http
        .get(_url, { params: params })
        .then((result) => {
          console.log(result);
        })
        .catch((err) => {
          console.log(err);
        });
    },

    TOKEN_test() {
      console.log("click 135 API");
      let userID = this.$cookies.get("USERID");
      let TokenValue = this.$cookies.get("token");
      // console.log('ClientBanTime',ClientBanTime(userID, TokenValue));
      ClientBanTime(userID, TokenValue)
        .then((res) => {
          console.log('res',res);
        })
        .catch((err) => {
          console.log('err',err);
        });
      // let headers = {
      //   TokenValue: TokenValue,
      // };
      // console.log("headers", headers);
      // this.$http
      //   .get("/API/elder/v2/" + userID, {
      //     headers: headers,
      //   })
      //   .then((token_res) => {
      //     console.log("/API/elder/v2/", token_res);
      //   })
      //   .catch((token_err) => {
      //     console.log(token_err);
      //   });
    },
    // when Slide Img  Load
    SlideImgLoad() {
      //第一次加载时,获取图片在窗口中的高度,赋值给Slide
      this.$nextTick(() => {
        this.bannerHeight = this.$refs.SlideImg[0].height;
        // console.log(this.$refs.SlideImg[0].height);
      });
    },

    //页面尺寸改变 监听函数
    getAutoHeight() {
      this.bannerHeight = this.$refs.SlideImg[0].height;
      this.SlideImgLoad();
    },
  },

  mounted() {
    this.SlideImgLoad();
    this.screenWidth = window.innerWidth;
    window.addEventListener("resize", this.getAutoHeight, false);
  },
  //销毁组件时移除监听
  destroyed() {
    window.removeEventListener("resize", this.getAutoHeight, false);
  },
};
</script>

// lang选择less语法，scoped限制该样式只在本文件使用，不影响其他组件
<style lang="less" scoped>
.client-index {
  //轮播图
  .topped-carousel {
    .slide-item {
      .slide-item-img {
        width: 100%;
      }
    }
  }

  //护理服务的宣传
  .care-intro-box {
    display: flex;
    justify-content: center;
    align-items: center;
    padding-top: 50px;
    padding-bottom: 50px;
    padding-left: 30px;
    padding-right: 30px;

    .care-img-box {
      margin-right: 60px;
      .care-img-item {
        width: 100%;
        height: 100%;
      }
    }
    .care-text-box {
      .text-upper {
        h2 {
          letter-spacing: 0.2px;
        }
        p {
          font-size: 1.1875rem;
          line-height: 1.69;
          color: rgba(22, 28, 45, 0.7);
          display: block;
        }
      }
      .text-lower {
        .lower-row {
          .lower-el-row {
            .lower-el-col {
              padding-top: 10px;
              padding-bottom: 10px;

              display: flex;
              justify-content: left;
              align-items: center;
              img {
                width: 30px;
                margin-right: 10px;
              }
            }
          }
        }
      }
    }
  }

  //预约护理服务的流程
  .care-flow-box {
    display: flex;
    flex-direction: column;
    justify-content: center;

    //head
    .flow_head_box {
      .head_text {
        font-size: 35px;
      }
    }

    //body
    .flow_body_box {
      display: flex;
      flex-direction: row;
      justify-content: center;
      align-items: center;
      flex-flow: wrap;
      .body_item {
        display: flex;
        flex-direction: column;
        justify-content: center;
        align-items: center;

        margin-left: 10px;
        margin-right: 10px;

        .item_img {
          img {
          }
        }
        .item_text {
          display: flex;
          flex-direction: column;
          justify-content: center;

          .text_name {
            font-weight: bold;
            font-size: 25px;
          }
          .text_descr {
            font-family: Georgia;
            font-size: 1.1875rem;
            line-height: 1.69;
            letter-spacing: 4px;
          }
        }
      }
    }
  }
}
</style>>
