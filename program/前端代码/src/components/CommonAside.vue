// 通用的侧边栏组件 

<template>
  <el-menu
    default-active="1-4-1"
    class="el-menu-vertical-demo"
    :unique-opened="true"
    :collapse="isCollapse"
    background-color="#00a497"
    text-color="#fff"
    active-text-color="#000"
  >
    <!-- 标题 -->

    <h3 class="headerText" v-if="!isCollapse">{{ aside_title }}</h3>

    <!-- 根据menu数据，动态渲染一级的noChildren目录 -->
    <el-menu-item
      @click="clickMenu(item)"
      v-for="item in noChildren"
      :index="item.path"
      :key="item.path"
    >
      <i :class="'el-icon-' + item.icon" style="color: #ffc107"></i>
      <template slot="title">
        <span slot="title">{{ item.label }}</span>
      </template>
    </el-menu-item>

    <!-- 根据menu数据，动态渲染hasChildren目录 -->
    <el-submenu v-for="item in hasChildren" :index="item.path" :key="item.name">
      <!-- 1级目录 -->
      <template slot="title">
        <i :class="'el-icon-' + item.icon" style="color: #ffc107"></i>
        <span slot="title">{{ item.label }}</span>
      </template>

      <!-- 2级目录 -->
      <el-menu-item-group v-for="subItem in item.children" :key="subItem.path">
        <el-menu-item :index="subItem.path" @click="clickMenu(subItem)">
          {{ subItem.label }}
        </el-menu-item>
      </el-menu-item-group>
    </el-submenu>
  </el-menu>
</template>


<script>
export default {
  name: "CommonAside",
  data() {
    return {};
  },

  methods: {
    // handleOpen(key, keyPath) {
    //   console.log(key, keyPath);
    // },
    // handleClose(key, keyPath) {
    //   console.log(key, keyPath);
    // },

    // 菜单点击事件，编程式路由
    clickMenu(item) {
      this.$router.push({
        name: item.name,
      });
    },
  },

  //计算属性
  computed: {
    // 返回menu中，有children的项
    noChildren() {
      return this.menu.filter((item) => {
        return !item.children;
      });
    },
    // 返回menu中，无children的项
    hasChildren() {
      return this.menu.filter((item) => {
        return item.children;
      });
    },
    isCollapse() {
      return this.$store.state.aside.isCollapse;
    },
    menu() {
      return this.$store.state.aside.aside_data;
    },
    aside_title() {
      return this.$store.state.aside.aside_title;
    },
  },

  //监听属性
  watch: {
    menu: {
      handler(new_val, old_val) {
        // console.log(new_val, old_val);
      },

      immediate: true,
    },
  },
};
</script>

<style lang="less" scoped>
// 展开的导航样式
.el-menu-vertical-demo:not(.el-menu--collapse) {
  width: 200px;
  min-height: 100vh;
  .title {
    color: white;
    display: flex;
    justify-content: center;
  }
  .headerText {
    color: white;
    display: flex;
    justify-content: center;
  }
}
//折叠的导航样式
.el-menu-vertical-demo:is(.el-menu--collapse) {
  min-height: 100vh;
}

//通用的样式
.el-menu-vertical-demo {
  font-weight: bold;
  position: fixed;
  z-index: 1000;
  left: 0;
  top: 0;
}
</style>
