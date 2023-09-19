<template>
  <div>
    <el-carousel :interval="4000" type="card" :height="bannerHeight+ 10 + 'px'">
      <el-carousel-item v-for="item in imgList" :key="item.id">
        <img :src="item.idView" class="myimg" alt="Img error" @load="SlideImgLoad" ref="SlideImg">
      </el-carousel-item>
    </el-carousel>
  </div>

</template>

<script>
  export default {
    data() {
      return {
        bannerHeight:"",
        imgList: [
          {id:0,idView:'https://tse2-mm.cn.bing.net/th/id/OIP-C.Qptq-84zE50NC0d2mYfItgHaE-?pid=ImgDet&rs=1'},
          {id:1,idView:'https://tse1-mm.cn.bing.net/th/id/OIP-C.2Tm9_ExVNx93tIxvS-4JYgHaE8?pid=ImgDet&rs=1'},
          {id:2,idView:'https://ts1.cn.mm.bing.net/th/id/R-C.611c9bdef05e24d32291bcaaba54defb?rik=Kzt7KteKrqqThQ&riu=http%3a%2f%2fwww.kongquehe.com%2fUploadFiles%2f2018-10%2f305%2f2018100811332323121.jpg&ehk=NVhobyGl94TLOdFTgqBP%2ba%2fThn4hhp3yDw4qa1S%2bl2c%3d&risl=&pid=ImgRaw&r=0'},
          {id:3,idView:'https://tse3-mm.cn.bing.net/th/id/OIP-C.xfeyymE43FkyU5yeYs0jSAHaE7?pid=ImgDet&rs=1'}            
        ]
      }
    },
    methods:{
      SlideImgLoad() {
        //第一次加载时,获取图片在窗口中的高度,赋值给Slide
        this.$nextTick(() => {
          this.bannerHeight = 600/1550*this.$refs.SlideImg[0].width;
        });
      },
    },
    mounted(){
      this.SlideImgLoad();
      window.onresize=()=>{
        this.bannerHeight = 600/1550*this.$refs.SlideImg[0].width;
      }
    }

  }
</script>


<style>
  .el-carousel__item h3 {
    color: #475669;
    font-size: 14px;
    opacity: 0.75;
    line-height: 200px;
    margin: 0;
  }
  
  .el-carousel__item:nth-child(2n) {
    background-color: #99a9bf;
  }
  
  .el-carousel__item:nth-child(2n+1) {
    background-color: #d3dce6;
  }
  .myimg{
    width:100%;
    height:inherit;
  }
</style>
