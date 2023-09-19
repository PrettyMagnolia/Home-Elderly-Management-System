<!-- 
  创建人：
  编辑人：叶登旭（医护模块）
    7-28重写该界面
  功能：主要用于医生角色下管理，该医生所负责的老人的健康报告
 -->
<template>
  <div class="views">
    <el-row :gutter="20">
      <el-col :span="24">
        <el-card shadow="hover" id="report_card">
          <!-- 正文部分，前面是排版设计 -->
          <el-row :gutter="20"><br>
            <el-col :span="19">
              <!-- 这里是搜索框，筛选指定的姓名 -->
              <el-input
                v-model="search"
                placeholder="请输入姓名进行搜索">
              </el-input>
            </el-col>
          </el-row><br>
          <el-divider></el-divider>
          <!-- 报告折叠面板 -->
          <el-row>
            <el-col :span="22">          
              <el-table 
                @expand-change="expandSelect"
                :row-key='getRowKeys'
                :expand-row-keys="expands"
                :data="all_elder_show"
                style="width:100%"
                v-loading="loading">
                <el-table-column type="index"></el-table-column>
                <el-table-column 
                  type="expand">
                  <template>
                    <!-- 嵌套的子列表展示对应老人的健康报告 -->
                    <el-row :gutter="30">
                      <el-col :offset="3">
                        <el-table :data="report_data" stripe style="width: 100%">
                          <el-table-column label="时间" prop="TIME">
                          </el-table-column>
                          <el-table-column label="医生" prop="DOCTORNAME">
                          </el-table-column>
                          <el-table-column label="操作" fixed="right" >
                            <template slot-scope="scope">
                              <el-button
                                size="mini"
                                @click="viewDetail(scope.row)"><i class="el-icon-view el-icon--right"></i>详情/编辑</el-button>
                            </template>                        
                          </el-table-column>
                        </el-table>
                      </el-col>
                    </el-row>
                  </template>
                </el-table-column>
                <el-table-column label="姓名">
                  <template slot-scope="scope">
                    <i class="el-icon-time"></i>
                    <span style="margin-left: 10px">{{ scope.row.NAME }}</span>
                  </template>
                </el-table-column>
                <el-table-column fixed="right" width="120">
                  <template slot-scope="scope">
                    <el-button 
                      @click="addReport(scope.row)"
                      size="mini"
                      type="primary"
                      :disabled="bantime!=null">
                      <i class="el-icon-view el-icon--right"></i>添加报告
                    </el-button>
                  </template>
                </el-table-column>
              </el-table>
            </el-col>
          </el-row>
        </el-card>
      </el-col>
    </el-row>

    <!-- 添加报告的对话框设计,与编辑报告的对话框基本一致 -->
    <el-dialog
      title="报告单"
      :visible.sync="dialogAddReport"
      width="50%">
      <!-- 表单和report_detail（与v-model相关）绑定获取数据，和rules(与prop相关)绑定进行校验 -->
      <el-form
      :model="returndata" 
      :rules="rules_return"
      ref="returndata"
      label-width="80px">
        <!-- 描述列表 -->
        <el-row>
          <el-col :span="11">
            <el-form-item label="医生" prop="doctor">
              <el-input disabled="true" class="report_input" v-model="report_detail.doctor" :placeholder="this.NAME"></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="11">
            <el-form-item label="时间">
              <el-input disabled="true" class="report_input" v-model="report_detail.time" :placeholder="this.currentdate">
              </el-input>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row label="个人信息"> 
          <el-col :span="8">
            <el-form-item label="姓名">
              <el-input disabled="true" class="report_input" v-model="report_detail.older_name" :placeholder="older_in_row.NAME">
              </el-input>
            </el-form-item>
          </el-col>
          <el-col :span="7">
            <el-form-item label="年龄">
              <el-input disabled="true" class="report_input" v-model="report_detail.age" :placeholder="older_in_row.AGE">
              </el-input>
            </el-form-item>
          </el-col>
          <el-col :span="7">
            <el-form-item label="性别">
              <el-input disabled="true" class="report_input" v-model="report_detail.sex" :placeholder="older_in_row.GENDER">
              </el-input>
            </el-form-item>
          </el-col>          
        </el-row>
        <el-row>
          <el-col :span="7">
            <el-form-item label="身高">
              <el-input disabled="true" class="report_input" v-model="report_detail.height" :placeholder="older_in_row.HEIGHT">
              </el-input>
            </el-form-item>
          </el-col>
          <el-col :span="7">
            <el-form-item label="体重">
              <el-input disabled="true" class="report_input" v-model="report_detail.weight" :placeholder="older_in_row.WEIGHT">
              </el-input>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="自理情况">
              <el-input disabled="true" class="report_input" v-model="report_detail.status" :placeholder="older_in_row.SITUATION">
              </el-input>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="11">
            <el-form-item label="心率" prop="rate">
              <el-input class="report_input" v-model="returndata.rate" 
                placeholder="75">
              </el-input>
            </el-form-item>
          </el-col>
          <el-col :span="11">
            <el-form-item label="血压" prop="pressure">
              <el-input class="report_input" v-model="returndata.pressure" 
                placeholder="110">
              </el-input>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="11">
            <el-form-item label="健康评估" prop="healthassess">
              <el-input class="report_input" v-model="returndata.healthassess" 
              placeholder="不错">
              </el-input>
            </el-form-item>
          </el-col>
          <el-col :span="11">
            <el-form-item label="建议" prop="advice">
              <el-input class="report_input" v-model="returndata.advice" 
              placeholder="多喝水">
              </el-input>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-form-item>
            <el-row>
              <el-col :span="9" offset='2'>
                <el-button @click="dialogAddReport = false">取 消</el-button>
              </el-col>
              <el-col :span="10" offset="3">
                <el-button type="primary" @click="addReport_submit()">提 交</el-button>
              </el-col>
            </el-row>
          </el-form-item>
        </el-row>
      </el-form>
    </el-dialog>

    <!-- 编辑/查看报告的对话框设计 -->
    <el-dialog
      title="报告单"
      :visible.sync="dialogEditReport"
      width="50%"
      :before-close="handleClose">
      <el-form ref="form" :model="report_detail" label-width="80px">
      <!-- 启动编辑的按钮 -->
        <el-row>
          <el-col :span="4">
            <el-button :disabled="bantime!=null" v-if="show==false&&doctor_mes.doctor_id==DOCTORID" type="warning" size="small" @click="StartEdit">编辑</el-button>
            <el-button v-if="show==true&&doctor_mes.doctor_id==DOCTORID" type="warning" size="small" @click="EditCommit">提交</el-button>
          </el-col>
          <el-col :span="4" v-show=show>
            <el-button type="warning" size="small" @click="ExitEdit">取消</el-button>
          </el-col>
        </el-row><br>
        <!-- 描述列表 -->
        <el-row>
          <el-col :span="11">
            <el-form-item label="医生">
              <el-input disabled="true" class="report_input" v-model="report_detail.doctor" :placeholder="doctor_mes.doctor_name">
              </el-input>
            </el-form-item>
          </el-col>
          <el-col :span="11">
            <el-form-item label="时间">
              <el-input disabled="true" class="report_input" v-model="report_detail.time" :placeholder="doctor_mes.time">
              </el-input>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row label="个人信息"> 
          <el-col :span="8">
            <el-form-item label="姓名">
              <el-input disabled="true" class="report_input" v-model="report_detail.NAME" :placeholder="report_detail.NAME">
              </el-input>
            </el-form-item>
          </el-col>
          <el-col :span="6">
            <el-form-item label="年龄">
              <el-input disabled="true" class="report_input" v-model="report_detail.age" :placeholder="report_detail.AGE">
              </el-input>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="性别">
              <el-input disabled="true" class="report_input" v-model="report_detail.sex" :placeholder="report_detail.GENDER">
              </el-input>
            </el-form-item>
          </el-col>          
        </el-row>
        <el-row>
          <el-col :span="7">
            <el-form-item label="身高">
              <el-input disabled="true" class="report_input" v-model="report_detail.height" :placeholder="report_detail.HEIGHT">
              </el-input>
            </el-form-item>
          </el-col>
          <el-col :span="7">
            <el-form-item label="体重">
              <el-input disabled="true" class="report_input" v-model="report_detail.weight" :placeholder="report_detail.WEIGHT">
              </el-input>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="自理情况">
              <el-input disabled="true" class="report_input" v-model="report_detail.status" :placeholder="report_detail.SITUATION">
              </el-input>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="11">
            <el-form-item label="心率">
              <el-input :disabled=readonly class="report_input" v-model="report_detail.rate" :placeholder="report_detail.RATE">
              </el-input>
            </el-form-item>
          </el-col>
          <el-col :span="11">
            <el-form-item label="血压">
              <el-input :disabled=readonly class="report_input" v-model="report_detail.pressure" :placeholder="report_detail.PRESSURE">
              </el-input>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="11">
            <el-form-item label="健康评估">
              <el-input :disabled=readonly class="report_input" v-model="report_detail.healthassess" :placeholder="report_detail.HEALTHASSESS">
              </el-input>
            </el-form-item>
          </el-col>
          <el-col :span="11">
            <el-form-item label="建议">
              <el-input :disabled=readonly class="report_input" v-model="report_detail.advice" :placeholder="report_detail.ADVICE">
              </el-input>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="21" offset="1">
            <el-card shadow="hover">
              <div slot="header" class="clearfix">
                <span>既往疾病</span>
              </div>
              <div v-if="hisdisea.length==0">空</div>
              <div v-for="item in hisdisea" class="text item">
                <div>{{item.STARTTIME}}&nbsp;&nbsp;&nbsp;{{item.NAME}}</div>
              </div>
            </el-card>
          </el-col>
        </el-row>
      </el-form>

      <span slot="footer" class="dialog-footer">
        <el-button v-if="show==false" @click="dialogEditReport = false">取 消</el-button>
        <el-button v-if="show==false" type="primary" @click="dialogEditReport = false">关 闭</el-button>
      </span>
    </el-dialog>
  </div>
