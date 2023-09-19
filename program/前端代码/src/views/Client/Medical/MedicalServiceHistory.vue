<template>
  <div class="views">
     <el-input
    clearable
    v-model="search"
    size="small"
    placeholder="请输入关键字进行搜索，删除输入则显示初始情况">
    <el-button type="success" slot="append" size="small" @click="clearFilter">清除所有筛选条件</el-button>
  </el-input>
  <el-divider></el-divider>
  <div class="block">
    <el-pagination
      align="center"
      @size-change="handleSizeChange"
      @current-change="handleCurrentChange"
      :current-page="currentPage"
      :page-sizes="[8, 20, 50, 100, this.tableData.length]"
      :page-size="pageSize"
      layout="total, sizes, prev, pager, next, jumper"
      :total="totalNum">
    </el-pagination>
  </div>
    <!-- table -->
    <el-table
    ref="filterTable"
    :data="showTable"
    style="width: 100%"
    :row-class-name="tableRowClassName"
    @sort-change="sortChange">
    <el-table-column
      prop="STARTTIME"
      label="开始日期"
      sortable="custom"
     
      column-key="STARTTIME"
      :filters="[{text: '2022-09', value: '2022-09'}, 
                 {text: '2022-08', value: '2022-08'}, 
                 {text: '2022-07', value: '2022-07'}, 
                 {text: '2022-06', value: '2022-06'},
                 {text: '2022-05', value: '2022-05'}, 
                 {text: '2022-04', value: '2022-04'}]"
      :filter-method="filterDateHandler"
    >
    <template slot-scope="scope">
      <i class="el-icon-time"></i>
      <span style="margin-left: 10px">{{ scope.row.STARTTIME }}</span>
    </template>
    </el-table-column>
    <el-table-column
      prop="TIMESLOT"
      label="时段"
      sortable="custom"
      column-key="TIMESLOT"
      :filters="[{text: '上午', value: '上午'}, 
                 {text: '下午', value: '下午'}]"
      :filter-method="filterTimeHandler"
    >
    </el-table-column>
    <el-table-column
      prop="NAME"
      label="申请医生"
    >
    </el-table-column>
    <el-table-column
      prop="STATUS"
      label="服务状态"
      
      column-key="STATUS"
      :filters="[{text: '未受理', value: '未受理'}, 
                 {text: '进行中', value: '进行中'},
                 {text: '已完成', value: '已完成'}]"
      :filter-method="filterStatusHandler"
    >
    </el-table-column>
    <el-table-column
      prop="ORDERSTATUS"
      label="支付状态"
      column-key="ORDERSTATUS"
      :filters="[{text: '未支付', value: '未支付'}, 
                 {text: '已支付', value: '已支付'}]"
      :filter-method="filterPaymentStatusHandler"
    >
    </el-table-column>
    <el-table-column
      prop="MONEY"
      label="金额"
    >
    </el-table-column>
    <el-table-column
      prop="PHONE"
      label="医生电话"
    >
    </el-table-column>
    <el-table-column align="center" label="医生详情">
      <template slot-scope="scope">
          <el-button
            size="mini"
            type="info"
            @click="gotoDoctorInfo(scope.row)">医生详情</el-button>
    </template>
    </el-table-column>
    <el-table-column align="center" label="撤回申请">
      <template slot-scope="scope">
          <el-button
            size="mini"
            type="danger"
            @click="takeBackApplication(scope.row)">撤回申请</el-button>
    </template>
    </el-table-column>
    <el-table-column width=200 align="center" label="支付操作">
      <template slot-scope="scope">
          <el-button
            size="mini"
            type="primary"
            @click="liveMethod(scope.row)">上门现付</el-button>
          <el-button
            size="mini"
            type="success"
            @click="onlineMethod(scope.row)">线上支付</el-button>
      </template>
    </el-table-column>
  </el-table>
  </div>
  
</template>

