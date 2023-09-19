<template>
  <div class="CommonFooter">
    <div class="text_link">
      <span
        class="text_item"
        v-for="(text_item, text_index) in TextLinkData"
        :key="text_index"
        >{{ text_item.text }}
      </span>
    </div>
    <div>_________________________</div>
    <div class="icon_link"></div>
    <div class="copy_right">{{ MsgCopyright }}</div>
  </div>
</template>

<script>
export default {
  name: "CommonFooter",
  data() {
    return {
      MsgCopyright: "Copyright @ 2022",
      TextLinkData: [
        {
          text: "关于",
          url: "",
        },
        {
          text: " 网站",
          url: "",
        },
      ],
      IconLinkData: [
        {
          icon: "",
          url: "",
        },
        {
          icon: "",
          url: "",
        },
      ],
    };
  },
};
</script>

<style lang="less" scoped>
.CommonFooter {
  height: 100%;
  width: 100%;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  color: black;

  .text_link {
    display: flex;
    flex-direction: row;
    align-items: center;
    justify-content: center;
    margin-bottom: -15px;
    font-family: Arial;
    font-weight: bold;
    .text_item {
      letter-spacing: 5px;
      margin-left: 15px;
      margin-right: 15px;
    }
  }
  .icon_link {
  }
  .copy_right {
  }
}
</style>