</template>

<script>
import { AccessTokenFailed } from "../../../api/data.js"
export default {
  name: "HealthReport",
  components: {},
  data() {
    return {
      // 医生id，在进入页面时便可获取
      DOCTORID:'',
      // 医生姓名
      NAME:'',
      // 医生所属社区
      COMMUNITYID:'',
      // 当前日期
      currentdate:'',
      // 设置输入框的禁用与启用
      readonly:true,
      // 编辑启动后的取消按钮
      show:false,
      // 获取row的key值
      getRowKeys(row) {
        return row.USERID;
      },
      // 要展开的行，数值的元素是row的key值
      expands: [],
      // 添加报告单的规则限制
      rules_return:{
        rate:[
          {required:true,message:'请输入老人心率',trigger:'blur'},],
        pressure:[
          {required:true,message:'请输入老人血压',trigger:'blur'}],
        healthassess:[
          {required:true,message:'请输入对老人的健康评估',trigger:'blur'}],
        advice:[
          {required:true,message:'请输入对老人的建议',trigger:'blur'}],
      },
      // 编辑/查看报告单初始化
      dialogEditReport:false,

      // 临时保存健康报告编写医生的部分信息，在点开详情按钮时更新
      doctor_mes:{
        doctor_id:'',
        doctor_name:'',
        time:''
      },

      // 添加报告弹窗初始化
      dialogAddReport:false,

      // 所有老人的信息
      all_elder:[],
      all_elder_show:[],
      // 健康报告的数据
      report_data:[],

      // 健康报告的详细数据
      report_detail:{},

      // 本行老人的信息
      older_in_row:{},
      // 本行老人的既往病史,在展开时获取
      hisdisea:[],

      // 是否加载
      loading:false,

      // 搜索框（选择器）的选项
      options: [{
          value: '选项1',
          label: '张三'
        }, {
          value: '选项2',
          label: '李四'
        }, {
          value: '选项3',
          label: '臧三'
        }, {
          value: '选项4',
          label: '带娃'
        }, {
          value: '选项5',
          label: '丰收'
        }],
      value: '',

      // 添加报告时返回的数据
      returndata:{
        doctorid:'',
        elderid:'',
        rate:'', 
        pressure:'',
        healthassess:'',
        advice:'',
        historyassess:'',
        status:'已提交'
      },

      // 临时存储本行的报告单id用于EditCommit函数
      reportidtmp:'',
      access_token:'',
      bantime:'',

        // 搜索框的内容
        search:'',
        // 分页的参数初始设置
        currentPage:1,
        pagesize:8,
        totalNum:0,
    };
  },

  watch: { //监听搜索框内容，当搜索框内容发生变化时调用searchResource方法
      search: {
        handler() {
          this.all_elder_show= this.searchResource();
          this.totalNum = this.all_elder.length;
        }
      }
    },

  computed: {
    //showTable计算属性通过slice方法计算表格当前应显示的数据
    showTable() {
      return this.all_elder_show.slice((this.currentPage - 1) * this.pageSize, this.currentPage * this.pageSize);
    }
  },

  created(){
    this.access_token = this.$cookies.get("token");
    this.DOCTORID=this.$cookies.get("USERID");
    this.loading=true;
    var date = new Date();
    var year = date.getFullYear();
    var month = date.getMonth() + 1;
    var strDate = date.getDate();
    if (month >= 1 && month <= 9) {
      month = "0" + month;
    }
    if (strDate >= 0 && strDate <= 9) {
      strDate = "0" + strDate;
    }
    this.currentdate = year + "-" + month + "-" + strDate;
    const _this=this;
    // 获取医生的姓名和所属社区
    this.$http
      .get('/API/Doctor/id?doctorid='+_this.DOCTORID,{
          headers: {
            TokenValue: this.access_token,
          },
        })
      .then((res)=>{
        _this.NAME=res.data[0].NAME;
        _this.COMMUNITYID=res.data[0].COMMUNITYID;
        this.bantime=res.data[0].BANTIME;
        console.log(res);
      })
      .catch((err)=>{
        console.log(err);
        if(err.message === 'Request failed with status code 500' || err.message === 'Request failed with status code 403'){
                    AccessTokenFailed();
                  }
      });
    // 获取所有老人
    this.$http
      .get('/API/Elder',{
          headers: {
            TokenValue: this.access_token,
          },
        })
      .then((res)=>{
        this.all_elder=res.data.message;

        //在页面挂载后调用此方法，确保数据与页面发生了交互
        this.all_elder_show = this.searchResource(); 
        this.totalNum=this.all_elder.length;
        this.pageSize=this.all_elder.length;

        this.loading=false;
        console.log("所有老人信息",res);
      })
      .catch((err)=>{
        console.log(err);
        if(err.message === 'Request failed with status code 500' || err.message === 'Request failed with status code 403'){
                    AccessTokenFailed();
                  }
      });  
  },

  methods: {

    // 处理搜索框的内容
    searchResource() {
      this.currentPage = 1; //将当前页设置为1，确保每次都是从第一页开始搜
      const search = this.search;
      if (search) {
        // filter() 方法创建一个新的数组，新数组中的元素是通过检查指定数组中符合条件的所有元素。
        // 注意： filter() 不会对空数组进行检测。
        // 注意： filter() 不会改变原始数组。
        return this.all_elder.filter(data => {
          // some() 方法用于检测数组中的元素是否满足指定条件;
          // some() 方法会依次执行数组的每个元素：
          // 如果有一个元素满足条件，则表达式返回true , 剩余的元素不会再执行检测;
          // 如果没有满足条件的元素，则返回false。
          // 注意： some() 不会对空数组进行检测。
          // 注意： some() 不会改变原始数组。
          return Object.keys(data).some(key => {
            // indexOf() 返回某个指定的字符在某个字符串中首次出现的位置，如果没有找到就返回-1；
            // 该方法对大小写敏感！所以之前需要toLowerCase()方法将所有查询到内容变为小写。
            return String(data[key]).toLowerCase().indexOf(search) > -1
          })
        })
      }
      return this.all_elder;
    },
    // 编辑后提交
    EditCommit(){
      this.returndata.doctorid=this.DOCTORID;
      this.returndata.elderid=this.older_in_row.USERID;
      this.returndata.advice=this.report_detail.advice;
      this.returndata.rate=this.report_detail.rate;
      this.returndata.pressure=this.report_detail.pressure;
      this.returndata.healthassess=this.report_detail.healthassess;
      console.log("修改数据",this.returndata);
      // 发送信息
      this.$http
        .put("/API/health?reportid="+this.reportidtmp,this.returndata,{
          headers: {
            TokenValue: this.access_token,
          },
        })
        .then((res)=>{
          console.log(res);
          this.$message({
            message: '修改成功',
            type: 'success'
          });
        })
        .catch((err)=>{
          console.log(err);
          if(err.message === 'Request failed with status code 500' || err.message === 'Request failed with status code 403'){
                    AccessTokenFailed();
                  }
        });
      this.show=false;
      this.readonly=true;
      this.dialogEditReport=false;
    },

    // 折叠面板每次只能展开一行
    expandSelect (row, expandedRows) {
      this.older_in_row=row;
      this.loading=true;
      if (!this.expands.includes(row.USERID)) {
        this.expands=[];
      }
      // 获取该老人的既往病史
      this.$http
        .get("/API/history/id?elderid="+row.USERID,{
          headers: {
            TokenValue: this.access_token,
          },
        })
        .then((res)=>{
          this.hisdisea=res.data;
          console.log("该老人的既往病史",this.hisdisea);
        })
        .catch((err)=>{
          console.log(err);
          AccessTokenFailed();
        });  
      // 获取该老人所有的健康报告
      this.$http
        .get('/API/health/elder?elderid='+row.USERID,{
          headers: {
            TokenValue: this.access_token,
          },
        })
        .then((res)=>{
            //只展开一行
            if (this.expands.includes(row.USERID)) {
              this.expands = this.expands.filter(val => val !== row.USERID);
            } 
            else {
              this.expands.push(row.USERID);
            }
            this.report_data=res.data;
            console.log('该老人所有健康报告',this.report_data);
            this.loading=false;
        })
        .catch((err)=>{
          console.log(err);
          AccessTokenFailed();
        }); 
    },

    // 健康报告编辑/查看按钮
    viewDetail(row){
      this.doctor_mes.doctor_id=row.DOCTORID;
      this.doctor_mes.doctor_name=row.DOCTORNAME;
      this.doctor_mes.time=row.TIME;
      this.reportidtmp=row.REPORTID;
      // 根据本行的健康报告id获取详细信息
      this.$http
        .get("/API/health/reportid?reportid="+row.REPORTID,{
          headers: {
            TokenValue: this.access_token,
          },
        })
        .then((res)=>{
          this.report_detail=res.data[0];
          console.log("健康报告的信息数据",res);
        })
        .catch((err)=>{
          console.log(err);
          if(err.message === 'Request failed with status code 500' || err.message === 'Request failed with status code 403'){
                    AccessTokenFailed();
                  }
        });   
      this.dialogEditReport=true; 
    },

    // 取消编辑
    ExitEdit(){
      this.readonly=true;
      this.show=false;
      this.$message({
        type: 'info',
        message: '已取消!'
      });
    },

    // 对话框启动编辑的按钮事件
    StartEdit(){
      this.readonly=false;
      this.show=true;
    },

    // 启动添加报告单的弹窗
    addReport(row){
      this.older_in_row=row;
      console.log(this.older_in_row);
      this.dialogAddReport=true;
    },

    // 添加报告单后的提交按钮
    addReport_submit(){
      this.returndata.doctorid=this.DOCTORID;
      this.returndata.elderid=this.older_in_row.USERID;
      console.log("返回数据",this.returndata);
      this.$refs.returndata.validate((valid)=>{
        if(valid){
          // 发送信息
          this.$http
            .post('/API/health',this.returndata,{
              headers: {
                TokenValue: this.access_token,
              },
            })
            .then((res)=>{
              console.log("添加成功的返回信息",res);
              this.$message({
                message: '添加成功',
                type: 'success'
              }); 
            })
            .catch((err)=>{
              this,$message.error("添加失败");
              console.log(err);
              if(err.message === 'Request failed with status code 500' || err.message === 'Request failed with status code 403'){
                    AccessTokenFailed();
                  }
            });
            this.dialogAddReport=false;
        }else{
          this.$message.error('请填写完整');
          return false;
        }
      })
    },

    // 以下5个均为已上传/未上传状态标签的选择器
    resetDateFilter() {
      this.$refs.filterTable.clearFilter('date');
    },
    clearFilter() {
      this.$refs.filterTable.clearFilter();
    },
    formatter(row) {
      return row.address;
    },
    filterTag(value, row) {
      return row.status === value;
    },
    filterHandler(value, row, column) {
      const property = column['property'];
      return row[property] === value;
    }
  },
};
</script>

<style lang="less" scoped>
.views {
  //标签
  .tabs {
  }
  //表格
  .order_table {
    width: 100%;

    //三种状态的颜色
    // /deep/.placed-row {
    //   background-color: #aed581;
    // }
    // /deep/.underway-row {
    //   background-color: #b3e5fc;
    // }
    // /deep/.completed-row {
    //   background: #fcf5e6;
    // }
  }

  // 报告表的样式
  #report_card{
    // 设置圆角
    border-radius: 30px;
  }
}
</style>>
