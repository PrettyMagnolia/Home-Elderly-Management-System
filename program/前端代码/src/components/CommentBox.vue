/*一个完整的评价组件
creator:yy
editor:yy 
使用手册：
参考views\Client\CareServiceMine.vue,90行使用该组件、488行配置组件参数 
父组件需要传入 comment_data 参数
该组件自带 提交按钮，点击后emit事件CommentBoxValueToParent，传送数据为评价的所有内容，可在父组件监听该事件，获取数据
*/ 
<template>
  <div class="CommentBox">
    <div class="head_box">
      {{ comment_data.head_text }}
    </div>

    <!-- star部分 -->
    <!-- 可能有多个评分，v-for -->
    <div
      class="star_comment_box"
      v-for="(star_text, star_index) in comment_data.all_star_text"
      :key="star_index"
    >
      <span class="text">{{ star_text }}</span>
      <el-rate
        show-text
        class="star"
        allow-half
        v-on:change="SingleStarChange($event, star_index)"
        v-model="comment_value.star_value[star_index]['value']"
      >
      </el-rate>
    </div>

    <!-- text评价 -->
    <div class="text_comment_box">
      <el-input
        type="textarea"
        placeholder="请输入评价内容"
        v-model="comment_value.textarea"
        maxlength="100"
        show-word-limit
        clearable
        :rows="4"
      >
      </el-input>
    </div>
    <!-- 匿名插槽 -->
    <slot></slot>
    <!-- 提交和匿名按钮 -->
    <div class="submit_button_box">
      <!-- 匿名radio -->
      <el-radio
        v-model="comment_value.is_anonym"
        class="anonym_radio"
        v-if="comment_data.anonym"
        :label="true"
        @click.native.prevent="onRadioChange(true)"
        >匿名评价</el-radio
      >

      <el-button
        class="submit_button"
        type="primary"
        @click="HandleSubmitValue"
        :disabled="comment_data.CanUseComment?false:true"
        >确认评价</el-button
      >
    </div>
  </div>
</template>

<script>
export default {
  name: "CommentBox",
  // 接受来自父组件的数据,
  //对于通过 props 传入的参数，不建议对其进行操作，因为会同时修改父组件里面的数据
  props: {
    //comment_data模板

    comment_data: {
      //js
      type: Object,
      required: true,
      // 默认值
      default: () => {
        return {
          anonym: false, //是否可以选择匿名
          all_star_text: ["评分"], //打星评分的参数数组
          head_text: "评价表格", //表头
          CanUseComment:true,
        };
      },
    },
  },

  data() {
    return {
      //全部的评分，最后是返回这个数组
      //其下标依据传入的text
      comment_value: {
        star_value: [],
        textarea: "",
        is_anonym: false,
      },
    };
  },
  mounted() {
    // 初始化star text
    this.comment_data.all_star_text.forEach((ele) => {
      let data_list = {
        text: ele,
        value: "",
      };
      this.comment_value.star_value.push(data_list);
    });
  },
  methods: {
    //star评分改变，触发change事件
    //rate组件传$event就是value
    SingleStarChange(value, star_index) {
      //   this.comment_value.star_value[star_index]["value"] = value;
      console.log(
        "in child,",
        this.comment_value.star_value[star_index]["text"],
        " value:",
        value
      );
    },

    //提交按钮的事件，把子组件的所有数据提交给父组件
    HandleSubmitValue() {
      this.$emit("CommentBoxValueToParent", this.comment_value);
    },

    //单选框改写
    onRadioChange(e) {
      this.comment_value.is_anonym =
        e === this.comment_value.is_anonym ? false : e;
    },
  },
};
</script>

<style scoped lang="less">
//评价box
.CommentBox {
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  border: solid #ebeef5 1px;

  //   表头
  .head_box {
    background-color: #fafafa;
    width: 100%;
    padding: 8px;
    font-size: 20px;
    font-weight: bold;
  }

  //   star部分
  .star_comment_box {
    display: flex;
    flex-direction: row;
    margin-top: 10px;
    margin-bottom: 10px;

    .text {
      font-size: 18px;
      margin-right: 5px;
    }
    .star /deep/ .el-rate__icon {
      font-size: 30px;
    }
  }

  //text部分
  .text_comment_box {
    width: 100%;

    el-input {
      width: 100%;
      color: red;
    }
  }

  .submit_button_box {
    margin-top: 5px;
    margin-bottom: 5px;
  }
}
</style>