<script>
import { AccessTokenFailed } from "../../../api/data.js"
export default {
  name: "ClientManagement",
  components: {},
  data() {
    return {
      access_token: null,
      tableData: [],
      showTableData:[],
      elderid:'',  //002022071200000002
      search:'',
      currentPage: 1,
      pageSize:8,
      totalNum:0
    };
  },
  methods: {
    gotoDoctorInfo(row){
      this._id = row.DOCTORID;
      this.$router.push({
        name: "DoctorUserInfor",
        params: {doctor_id: this._id}
      });
    },
    //根据订单状态，赋予不同的类名
    tableRowClassName({row}) {
        if (row.STATUS === '已拒绝') {
          return 'warning-row';
        } else if (row.STATUS === '已完成' || row.STATUS === '进行中') {
          return 'success-row';
        }
        return '';
      },
      clearFilter() {
        this.$refs.filterTable.clearFilter();
      },
      formatter(row, column) {
        return row.address;
      },
      filterTag(value, row) {
        return row.tag === value;
      },
      filterDateHandler(value, row, column) {
        const property = column['property'];
        return row[property].substr(0,7) === value;
      },
      filterTimeHandler(value, row, column) {
        const property = column['property'];
        return row[property].substr(0,7) === value;
      },
      filterBuildingHandler(value, row, column) {
        const property = column['property'];
        return row[property] === value;
      },
      filterStatusHandler(value, row, column) {
        const property = column['property'];
        return row[property] === value;
      },
      filterPaymentStatusHandler(value, row, column) {
        const property = column['property'];
        return row[property] === value;
      },
      filterPaymentMethodHandler(value, row, column) {
        const property = column['property'];
        return row[property] === value;
      },
      searchResource() {
        this.currentPage = 1; //将当前页设置为1，确保每次都是从第一页开始搜
        const search = this.search;
        if (search) {
          // filter() 方法创建一个新的数组，新数组中的元素是通过检查指定数组中符合条件的所有元素。
          // 注意： filter() 不会对空数组进行检测。
          // 注意： filter() 不会改变原始数组。
          return this.tableData.filter(data => {
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
        return this.tableData;
      },
      handleSizeChange(psize) {
        this.pageSize = psize;
      },
      handleCurrentChange(currentPage) {
        this.currentPage = currentPage;
      },
      onlineMethod(row){
        if(row.ORDERSTATUS === '未支付' && row.STATUS != '未受理' && row.STATUS != '已拒绝'){
           //传递给付款页面的数据
            let bill_data={
              price:"10元",
              bill_name:"社区医生上门服务",
              backPageName:'MedicalServiceHistory', //退出返回到哪个页面
              serviceID:row.SERVICEID,
            };
            //跳转到 付款页面
            this.$router.push({
              name: "ClientPayment",
              params: {bill_data:bill_data},
            });
        }
      },
      liveMethod(row){
        if(row.ORDERSTATUS === '未支付' && row.STATUS != '未受理' && row.STATUS != '已拒绝'){
           console.log('上门现付');
           this.$http
            .put('/API/medical/order?serviceid='+row.SERVICEID+'&method=上门现付',null,{
              headers:{
                TokenValue: this.access_token
              }
            })
            .then((result) => {
            })
            .catch((err) => {
              if(err.message === 'Request failed with status code 500' || err.message === 'Request failed with status code 403'){
                AccessTokenFailed();
              }
              console.log(err);
            });
        }
      },
      takeBackApplication(row){
        if(row.STATUS === '未受理'){
          console.log(row.SERVICEID);
          row.STATUS = '已撤回';
          this.$http
          .delete('/API/medical/all?serviceID='+row.SERVICEID,{
            headers:{
              TokenValue: this.access_token
            }
          })
          .then((result) => {
            console.log('******delete******');
          })
          .catch((err) => {
            if(err.message === 'Request failed with status code 500' || err.message === 'Request failed with status code 403'){
              AccessTokenFailed();
            }
            console.log(err);
          });
        }
        else if(row.STATUS === '已完成'){
          alert('该服务已完成，无法撤回。')
        }
        else if(row.STATUS === '进行中'){
          alert('该服务已被医生受理，无法自主撤回。如需取消服务请致电医生终止服务。')
        }
        else if(row.STATUS === '已拒绝'){
          alert('该服务已被拒绝，无法撤回。')
        }
      },
      sortChange(column) { // column是个形参，具体查看element-ui文档
        this.currentPage = 1 // return to the first page after sorting
        this.totalNum = this.showTableData.length
        //console.log(this.showTableData);
        this.showTableData = this.showTableData.sort(this.sortFun(column.prop, column.order === 'ascending'));
        //console.log(this.showTableData);
        //this.showeTableData = this.tableData; // 排序完显示到第一页
      },
      sortFun (attr, rev) {
        //第一个参数传入info里的prop表示排的是哪一列，第二个参数是升还是降排序
        if (rev == undefined) {
          rev = 1;
        } else {
          rev = (rev) ? 1 : -1;
        }
        return function (a, b) {
          a = a[attr];
          b = b[attr];
          if (a <= b) {
            return rev * -1;
          }
          else if (a > b) {
            return rev * 1;
          }
          return 0;
        }
      }
  },
   mounted() {
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
          .get('/API/medical/elder',{
            params:{
              elderID:this.elderid
            },
            headers:{
              TokenValue: this.access_token
            }
          })
          .then((result) => {
            console.log(result.data);
            this.tableData = result.data; 
            for(var i=0;i<this.tableData.length;i++){
              this.tableData[i].STARTTIME = this.tableData[i].STARTTIME.substr(0,10);
            }
            this.showTableData = this.searchResource(); //在页面挂载后调用此方法，确保数据与页面发生了交互
            this.totalNum = this.tableData.length;
            this.pageSize = this.tableData.length;
          })
          .catch((err) => {
            if(err.message === 'Request failed with status code 500' || err.message === 'Request failed with status code 403'){
              AccessTokenFailed();
            }
            console.log(err.message);
          });
      }
    },
    watch: { //监听搜索框内容，当搜索框内容发生变化时调用searchResource方法
      search: {
        handler() {
          this.showTableData = this.searchResource();
          this.totalNum = this.showTableData.length;
        }
      }
    },
    computed: {
      //showTable计算属性通过slice方法计算表格当前应显示的数据
      showTable() {
        return this.showTableData.slice((this.currentPage - 1) * this.pageSize, this.currentPage * this.pageSize);
      }
    }
};
</script>

<style>
    .el-table .warning-row {
    background: oldlace;
  }

  .el-table .success-row {
    background: #f0f9eb;
  }
</style>

// lang选择less语法，scoped限制该样式只在本文件使用，不影响其他组件
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
}
</style>>