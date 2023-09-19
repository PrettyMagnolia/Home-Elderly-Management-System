<!-- Main是用户平台的框架，包含左侧导航栏、顶部、主体部分、页脚 -->
<template>
  <div class="views">
    <el-container style="100%">
      <el-aside class="aside" width="auto">
        <!-- 自定义aside组件 -->
        <common-aside></common-aside>
      </el-aside>

      <el-container
        :class="{
          'right-main-box-fold': true,
          'right-main-box-unfold': isCollapse,
        }"
      >
        <el-header class="header">
          <!-- 自定义header组件 -->
          <common-header></common-header>
        </el-header>

        <!-- <el-main class="main" style="height:610px;flex-grow:1;"> -->
        <el-main :style="{ minHeight: Height + 'px' }">
          <!-- 路由视图 -->
          <router-view></router-view>
        </el-main>

        <el-footer class="footer" height="">
          <!-- 自定义footer组件,默认height 60px -->
          <common-footer></common-footer>
        </el-footer>
      </el-container>
    </el-container>
  </div>
</template>

<script>
import CommonAside from "../components/CommonAside.vue";
import CommonHeader from "../components/CommonHeader.vue";
import CommonFooter from "../components/CommonFooter.vue";
export default {
  name: "Care",
  components: { CommonAside, CommonHeader, CommonFooter },
  data() {
    return {
      Height: 1000,
    };
  },
  computed: {
    isCollapse() {
      return this.$store.state.aside.isCollapse;
    },
  },
  mounted() {
    //动态设置内容高度 让footer始终居底
    this.Height = document.documentElement.clientHeight - 100;
    //监听浏览器窗口变化
    window.onresize = () => {
      this.Height = document.documentElement.clientHeight - 100;
    };
  },
};
</script>

// lang选择less语法，scoped限制该样式只在本文件使用，不影响其他组件
<style lang="less" scoped>
.views {
  //Header
  .header {
    background-color: #ffffff;
    padding: 0;
  }

  //Aside
  .right-main-box-fold {
    display: block;
    position: relative;
    margin-left: 200px;
  }
  .right-main-box-unfold {
    display: block;
    position: relative;
    margin-left: 64px;
  }

  //Main
  .main {
    border-top: solid #b2b2b2 2px;
  }

  //Footer
  .footer {
    width: 100%;
    background-color: #fafcfb;
    margin-left: -8px;
  }
}
</style>>







