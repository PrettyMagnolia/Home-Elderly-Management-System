<template>
  <!-- 通用的header组件 -->
  <div class="header">
    <!-- 左侧部分 -->
    <div class="left_content">
      <el-button
        :icon="getIcon"
        class="tabButton"
        size="mini"
        @click="handleTab"
      ></el-button>

      <span class="header-logo-box">
        <img
          :src="require('../assets/Img/CommonHeader/logo.png')"
          alt="ImgError"
          class="header-logo"
        />
      </span>
    </div>

    <!-- 右侧部分 -->
    <div class="right_content">
      <el-dropdown trigger="click">
        <!-- 触发下拉框 -->
        <span class="el-dropdown-link">
          <span>
            <i class="el-icon-s-custom"></i>
          </span>
        </span>

        <!-- 下拉框部分 -->
        <el-dropdown-menu slot="dropdown">
          <el-dropdown-item @click.native="Personal">个人中心</el-dropdown-item>
          <el-dropdown-item @click.native="ExitLogin">退出</el-dropdown-item>
        </el-dropdown-menu>
      </el-dropdown>
    </div>
  </div>
</template>

<script>
export default {
  name: "CommonHeader",
  computed: {
    // 根据导航栏是否展开，返回对应icon
    getIcon() {
      return this.$store.state.aside.isCollapse
        ? "el-icon-s-unfold"
        : "el-icon-s-fold";
    },
  },
  methods: {
    //个人中心跳转
    Personal() {
      //根据role，跳转不同的个人中心
      let role = this.$cookies.get("role");
      switch (role) {
        case "0":
          this.$router.push({ name: "UserInfor" });
          break;
        case "1":
          this.$router.push({ name: "NurserInfor" });
          break;
        case "2":
          this.$router.push({ name: "DoctorInfor" });
          break;
        case "3":
          this.$router.push({ name: "AdminUserInfor" });
          break;
        default:
          break;
      }
    },
    handleTab() {
      this.$store.commit("collapseMenu");
    },
    //退出登录
    ExitLogin() {
      //message提示
      this.$msgbox
        .confirm("您确定要退出登录？", "注意", {
          confirmButtonText: "确认退出",
          cancelButtonText: "取消",
          type: "warning",
          center: true,
        })
        .then((action) => {
          if (action === "confirm") {
            //确认退出登录
            console.log("confirm quit");
            //清空local
            window.localStorage.removeItem("refresh_token");
            window.localStorage.removeItem("role");
            //清空cookies
            this.$cookies.remove("token");
            this.$cookies.remove("role");
            this.$cookies.remove("USERID");
            //跳转回login
            this.$router.push({
              name: "Login",
            });
          } else {
            //取消，无事发生
          }
        })
        .catch((err) => {
          console.log(err);
        });
    },
  },
};
</script>

<style lang="less" scoped>
.header {
  display: flex;
  // border: 1px solid black;
  align-content: center;
  height: 50px;
  .left_content {
    display: flex;
    flex: 20;
    .tabButton {
      flex: 1;
      height: 30px;
      width: 30px;
      display: flex;
      justify-content: center;
      align-items: center;
    }
    .header-logo-box {
      flex: 1000;
      display: flex;
      align-items: center;
      margin-left: 40%;
      .header-logo {
        height: 80%;
      }
      .header-text {
        height: 100%;
      }
    }
  }
  .right_content {
    flex: 1;
    display: flex;
    align-content: center;

    .el-dropdown-link {
      .el-icon-s-custom {
        font-size: 40px;
      }
    }
  }
}
</style>