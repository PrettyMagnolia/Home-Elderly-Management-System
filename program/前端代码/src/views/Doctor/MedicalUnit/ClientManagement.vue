
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
      type="expand">
      <template  slot="header">
         <i class="el-icon-phone-outline"></i>
      </template>
      <template slot-scope="props">
        <el-form label-position="left" inline class="demo-table-expand">
          <el-form-item label="本人电话: ">
            <span>{{ props.row.PHONE }}</span>
          </el-form-item>
          <el-form-item label="紧急电话: ">
            <span>{{ props.row.URGENT }}</span>
          </el-form-item>
          <el-form-item label="时段: ">
            <span>{{ props.row.TIMESLOT }}</span>
          </el-form-item>
          <el-form-item label="金额: ">
            <span>{{ props.row.MONEY }}</span>
          </el-form-item>
          <el-form-item label="服务地址: ">
            <span>{{ props.row.ADDRESS }}</span>
          </el-form-item>
          <el-form-item label="支付方式: ">
            <span>{{ props.row.METHOD }}</span>
          </el-form-item>
        </el-form>
      </template>
    </el-table-column>
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
    ></el-table-column>
    <el-table-column
      prop="ENDTIME"
      label="结束日期"
      sortable="custom"
     
      column-key="ENDTIME"
      :filters="[{text: '2022-09', value: '2022-09'}, 
                 {text: '2022-08', value: '2022-08'}, 
                 {text: '2022-07', value: '2022-07'}, 
                 {text: '2022-06', value: '2022-06'},
                 {text: '2022-05', value: '2022-05'}, 
                 {text: '2022-04', value: '2022-04'}]"
      :filter-method="filterDateHandler"
    ></el-table-column>
    <el-table-column
      prop="NAME"
      label="申请人"
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
    <el-table-column  align="center" label="关注/取消关注">
      <template slot-scope="scope">
        <el-button
        type="primary"
        v-if="!FollowedOrNot(scope.row)"
        size="mini"
        @click="Follow(scope.row)"
          >关注/取消关注</el-button
        >
        <el-button
        type="danger"
        v-if="FollowedOrNot(scope.row)"
        size="mini"
        @click="Follow(scope.row)"
          >取消关注</el-button
        >
      </template>
    </el-table-column>
    <el-table-column width=200 align="center" label="支付管理">
      <template slot-scope="scope">
          <el-button
          size="mini"
          type="success"
          @click="onlineMethod(scope.row,1)">已支付</el-button>
          <el-button
          size="mini"
          type="danger"
          @click="onlineMethod(scope.row,0)">未支付</el-button>
      </template>
    </el-table-column>
    <el-table-column width=200 align="center" label="服务管理">
      <template slot-scope="scope">
          <el-button
          size="mini"
          type="success"
          @click="serviceManagement(scope.row,1)">服务完成</el-button>
          <el-button
          size="mini"
          type="primary"
          @click="serviceManagement(scope.row,0)">继续服务</el-button>
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
      ban:false,
      access_token: null,
      tableData: [],
      tableData2: [],
      showTableData:[],
      search:'',
      currentPage: 1,
      pageSize:8,
      totalNum:0,
      doctorid:'', //102022082300000001

      followed:false,
      content:"关注",
      bg_color:"#8080ff",
      ft_color:"#ffffff",
    };
  },
  methods: {
    //根据订单状态，赋予不同的类名
    tableRowClassName({row}) {
        if (row.STATUS === '已完成') {
          return 'success-row';
        }else if(row.STATUS === '未受理'){
          return 'warning-row';
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
      onlineMethod(row,option){
        if(this.ban === true){
          alert('您已被封号，失去权限');
        }
        else{
          if(option === 1){
            if(row.ORDERSTATUS === '未支付' && row.METHOD === '上门现付'){
              row.ORDERSTATUS = '已支付';
              this.$http
              .put('/API/medical/order?serviceid='+row.SERVICEID+'&orderstatus=已支付',null,{
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
            else if(row.ORDERSTATUS === '未支付' && row.METHOD === '线上支付'){
              alert('您无权限更改线上支付的付款情况！');
            }
          }
          else if(option === 0){
            if(row.ORDERSTATUS === '已支付' && row.METHOD === '上门现付'){
              row.ORDERSTATUS = '未支付';
              this.$http
              .put('/API/medical/order?serviceid='+row.SERVICEID+'&orderstatus=未支付',null,{
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
            else if(row.ORDERSTATUS === '已支付' && row.METHOD === '线上支付'){
              alert('该服务已线上付款！');
            }
          }
        }
      },
      serviceManagement(row,option){
        if(this.ban === true){
          alert('您已被封号，失去权限');
        }
        else{
          if(option === 1){
            if(row.STATUS === '已完成'){
              alert('该服务已完成！');
            }
            else if(row.STATUS === '进行中'){
              row.STATUS = '已完成';
              this.$http
              .put('/API/medical/status?serviceid='+row.SERVICEID+'&status=已完成',null,{
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
          }
          else if(option === 0){
            if(row.STATUS === '进行中'){
              alert('该服务进行中！');
            }
            else if(row.STATUS === '已完成'){
            const nowDate = new Date();
              const date = {
                year: nowDate.getFullYear(),
                month: nowDate.getMonth() + 1,
                date: nowDate.getDate(),
              };
              const newmonth = date.month >= 10 ? date.month : "0" + date.month;
              const day = date.date >= 10 ? date.date : "0" + date.date;
              var currentDate = date.year + "-" + newmonth + "-" + day;

              if(row.ENDTIME > currentDate){
                row.STATUS = '进行中';
                this.$http
                .put('/API/medical/status?serviceid='+row.SERVICEID+'&status=进行中',null,{
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
              else{
                alert('该服务已过期');
              }
            }
          }
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
          if (a < b) {
            return rev * -1;
          }
          if (a > b) {
            return rev * 1;
          }
          return 0;
        }
      },
      Follow(row) {
      this.followed=!this.followed;
      
      if(!this.followed){
          this.content="关注"
          this.bg_color="#8080ff";
          this.ft_color="#ffffff"
          this.$http
        .delete(
          '/API/follow?FOLLOWERID='+this.$cookies.get("USERID")
          +'&FOLLOWINGID='+row.ELDERID,
          {
            headers: {
              TokenValue: this.access_token,
            },
          }
        )
        .then((result) => {
          console.log("******delete******");
        })
        .catch((err) => {
          if (
            err.message === "Request failed with status code 500" ||
            err.message === "Request failed with status code 403"
          ) {
            AccessTokenFailed();
          }
          console.log(err);
        });
      this.$message({
        message: "取消关注成功",
        type: "success",
      });
      }

      else{
          this.content="取消关注";
          this.bg_color="#f56c6c";
          this.ft_color="#fef0f0";
          this.$http
        .post(
          "/API/follow",
          {
            FOLLOWERID: this.$cookies.get("USERID"),
            FOLLOWINGID: row.ELDERID,
          },
          {
            headers: {
              TokenValue: this.access_token,
            },
          }
        )
        .then((result) => {
          console.log("******success******");
        })
        .catch((err) => {
          if (
            err.message === "Request failed with status code 500" ||
            err.message === "Request failed with status code 403"
          ) {
            AccessTokenFailed();
          }
          console.log(err);
        });
      this.$message({
        message: "关注成功",
        type: "success",
      });
      }

      
    },
    FollowedOrNot(row){
      this.$http.request({
        url: "/API/Follow/followingID?followingID=" + row.ELDERID,
        method: "get",     
        headers: {
              TokenValue: this.access_token,
        },
      })
      .then(async (res) => {
        this.tableData2 = JSON.parse(res.data);
        for(let i = 0; i < this.tableData2.length; i++)
          if(this.$cookies.get("USERID") == this.tableData2[i].USERID)
          {
            return true;
          }
      })
      .catch((err) => {
        if (
            err.message === "Request failed with status code 500" ||
            err.message === "Request failed with status code 403"
          ) {
            AccessTokenFailed();
          }
          console.log(err);
      });
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
          .get('/API/medical/doctor?doctorID='+this.doctorid+'&status=1',{
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
