/*
登录、注册、忘记密码页面的框架
 */


<template>
  <div class="page">
    <!-- <div class="text_box">颐养通</div> -->
    <img
      class="logo_img"
      alt="ImgError"
      :src="require('../../assets/Img/Login/logo_white.png')"
    />
    <div class="frame_box">
      <router-view></router-view>
    </div>
  </div>
</template>

<script>
export default {
  name: "LoginFrame",
  components: {},
  data() {
    return {};
  },
};
</script>

// lang选择less语法，scoped限制该样式只在本文件使用，不影响其他组件
<style lang="less" scoped>
.page {
  background: url("../../assets/Img/Login/LoginBackground.png");
  width: 100%;
  // height: 100%;
  min-height: 800px;

  display: flex;
  justify-content: center;
  align-items: center;
  flex-direction: column;
  //   width: 100%;
  //   margin: 0;
  //   background-color: #81d4fa;
  //   display: flex;
  //   justify-content: center;
  .logo_img {
    width: 300px;
    margin-right: 550px;
  }
  .text_box {
    margin-right: 550px;
    color: white;
    font-size: 60px;
    font-family: "Courier New";
  }
  .frame_box {
    border-radius: 15px;
    background-color: white;
    width: auto;
    height: 80%;
    margin-top: 20px;
    margin-bottom: 40px;
    margin-right: 550px;
    padding-left: 80px;
    padding-right: 80px;
    padding-bottom: 40px;
    padding-top: 20px;
    // 投影
    box-shadow: 5px 5px 5px black;
  }
}
</style>>
