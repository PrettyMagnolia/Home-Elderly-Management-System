<template>
    <div class="placeCareOrder_box">


      <!-- 分步表单 -->
      <div class="step_box" align-center="true">
        <el-steps :active="order_step" finish-status="success">
          <el-step title="核对信息"> </el-step>
          <el-step title="可选医生"></el-step>
          <el-step title="提交成功"></el-step>
        </el-steps>
      </div>

      <!-- 实际表单部分 -->
      <div class="order-form-box">
        <!-- step1 -->
        <el-form
          class="order-form"
          :inline="false" 
          :model="form"
          label-width="80px"
          v-show="order_step == 0"
          :rules="rules"
          ref="form"
        >
          <el-form-item prop="name" label="本人姓名">
            <el-input
              class="input-item"
              v-model="form.name"
              :disabled = true
              style="font-weight: bold"
              
            ></el-input>
          </el-form-item>


          <el-form-item prop="community" label="所在社区">
            <el-col>
              <el-input
              class="input-item"
              v-model="form.community"
              :disabled=true
              style="width:40% ; font-weight: bold"
              
            ></el-input>
            </el-col>
          </el-form-item>
          
          <el-row>
            <el-form-item prop="address" label="详细地址">
            <el-input
              class="input-item"
              :disabled=true
              v-model="form.address"
             
            ></el-input>
          </el-form-item>
          
          <el-form-item prop="phone" label="本人电话">
            <el-input
              class="input-item"
              :disabled=true
              v-model="form.phone"
             
            ></el-input>
          </el-form-item>
          
          <el-form-item prop="helperPhone" label="紧急电话">
            <el-input
              class="input-item"
              v-model="form.helperPhone"
              :disabled= true
              style="font-weight: bold"
              
            ></el-input>
          </el-form-item>
          
          <!-- 价格 -->

          <el-form-item label="服务价格">
            <el-input
              class="input-item"
              v-model="form.price"
              :disabled= true
              style="font-weight: bold"
              
            ></el-input>
          </el-form-item>

          </el-row>
          
          

          <!-- 时间 -->
          <el-row>  
         
             

            
              <el-form-item prop="time_start" label="起始时间">
                <el-date-picker
                  type="date"
                  placeholder="选择日期"
                  v-model="form.time_start"
                  style="width: 40%"
                  :picker-options="timePickerOptions"
                  value-format="yyyy-MM-dd"
                ></el-date-picker>
              </el-form-item>
          
              
            
         
          </el-row>
          <el-row>
            <el-form-item prop="time_end" label="结束时间">
                <el-date-picker
                  type="date"
                  placeholder="选择日期"
                  v-model="form.time_end"
                  style="width: 40%"
                  :picker-options="timePickerOptions"
                  value-format="yyyy-MM-dd"
                ></el-date-picker>
              </el-form-item>
          </el-row>
          <el-row>
             <el-form-item prop="option_value" label="选择时段">
              <el-select v-model="form.option_value" placeholder="请选择">
                   <el-option
                     v-for="item in options"
                     :key="item.value"
                     :label="item.label"
                     :value="item.value">
                    </el-option>
                </el-select>
          </el-form-item> 

          <el-form-item label="既往病史">
             <el-button
               v-on:click="historyDisease"
                 >点击详情</el-button
              >
          </el-form-item>
          </el-row>
         
        </el-form>
        <!--step2  -->
       
         <el-table
               :data="tableData"
               style="width: 100%"
                v-show="order_step == 1">
                <el-table-column
                   prop="NAME"
                   label="医生姓名">
                </el-table-column>
                <el-table-column
                   prop="AGE"
                   label="年龄">
                </el-table-column>
                <el-table-column
                   prop="FIELD"
                   label="擅长领域">
                </el-table-column>
                <el-table-column
                  prop="PHONE"
                  label="医生电话">
                </el-table-column>
                <el-table-column
                   prop="HISTORY"
                   label="经历简述">
                </el-table-column>
                <el-table-column width=200 align="center" label="预约顺序">
                   <template slot-scope="scope">
                     <el-button
                       size="mini"
                       type="primary"
                       @click="moveUp(scope.$index,scope.row)">上移</el-button>
                     <el-button
                       size="mini"
                       type="danger"
                       @click="moveDown(scope.$index,scope.row,1)">下移</el-button>
                    </template>
                </el-table-column>
            </el-table>

       

        <!-- step 3  -->
        <el-form
          class="order-form"
          :model="form"
          v-show="order_step == 2"
          :rules="rules"
          >
        </el-form>
      </div>

      <br />
      <br />

      <!-- 按钮盒子 -->
      <div class="button-box">

        <el-button
          class="step-button"
          v-on:click="back"
          v-show="order_step != 0 && order_step != 3"
          >上一步</el-button
        >

        

        <el-button
          class="step-button"
          v-on:click="exitToService"
          v-show="order_step == 3"
          type="primary"
          >退出</el-button
        >
        <el-button
          class="step-button"
          v-on:click="next"
          type="primary"
          v-show="order_step != 0 && order_step != 2 && order_step != 3"
          >下一步</el-button
        >

        <el-button
          class="step-button"
          v-on:click="gotoUserInfor"
          type="primary"
          v-show="order_step == 0"
          >编辑信息</el-button
        >
        <el-button
          class="step-button"
          v-on:click="getDoctorList"
          type="primary"
          v-show="order_step == 0"
          >下一步</el-button
        >

        <el-button
          class="step-button"
          v-on:click="formSubmit('form')"
          type="primary"
          v-show="order_step == 2"
          >提交</el-button
        >
      </div>



  </div>
