
<template>
<div class="views">
    
<router-view></router-view>

</div>
</template>

<script>

export default {
    name:'Client',
    components: { },
    data() {
        return {
            
        };
    },
    mounted:function(){
        //导航栏数据配置为Client的
        this.$store.commit('clearAsideData');
        this.$store.commit('SetClientAsideData');
        // console.log("进入 Client 路由");
    }
 }
</script>

<!-- lang选择less语法，scoped限制该样式只在本文件使用，不影响其他组件 -->
<style lang="less" scoped>

</style>>
