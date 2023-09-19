<template>
<div>
  <el-input
    clearable
    v-model="search"
    size="small"
    placeholder="请输入关键字进行搜索，删除输入则显示初始情况">
    <el-button slot="append" size="mimi" icon="el-icon-search"></el-button>
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
  
  <el-table
    ref="filterTable"
    :data="showTable"
    style="width: 100%"
    :row-class-name="tableRowClassName"
    @sort-change="sortChange">
    <el-table-column type="expand">
      <template slot-scope="props">
        <el-form label-position="left" inline class="demo-table-expand">
          <el-form-item label="性别">
            <span>{{ props.row.GENDER }}</span>
          </el-form-item>
          <el-form-item label="自理状态">
            <span>{{ props.row.SITUATION }}</span>
          </el-form-item>
           <el-form-item label="本人电话">
            <span>{{ props.row.PHONE }}</span>
          </el-form-item>
          <el-form-item label="紧急电话">
            <span>{{ props.row.URGENT }}</span>
          </el-form-item>
        </el-form>
      </template>
    </el-table-column>
    <el-table-column
      label="申请时间"
      prop="STARTTIME"
      sortable="custom"
     
      column-key="STARTTIME"
      :filters="[{text: '2022-09', value: '2022-09'}, 
                 {text: '2022-08', value: '2022-08'}, 
                 {text: '2022-07', value: '2022-07'}, 
                 {text: '2022-06', value: '2022-06'},
                 {text: '2022-05', value: '2022-05'}, 
                 {text: '2022-04', value: '2022-04'}]"
      :filter-method="filterDateHandler">
    </el-table-column>
    <el-table-column
      label="结束时间"
      prop="ENDTIME"
      sortable="custom"
     
      column-key="ENDTIME"
      :filters="[{text: '2022-09', value: '2022-09'}, 
                 {text: '2022-08', value: '2022-08'}, 
                 {text: '2022-07', value: '2022-07'}, 
                 {text: '2022-06', value: '2022-06'},
                 {text: '2022-05', value: '2022-05'}, 
                 {text: '2022-04', value: '2022-04'}]"
      :filter-method="filterDateHandler">
    </el-table-column>
    <el-table-column
      label="申请人"
      prop="NAME">
    </el-table-column>
    <el-table-column
      label="年龄"
      prop="AGE">
    </el-table-column>
    <el-table-column
      label="时段"
      prop="TIMESLOT">
    </el-table-column>
    <el-table-column
      label="服务地址"
      prop="ADDRESS">
    </el-table-column>
    <el-table-column
      label="申请状态"
      prop="STATUS"
      column-key="STATUS"
      :filters="[{text: '未受理', value: '未受理'}, 
                 {text: '已拒绝', value: '已拒绝'},
                 {text: '已同意', value: '已同意'}]"
      :filter-method="filterStatusHandler">
    </el-table-column>
    <el-table-column>
      <template slot-scope="scope">
        <el-popover
          trigger="click">
          <el-table :data="elderHistoryDisease">
            <el-table-column property="NAME" label="疾病名称"></el-table-column>
            <el-table-column property="STARTTIME" label="患病时间"></el-table-column>
          </el-table>
          <el-button slot="reference"  size="small" type="primary" @click="getHistoryDisease(scope.row)">既往病史</el-button>
        </el-popover>
      </template>
    </el-table-column>
     <el-table-column width=200 align="center">
      <template  slot="header">
         <el-button size="small" type="warning" @click="clearFilter">清除所有筛选条件</el-button>
      </template>
      <template slot-scope="scope">
        <el-button
          size="mini"
          type="success"
          @click="handleAgree(scope.row)">同意</el-button>
        <el-button
          size="mini"
          type="danger"
          @click="handleRefuse(scope.row)">拒绝</el-button>
      </template>
    </el-table-column>
  </el-table>
</div>

</template>


