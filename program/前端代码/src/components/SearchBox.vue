<!--
一个搜索框组件
creator:刘一飞
editor:刘一飞
使用手册：
  参考views\Nurse\TakeOrder\TakeOrderApply.vue
  父组件需要传入 input_data 参数，代表着为用户提供输入建议的所有内容，
  
该组件自带 搜索按钮，点击后emit事件SearchBoxValueToParent，传送数据为用户当前在输入框中输入的内容，可在父组件监听该事件，获取数据
-->
<template>
  <el-row>
    <!-- 搜索框部分 -->
    <el-autocomplete
        class="inline-input"
        v-model="state"
        popper-class="my-kqoption"
        :fetch-suggestions="querySearch"
        placeholder="请输入关键词"
        :trigger-on-focus="false"
        @select="handleSelect"
        :popper-append-to-body="false">
    </el-autocomplete>
    &ensp;
    <!-- 搜索按钮部分 -->
    <el-button type="primary" @click="HandleSubmitValue">搜索</el-button>
  </el-row>
</template>

<script>
  export default {
    data() {
      return {
        caretypes: [],
        state: ''
      };
    },
    methods: {
      querySearch(queryString, cb) {
        var caretypes = this.caretypes;
        var results = queryString ? caretypes.filter(this.createFilter(queryString)) : caretypes;
        // 调用 callback 返回建议列表的数据
        cb(results);
      },
      createFilter(queryString) {
        return (restaurant) => {
          return (restaurant.value.toLowerCase().indexOf(queryString.toLowerCase()) === 0);
        };
      },
      handleSelect(item) {
        console.log(item);
      },
      //提交按钮的事件，把子组件的所有数据提交给父组件
      HandleSubmitValue() {
        this.$emit("SearchBoxValueToParent", this.state);
      },
    },
    mounted() {
      this.caretypes = this.input_data;
    },
    props:{
      input_data:{
        //js
        type: Array,
        required: true,
      }
    }
  }
</script>

<style lang="less" scoped>
& /deep/ .my-kqoption {
    width: 200%!important; 
}
</style>>
