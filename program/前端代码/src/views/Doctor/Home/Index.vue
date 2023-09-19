<!-- 
    创建人：
    编辑人： 叶登旭 （医护模块）
        7-28完善界面
    该界面是医生的主界面，主要用于信息提醒、信息概要
 -->
<template>
<div class="views">
    <!-- 布局方式 -->
    <el-row gutter="30" v-loading.fullscreen.lock="fullscreenLoading">
        <el-col :span="10">
            <!-- 展示用户简要信息和部分通知 -->
            <el-card class="box_card_show" shadow="hover">
                <el-row>
                    <el-col :span="4">
                        <div class="block"><el-avatar :size="50" src="https://cube.elemecdn.com/0/88/03b0d39583f48206768a7534e55bcpng.png"></el-avatar></div>
                    </el-col>
                    <el-col :span="8">
                        <p id="doctor_name">{{this.doctor_mes.NAME}}</p>
                        <p id="doctor_name">擅长领域:{{doctor_mes.FIELD}}</p>
                    </el-col>
                </el-row>
                <el-tag
                    v-if="doctor_mes.BANTIME===null"
                    type="success"
                    effect="plain">
                    信誉良好，当前账号状态正常
                </el-tag>
                <el-tag
                    v-else
                    type="danger"
                    effect="plain">
                    账号封禁中，{{doctor_mes.BANTIME}}后解封
                </el-tag>
                <!-- 分割线 -->
                <el-divider>便捷入口</el-divider>
                <el-row gutter="20">
                    <el-col :span="6">
                        <div>
                            <el-badge :value="new_value" class="item">
                            <el-button size="small" @click="gotoEvaluation">新的评价</el-button>
                            </el-badge>
                        </div>                           
                    </el-col>     
                    <el-col :span="6">
                        <div>
                            <el-badge :value="new_value" class="item">
                            <el-button size="small" @click="$router.replace('/doctor/medicalOrderApply')">服务申请</el-button>
                            </el-badge>
                        </div>                           
                    </el-col>  
                    <el-col :span="6">
                        <div>
                            <el-badge :value="new_value" class="item">
                            <el-button size="small" @click="$router.replace('/doctor/onlineInteraction')">线上问诊</el-button>
                            </el-badge>
                        </div>                           
                    </el-col>
                    <el-col :span="6">
                        <div>
                            <el-badge :value="new_value" class="item">
                            <el-button size="small" @click="$router.replace('/doctor/healthReport')">健康报告</el-button>
                            </el-badge>
                        </div>                           
                    </el-col>                 
                </el-row>
                <el-row gutter="20">
                    <el-col :span="7">
                        <el-badge :value="new_value" class="item">
                            <el-button size="small" @click="$router.replace('/doctor/doctorInfor')">修改个人信息</el-button>
                            </el-badge>
                    </el-col>
                    <el-col :span="7" offset="5">
                        <div>
                            <el-badge :value="new_value" class="item">
                            <el-button size="small" @click="$router.replace('/doctor/doctorQualification')">提交资质核验</el-button>
                            </el-badge>
                        </div>                           
                    </el-col>
                </el-row>
            </el-card><br><br>  

            <!-- 首页的卡片给医生部分帮助 -->
            <el-card class="box_card_show" shadow="hover" >
                <div slot="header" class="clearfix">
                    <span>医生助手</span>
                </div>
                <div class="text item">
                    <p>各页面功能介绍</p>
                    <p>服务申请受理：接受或拒绝老人的服务申请</p>
                    <p>线上问诊管理：进行发布会议，并查看自己的会议</p>
                    <p>健康报告管理：查看本社区老人的健康报告，编辑或添加</p>
                    <p>查看服务评价：查看老人对自己完成的服务评价</p>
                    <p>申请人员归档：确认是否收款及服务是否完成</p>
                </div>
            </el-card>
        </el-col>
        
        <!-- 日历，显示线上问诊的时间 -->
        <el-col :span="14">
            <el-card id="doctor_date" shadow="hover">
                <el-calendar>
                    <template
                        slot="dateCell"
                        slot-scope="{date, data}">
                        {{ data.day.split('-').slice(1).join('-') }}   
                        <el-row>
                            <el-col :span="24">
                                <span id="data_style">{{my_date(data.day)}}</span>
                            </el-col>
                        </el-row>      
                    </template>
                </el-calendar>
            </el-card>
        </el-col>
    </el-row>
</div>
</template>

<script>

export default {
    name:'DoctorHome',
    components: { },
    data() {
        return {
        // 医生id
        doctor_id:'',
        access_token:'',
        // 医生信息包括姓名、擅长领域等
        doctor_mes: {},

        // 头像数据
        circleUrl: '',
        // 日历中的会议数据
        res_date: [],
        meeting_mes:[],

        new_value:null,
        // 全屏的加载动画
        fullscreenLoading:true,
        }
    },
    created(){
      this.doctor_id=this.$cookies.get("USERID");
      this.access_token = this.$cookies.get("token");
      // 获取医生的姓名和擅长领域信息
      this.$http
        .get('/API/Doctor/id?doctorid='+this.doctor_id,{
          headers: {
            TokenValue: this.access_token,
          },
        })
        .then((res)=>{
            this.doctor_mes=res.data[0];
            console.log("获取的医生信息",this.doctor_mes);
        })
        .catch((err)=>{
          console.log("医生信息获取失败",err);
        });
      // 获取所有的会议信息，用于日历的初始显示
      this.$http
        .get('/API/room/doctor?doctorid='+this.doctor_id,{
          headers: {
            TokenValue: this.access_token,
          },
        })
        .then((res)=>{
            this.meeting_mes=res.data;
            for(var i=0;i<this.meeting_mes.length;i++)
            {
                var tmp=this.meeting_mes[i].STARTTIME.split(' ');
                this.res_date.push({date:tmp[0],content:'线上问诊'});
            }
            this.fullscreenLoading=false;
            console.log("获取的会议信息",this.meeting_mes);
            console.log("处理后的会议数据",this.res_date);
        })
        .catch((err)=>{
          console.log(err);
        });     
    },  
    methods:{

        // 按钮跳转事件
        gotoEvaluation(){
            this.new_value=null;
            console.log("改变按钮值",this.new_value);
            this.$router.replace('/doctor/serviceComment');
        },
        // 日历显示
        my_date(v) {
            console.log(v)
            let len = this.res_date.length
            let res = ""
            for(let i=0; i<len; i++){
                if(this.res_date[i].date == v) {
                    res = this.res_date[i].content
                    break
                }
            }
            return res
        },
    }
 }
</script>

<style lang="less" scoped>
// 代办事项卡片的样式设计
.box_card_show{
    // 设置圆角
    border-radius: 30px;
    // width: 400px;
    // height: 100px;
}

// 医生主页面中的日历样式设计
#doctor_date{
    // 设置圆角
    border-radius: 30px;
    // width: 20px;
    // height: 30px;
}

// 日历中标记的样式
#data_style{
    color: rgb(43, 148, 193);
    font:"PingFang SC";
}

// 新评价与新服务标记样式
.item {
  margin-top: 10px;
  margin-right: 40px;
}

</style>>
