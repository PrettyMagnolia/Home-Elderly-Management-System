<template>
  <div id="app">
    <!-- 路由视图 -->
    <router-view></router-view>
  </div>
</template>

<script>
export default {
  name: "App",
  components: {},
  mounted() {
    //绑定监听事件
    window.addEventListener("beforeunload", (e) => this.beforeunloadHandler(e));
    window.addEventListener("unload", (e) => this.unloadHandler(e));
    window.addEventListener("load", (e) => this.loadHandler(e));
  },
  methods: {
    beforeunloadHandler(e) {
      // this.$cookies.remove("token");
      // this.$cookies.set("onbeforeunload", "1");
    },
    unloadHandler(e) {
      // this.$cookies.remove("onbeforeunload");
    },
    loadHandler(e) {
      // this.$cookies.set("on", "1");
    },
  },
  destroyed() {
    //解绑
    window.removeEventListener("beforeunload ", (e) =>
      this.beforeunloadHandler(e)
    );
    window.removeEventListener("unload", (e) => this.unloadHandler());
    window.removeEventListener("load", (e) => this.loadHandler());
  },
};
</script>

<style>
html,
body,
#app {
  height: 100%;
  min-height: 600px;
}
.el-autocomplete-suggestion {
  width: auto!important;
  
}
</style>
