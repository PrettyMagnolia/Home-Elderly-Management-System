
<template>
<el-form ref="baseForm" :model="baseForm" :rules="rules" auto-complete="on">
  <div class="views">
    <el-row>
      <el-button type="primary" @click="addLine">增加</el-button>
      <el-button type="primary" @click="submit">提交</el-button>
      <el-button type="success" @click="clickBack" v-show="backIsOk">返回</el-button>
    </el-row>
  </div>
  <el-table
    ref="table-inpu"
    class="table"
    :data="baseForm.demoList"
    highlight-current-row
    @row-click="selectItem"
    style="width: 100%">
    <el-table-column label="疾病名称">
      <template slot-scope="scope">
        <el-form-item :prop="'demoList.'+scope.$index+'.NAME'" :rules="rules.NAME">
          <el-input 
             v-model="scope.row.NAME" 
             placeholder="请输入" 
             clearable 
             @focus="$refs.baseForm.clearValidate(`demoList.${scope.$index}.NAME`)">
          </el-input>
        </el-form-item>
      </template>
    </el-table-column>
    <el-table-column label="患病时间">
      <template slot-scope="scope">
        <el-form-item :prop="'demoList.'+scope.$index+'.STARTTIME'" :rules="rules.STARTTIME">
          <el-date-picker 
            type="date"
            placeholder="请选择" 
            v-model="scope.row.STARTTIME" 
            value-format="yyyy-MM" 
            clearable 
            @focus="$refs.baseForm.clearValidate(`demoList.${scope.$index}.STARTTIME`)">
          </el-date-picker>
        </el-form-item>
      </template>
    </el-table-column>
    <el-table-column prop="" label="操作">
      <template>
        <div>
          <span class="delete-btn">删除</span>
        </div>
      </template>
    </el-table-column>
  </el-table>
  </el-form>
</template>

<script>
  import { AccessTokenFailed } from "../../../api/data.js"
  export default {
    data() {
      return {
      access_token: null,
      jumpBack:{
        backName : '',
      },
      backIsOk : true,
      elderid:'', //002022071200000002
      baseForm:{
      demoList: []
      },
      index: 0,
      rules: {
        NAME: [{
          required: true,
          message: "请输入疾病名称",
          trigger: "blur"}],
        STARTTIME: [{ 
          required: true, 
          message: "请输入患病时间", 
          trigger: "blur" }]
      }
      }
    },

    methods: {
      clickBack(){
        if(this.backIsOk){
          console.log(this.jumpBack.backName);
          this.$router.push({
            name: this.jumpBack.backName,
            params: {},
          });
        }
      },
      setCurrent(row) {
        this.$refs.singleTable.setCurrentRow(row);
      },
      handleCurrentChange(val) {
        this.currentRow = val;
      },
      // 选中某一行修改或移除
      selectItem(row, column, event) {
        this.selectedFundRow = row
          if (event.target.innerText == "删除") {
            this.removeFundBtn(this.selectedFundRow);
          }
      },
      // 删除指定行
        removeFundBtn(params) {
          this.baseForm.demoList = this.baseForm.demoList.filter((ele) => {
            var flag = false
            // 如果不一致，则保留该行
            for (const key in params) {
              if (ele[key] != params[key]) {
                flag = true;
                this.$http
                .delete('/API/history?diseaseID='+params.DISEASEID+
                                   '&elderID='+this.elderid,{
                                      headers:{
                                        TokenValue: this.access_token,
                                      },
                                   })
                .then((result) => {
                  console.log('******success******');
                })
                .catch((err) => {
                  if(err.message === 'Request failed with status code 500' || err.message === 'Request failed with status code 403'){
                    AccessTokenFailed();
                  }
                  //AccessTokenFailed();
                  console.log(err);
                });
                break;
                }
            }
            return flag
          })
          // 如果全部删除后没有可以点击的一行了，需要加一行空行
          if (!this.baseForm.demoList.length) {
            this.addLine()
          }
      },
       // 增加一个空行, 用于录入或显示第一行
      addLine() {
        const newLine = {
          NAME: "",
          STARTTIME: ""};
          this.index++ ;
          this.baseForm.demoList.push(newLine);
      },
      submit() {
        this.$refs.baseForm.validate((valid) => {
          if (valid) {
            this.$confirm("您确定【提交】吗 ？", "提示", {
              confirmButtonText: "确定",
              cancelButtonText: "取消",
              type: "warning"
            }).then(() => {
              console.log("校验通过");
              for(var i=0;i<this.baseForm.demoList.length;i++){
                console.log(this.baseForm.demoList[i].NAME);
                console.log(this.baseForm.demoList[i].STARTTIME.substr(0,7));
                this.$http
                .post('/API/history?diseasename='+this.baseForm.demoList[i].NAME+
                                   '&elderid='+this.elderid+
                                   '&starttime='+this.baseForm.demoList[i].STARTTIME.substr(0,7),null,{
                                      headers:{
                                        TokenValue: this.access_token,
                                      },
                                    }
                                   )
                .then((result) => {
                  console.log('******success******');
                })
                .catch((err) => {
                  if(err.message === 'Request failed with status code 500' || err.message === 'Request failed with status code 403'){
                    AccessTokenFailed();
                  }
                  console.log(typeof(err));
                  //AccessTokenFailed();
                  console.log(err.message);
                });
              }
            })
          }
        })
      }
    },
    mounted: function () {
      this.access_token = this.$cookies.get("token");
      try{
        this.elderid = this.$cookies.get("USERID");
      }
      catch(err){
        console.log('从cookie获取ID失败');
        this.elderid = '002022071200000002';
      }
      finally{
        //向后端请求
        this.$http
          .get('/API/history/id',{
            params:{
              elderid:this.elderid
            },
            headers:{
              TokenValue: this.access_token
            }
          })
          .then((result) => {
            console.log('获取老人既往病史');
            console.log(result.data);
            this.baseForm.demoList = result.data;
          })
          .catch((err) => {
            if(err.message === 'Request failed with status code 500' || err.message === 'Request failed with status code 403'){
              AccessTokenFailed();
            }
            console.log(err);
          });
          this.jumpBack = this.$route.params.jumpBack;
          try{
            console.log(this.jumpBack.backName);
          }
          catch(error){
            this.backIsOk = false
            console.log(this.backIsOk);
          }
          finally{
            if(this.backIsOk){
              console.log(this.jumpBack.backName);
            }
          }
      }
    },
  }
</script>
// lang选择less语法，scoped限制该样式只在本文件使用，不影响其他组件
<style lang="less" scoped>
.delete-btn {
    color: #f56c6c;
  }
</style>
