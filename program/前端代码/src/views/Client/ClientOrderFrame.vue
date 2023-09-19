/*
client下订单之类的统一框架，纯蓝底+白色卡片
creator,editor:yy 
 */


<template>
  <div class="page">
    <div class="order_box">
      <router-view></router-view>
    </div>
  </div>
</template>

<script>
export default {
  name: "ClientOrderFrame",
  components: {},
};
</script>

// lang选择less语法，scoped限制该样式只在本文件使用，不影响其他组件
<style lang="less" scoped>
.page {
  width: 100%;
  margin: 0;
  background-color: #81d4fa;
  display: flex;
  justify-content: center;
  min-height: 800px;
  .order_box {
    border-radius: 15px;
    background-color: white;
    width: 50%;
    height: 80%;

    margin-top: 40px;
    margin-bottom: 40px;

    padding-left: 40px;
    padding-right: 40px;
    padding-bottom: 40px;
    padding-top: 20px;
    // 投影
    box-shadow: 5px 5px 5px #888888;
  }
}
</style>>