<script>
  import { AccessTokenFailed } from "../../../api/data.js"
  export default {
    data() {
      return {
        ban:false,
        access_token: null,
        elderHistoryDisease:[],
        tableData: [],
        showTableData:[],
        search:'',
        currentPage: 1,
        pageSize:8,
        totalNum:0,
        doctorid:'' //102022082300000001
      }
    },
    methods: {
      getHistoryDisease(row){
        if(this.ban === true){
          alert('您已被封号，失去权限');
        }
        else{
          console.log(row.ELDERID);
          this.$http
            .get('/API/history/id?elderid='+row.ELDERID,{
              headers:{
                TokenValue: this.access_token
              }
            })
            .then((result) => {
              console.log(result.data);
              this.elderHistoryDisease = result.data;
            })
            .catch((err) => {
              if(err.message === 'Request failed with status code 500' || err.message === 'Request failed with status code 403'){
                AccessTokenFailed();
              }
              console.log(err);
            });
        }
      },
      tableRowClassName({row}) {
        if (row.STATUS === '已拒绝') {
          return 'warning-row';
        } else if (row.STATUS === '进行中') {
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
        console.log("look here233");
        console.log(row[property].substr(0,7) === value);
        return row[property].substr(0,7) === value;
      },
      filterBuildingHandler(value, row, column) {
        const property = column['property'];
        return row[property].substr(0,2) === value;
      },
      filterStatusHandler(value, row, column) {
        const property = column['property'];
        return row[property] === value;
      },
      handleAgree(row) {
        if(this.ban === true){
          alert('您已被封号，失去权限');
        }
        else{
          if(row.STATUS === '未受理'){
            row.STATUS = '进行中';
            this.$http
            .put('/API/medical/agree?serviceid='+row.SERVICEID,null,{
              headers:{
                TokenValue: this.access_token
              }
            })
            .then((result) => {
            })
            .catch((err) => {
              if(err.message === 'Request failed with status code 500' || err.message === 'Request failed with status code 403'){
                console.log('同意成功---静置一小时token过期测试错误信息看这里');
                console.log(err);
                console.log(err.message);
                AccessTokenFailed();
              }
              console.log('同意失败---静置一小时token过期测试错误信息看这里');
              console.log(err);
              console.log(err.message);
            });
          }
        }
      },
      handleRefuse(row) {
        if(this.ban === true){
          alert('您已被封号，失去权限');
        }
        else{
          if(row.STATUS === '未受理'){
            row.STATUS = '已拒绝';
            this.$http
            .delete('/API/medical?serviceID='+row.SERVICEID,{
              headers:{
                TokenValue: this.access_token
              }
            })
            .then((result) => {
            })
            .catch((err) => {
              if(err.message === 'Request failed with status code 500' || err.message === 'Request failed with status code 403'){
                console.log('拒绝成功---静置一小时token过期测试错误信息看这里');
                console.log(err);
                console.log(err.message);
                AccessTokenFailed();
              }
              console.log('拒绝失败---静置一小时token过期测试错误信息看这里');
              console.log(err);
              console.log(err.message);
            });
          }
        }
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
          if (a < b) {
            return rev * -1;
          }
          if (a > b) {
            return rev * 1;
          }
          return 0;
        }
      }

    },
    mounted() {
      this.access_token = this.$cookies.get("token");
      try{
        this.doctorid = this.$cookies.get("USERID");
      }
      catch(err){
        console.log('从cookie获取ID失败');
        this.doctorid = '102022082300000001';
      }
      finally{
        this.$http
          .get('/API/medical/doctor?doctorID='+this.doctorid+'&status=0',{
            headers:{
              TokenValue: this.access_token
            }
          })
          .then((result) => {
            this.tableData = result.data;
            this.showTableData = this.searchResource(); //在页面挂载后调用此方法，确保数据与页面发生了交互
            this.totalNum = this.tableData.length;
            this.pageSize = this.tableData.length;
              // 由医生id获取医生的姓名
              this.$http
                .get('/API/Doctor/id?doctorid='+this.doctorid,{
                  headers: {
                    TokenValue: this.access_token,
                  },
                })
                .then((res)=>{
                  if(res.data[0].BANTIME != null){
                    this.ban = true;
                  }
                  console.log("医生账户状态",this.ban);
                })
                .catch((err)=>{
                  console.log(err);
                  if(err.message === 'Request failed with status code 500' || err.message === 'Request failed with status code 403'){
                    AccessTokenFailed();
                  }
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
  }
</script>

<style>
  .demo-table-expand {
    font-size: 0;
  }
  .demo-table-expand label {
    width: 90px;
    color: #99a9bf;
  }
  .demo-table-expand .el-form-item {
    margin-right: 0;
    margin-bottom: 0;
    width: 50%;
  }
    .el-table .warning-row {
    background: oldlace;
  }

  .el-table .success-row {
    background: #f0f9eb;
  }
</style>