</template>

<script>
var forbiddenEdit=true;
import { getDoctorService } from "../../../api/data.js";
import { AccessTokenFailed } from "../../../api/data.js"
export default {
  name: "PlaceCareOrder",
  components: {},
  data() {
    //自定义校验规则
    //日期1
    var validatetime_start = (rule, value, callback) => {
      if (!value) {
        return callback(new Error("请选择起始日期"));
      } else {
        if (this.form.time_start > this.form.time_end) {
          return callback(new Error("起始日期必须早于结束日期"));
        } else return callback();
      }
    };

    //日期2
    var validatetime_end = (rule, value, callback) => {
      if (!value) {
        return callback(new Error("请选择结束日期"));
      } else {
        if (this.form.time_start > this.form.time_end) {
          return callback(new Error("结束日期必须晚于起始日期"));
        } else return callback();
      }
    };
     //时段
    var validatetime = (rule, value, callback) => {
      if (!value) {
        return callback(new Error("请选择时段"));
      } else {
        if (this.form.option_value != '上午09:00-11:00' && this.form.option_value != '下午03:00-05:00') {
          return callback(new Error("请选择时段"));
        } else return callback();
      }
    };
    //电话号码
    var validatePhone = (rule, value, callback) => {
      if (!value) {
        return callback(new Error("请输入电话号码"));
      } else {
        const reg = /^1[3|4|5|7|8][0-9]\d{8}$/;
        const isPhone = reg.test(value);

        value = Number(value); //转换为数字
        if (typeof value === "number" && !isNaN(value)) {
          //判断是否为数字
          value = value.toString(); //转换成字符串
          if (value.length < 0 || value.length > 12 || !isPhone) {
            //判断是否为11位手机号
            callback(new Error("手机号码格式如:138xxxx8754"));
          } else {
            callback();
          }
        } else {
          callback(new Error("请勿输入字符  "));
        }
      }
    };
    return {
      access_token: null,
      timePickerOptions: { 
         disabledDate(date) {
            return date.getTime() < Date.now() - 8.64e7;//如果没有后面的-8.64e7就是不可以选择今天的 
         }
      },
      elderData: {},
      order_step: 0,
      //为配合后端elder-PUT函数
      elderid:'', //002022071200000002
      pwd:"",
      idcardno:"",
      options: [{
          value: '上午09:00-11:00',
          label: '上午09:00-11:00'
        }, {
          value: '下午03:00-05:00',
          label: '下午03:00-05:00'
        }],
      option_value: '',
      //表单模型
      form: {
        name: "",
        gender: "",
        height: "",
        weight: "",
        age: "",
        type: "",
        communityid: "",
        community: "",
        address: "",
        phone: "",
        helperPhone: "",
        option_value: "",
        time_start: "",
        time_end: "",
        price: "10 ¥",
        address: "",
        number_phone: "",
      },
      tableData: [],
      doctorIDList:[],
      //表单检验规则
      rules: {
        time_start: [{ validator: validatetime_start, trigger: "blur" }],
        time_end: [{ validator: validatetime_end, trigger: "blur" }],

        phone: [{ validator: validatePhone, trigger: "blur" }],
        helperPhone: [{ validator: validatePhone, trigger: "blur" }],
        address: [{ required: true, message: "请输入地址", trigger: "blur" }],
        name: [{ required: true, message: "请输入姓名", trigger: "blur" }],
        gender: [{ required: true, message: "请输入性别", trigger: "blur" }],
        height: [{ required: true, message: "请输入身高", trigger: "blur" }],
        weight: [{ required: true, message: "请输入体重", trigger: "blur" }],
        age: [{ required: true, message: "请输入年龄", trigger: "blur" }],
        type: [{ required: true, message: "请输入自理状态", trigger: "blur" }],
        community: [{ required: true, message: "请输入社区", trigger: "blur" }],
        option_value:[{ validator: validatetime, trigger: "blur" }]
      },
    };
  },
  methods: {
    gotoUserInfor(){
      //传递给付款页面的数据
      let jumpBack={
        backName : 'DoctorService'
      };
      //跳转到 付款页面
      this.$router.push({
        name: "UserInfor",
        params: {jumpBack:jumpBack},
      });
    },
    getDoctorList(){
      console.log('获取医生列表');
      if (++this.order_step > 3) this.order_step = 0;
      this.$http
        .get('/API/Doctor/id',{
          params:{
            communityid:this.form.communityid
          },
          headers:{
            TokenValue: this.access_token
          }
        })
        .then((result) => {
          console.log(result.data);
          this.tableData = result.data; 
        })
        .catch((err) => {
          if(err.message === 'Request failed with status code 500' || err.message === 'Request failed with status code 403'){
            AccessTokenFailed();
          }
          console.log(err);
        });
    },
    //step分步表单下一步
    next() {
      if(this.order_step==0) {
        console.log('********************************');
        console.log(this.form.option_value);
      }
      if(this.order_step==1) {
        console.log(this.form.name);
      }
      if (++this.order_step > 3) this.order_step = 0;
    },
    //step下一步
    back() {
      if (--this.order_step < 0) this.order_step = 0;
    },
    //查看既往病史详情
    historyDisease() {
      let jumpBack={
        backName : 'DoctorService'
      };
      //跳转到 页面
      this.$router.push({
        name: "HistoryDisease",
        params: {jumpBack:jumpBack},
      });
    },
    exitToService() {
      this.$router.push({
        name: "MedicalServiceHistory",
      });
    },
    moveUp(index, row) {
      var that = this;
      // console.log('上移', index, row)
      if (index > 0) {
        const upDate = that.tableData[index - 1];
        that.tableData.splice(index - 1, 1);
        that.tableData.splice(index, 0, upDate);
      } else {
        console.log('已经是第一条，不可上移');
      }
    },
    // 下移
    moveDown(index, row) {
      var that = this;
      // console.log('下移', index, row)
      if ((index + 1) === that.tableData.length) {
        console.log('已经是最后一条，不可下移');
      } else {
        const downDate = that.tableData[index + 1];
        that.tableData.splice(index + 1, 1);
        that.tableData.splice(index, 0, downDate);
      }
    },
    //表单提交
    formSubmit(formName) {
      //触发检查
      this.$refs[formName].validate((valid) => {
        if (valid) {
          for(var i=0;i<this.tableData.length;i++){
            this.doctorIDList.push(this.tableData[i].USERID);
          }
          console.log(this.doctorIDList);
          alert("submit!");
          this.order_step++;
          this.forbiddenEdit = true;
          //向后端请求
          this.$http
            .post('/API/medical',{
              "doctorid":this.doctorIDList,
              "elderid":this.elderid,
              "address":this.form.address,
              "starttime":this.form.time_start,
              "endtime":this.form.time_end,
              "timeslot":this.form.option_value.substr(0,2), 
              "evalustatus":'未评价',
              "status":'未受理',
              "orderstatus":'未支付',
              "method":'线上支付',
              "money":'10'
            },{
              headers:{
                TokenValue: this.access_token
              }
            })
            .then((result) => {
               console.log('******success******');
                this.$http
                  .put('/API/Elder/?id='+this.elderid,{
                    "pwd":this.pwd,
                    "name":this.form.name,
                    "idcardno":this.idcardno,
                    "gender":this.form.gender,
                    "age":this.form.age,
                    "phone":this.form.phone, 
                    "height":this.form.height,
                    "weight":this.form.weight,
                    "address":this.form.address,
                    "urgent":this.form.helperPhone,
                    "situation":this.form.type,
                    "communityid":this.form.communityid},{
                      headers:{
                        TokenValue: this.access_token
                      }
                    })
                  .then((result) => {
                    //console.log(result.data[0].ADDRESS);
                  })
                  .catch((err) => {
                    if(err.message === 'Request failed with status code 500' || err.message === 'Request failed with status code 403'){
                      AccessTokenFailed();
                    }
                    console.log(err);
                  });
        })
        .catch((err) => {
          if(err.message === 'Request failed with status code 500' || err.message === 'Request failed with status code 403'){
            AccessTokenFailed();
          }
          console.log(err);
        });
        } else {
          alert("！提交申请单之前请回退至第一步完善订单信息！");
          return false;
        }
      });
    }
  },
  mounted: function () {
    this.access_token = this.$cookies.get("token");
    try{
      //读取cookies内的老人id
      this.elderid = this.$cookies.get("USERID");
    }
    catch(err){
      console.log('从cookie获取ID失败');
      this.elderid = '002022071200000002';
    }
    finally{
       //向后端请求
    this.$http
        .get('/API/elder/',{
          params:{
            id:this.elderid
          },
          headers:{
            TokenValue: this.access_token
          }
        })
        .then((result) => {
          console.log('获取老人信息');
          console.log(result.data.message[0]);
          this.elderData = result.data.message;

          this.form.name = this.elderData[0].NAME;
          this.form.gender = this.elderData[0].GENDER;
          this.form.height = this.elderData[0].HEIGHT;
          this.form.weight = this.elderData[0].WEIGHT;
          this.form.age = this.elderData[0].AGE;
          this.form.type = this.elderData[0].SITUATION;
          this.form.communityid = this.elderData[0].COMMUNITYID;
          this.form.address = this.elderData[0].ADDRESS;
          this.form.phone = this.elderData[0].PHONE;
          this.form.helperPhone = this.elderData[0].URGENT;
          this.pwd = this.elderData[0].PASSWORD;
          this.idcardno = this.elderData[0].IDCARDNO;
          console.log('---------------------------------');
          console.log(this.form.communityid);
          this.$http
            .get('/API/community/id',{
              params:{
                id:this.form.communityid
              },
              headers:{
                TokenValue: this.access_token
              }
            })
            .then((result) => {
              console.log(result.data[0]);
              this.form.community = result.data[0].NAME; 
            })
            .catch((err) => {
              if(err.message === 'Request failed with status code 500' || err.message === 'Request failed with status code 403'){
                AccessTokenFailed();
              }
              console.log(err);
            });
        })
        .catch((err) => {
          if(err.message === 'Request failed with status code 500' || err.message === 'Request failed with status code 403'){
            AccessTokenFailed();
          }
          console.log(err);
        });
    }
   
  },
};
</script>

// lang选择less语法，scoped限制该样式只在本文件使用，不影响其他组件
<style lang="less" scoped>


.placeCareOrder_box{
  height: 100%;
  width: 100%;


    //步骤
    .step_box {
      margin-bottom: 50px;
    }

    // 实际表单
    .order-form-box {
      display: flex;
      .order-form {
        width: 100%;
        height: 100%;
        //禁止输入的样式，样式穿透
        .input-item /deep/.el-input__inner:disabled,
        .textarea-item /deep/.el-textarea__inner:disabled {
          color: black;
          font-weight: bold;
        }
        .input-item {
          width: auto;
        }
      }
    }
    //按钮
    .button-box {
      display: flex;
      justify-content: center;
      .step-button {
      }
    }
}
  

</style>
