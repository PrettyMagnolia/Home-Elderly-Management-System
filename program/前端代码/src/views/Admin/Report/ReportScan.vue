
<template>
  <div class="views">
    <div style="margin-bottom:30px">
      <h1 class=head>举报单审核</h1>
      <el-button
        type="primary"
        icon="el-icon-arrow-left"
        @click="toConsole()"
        class=backButton
      >返回</el-button>
    </div>

    <div style="float:left">
      <!-- tab -->
      <el-card>
        <el-tabs
          type="card"
          class="tabs"
          @tab-click="HandleTabsClick"
        >
          <el-tab-pane label="待办审核"></el-tab-pane>
          <el-tab-pane label="已完成审核"></el-tab-pane>

        </el-tabs>

        <!-- table -->
        <el-table
          :data="show_table"
          class="order_table"
          height="500"
          v-loading="loading"
          :default-sort="{prop: 'REPORTTIME', order: 'descending'}"
          stripe
        >
          <el-table-column
            prop="report_id"
            label="举报单编号"
            width="180"
            sortable
          ></el-table-column>
          <el-table-column
            prop="report_type"
            label="举报原因"
            width="180"
          >
          </el-table-column>

          <el-table-column
            prop="date"
            label="举报时间"
            width="180"
            sortable
          ></el-table-column>

          <el-table-column
            prop="state"
            label="审核状态"
            width="380"
          >
            <template slot-scope="scope">
              <el-tag
                v-if="scope.row.state=='待审核'"
                type='info'
                disable-transitions
              >{{scope.row.state}}</el-tag>
              <el-tag
                v-else-if="scope.row.state=='审核通过'"
                type='success'
                disable-transitions
              >{{scope.row.state}}</el-tag>
              <el-tag
                v-else-if="scope.row.state=='审核不通过'"
                type='danger'
                disable-transitions
              >{{scope.row.state}}</el-tag>

            </template>
          </el-table-column>

          <el-table-column
            fixed="right"
            label="操作"
            width="120"
          >
            <template slot-scope="scope">
              <el-button
                @click.native.prevent="viewDetailButtonClick(scope.row)"
                type="text"
                size="small"
              >
                查看详情
              </el-button>
            </template>
          </el-table-column>
        </el-table>

        <el-pagination
          @size-change="handleSizeChange"
          @current-change="handleCurrentChange"
          :current-page="query.pageNum"
          :page-sizes="[5, 10, 20, 30]"
          :page-size="query.pageSize"
          layout="total, sizes, prev, pager, next, jumper"
          :total="query.total"
        ></el-pagination>
      </el-card>

      <!-- 弹窗对话框 -->
      <el-dialog
        class="serviceDialog"
        title="举报单卡片"
        :visible.sync="dialogVisible"
      >

        <div class="dialog-content">

          <!-- :column="2" 一行有2个 -->
          <el-descriptions
            class="margin-top"
            :column="2"
            border
          >

            <el-descriptions-item>
              <template slot="label">
                <i class="el-icon-user"></i>
                举报单编号
              </template>
              {{report_detail.report_id}}
            </el-descriptions-item>

            <el-descriptions-item>
              <template slot="label">
                <i class="el-icon-user"></i>
                举报类型
              </template>
              {{report_detail.report_type}}
            </el-descriptions-item>

            <el-descriptions-item>
              <template slot="label">
                <i class="el-icon-user"></i>
                举报人ID
              </template>
              {{report_detail.id1}}
            </el-descriptions-item>

            <el-descriptions-item>
              <template slot="label">
                <i class="el-icon-s-custom"></i>
                举报人姓名
              </template>
              {{report_detail.name1}}
            </el-descriptions-item>

            <el-descriptions-item>
              <template slot="label">
                <i class="el-icon-s-custom"></i>
                被举报人ID
              </template>
              {{report_detail.id2}}
            </el-descriptions-item>

            <el-descriptions-item>
              <template slot="label">
                <i class="el-icon-s-custom"></i>
                被举报人姓名
              </template>
              {{report_detail.name2}}
            </el-descriptions-item>

            <el-descriptions-item>
              <template slot="label">
                <i class="el-icon-mobile-phone"></i>
                订单编号
              </template>
              {{report_detail.order_id}}
            </el-descriptions-item>

            <el-descriptions-item>
              <template slot="label">
                <i class="el-icon-mobile-phone"></i>
                举报是否属实
              </template>
              {{report_detail.real}}
            </el-descriptions-item>

            <el-descriptions-item>
              <template slot="label">
                <i class="el-icon-mobile-phone"></i>
                惩罚类型
              </template>
              封号{{report_detail.punish_type}}天
            </el-descriptions-item>

            <el-descriptions-item>
              <template slot="label">
                <i class="el-icon-mobile-phone"></i>
                举报时间
              </template>
              {{report_detail.report_time}}
            </el-descriptions-item>

            <el-descriptions-item>
              <template slot="label">
                <i class="el-icon-mobile-phone"></i>
                审核时间
              </template>
              {{report_detail.audit_time}}
            </el-descriptions-item>

            <el-descriptions-item>
              <template slot="label">
                <i class="el-icon-mobile-phone"></i>
                审核状态
              </template>
              {{report_detail.state}}
            </el-descriptions-item>

            <el-descriptions-item>
              <template slot="label">
                <i class="el-icon-mobile-phone"></i>
                举报详述
              </template>
              {{report_detail.report_text}}
            </el-descriptions-item>

          </el-descriptions>
        </div>
        <!-- 底部的slot插槽 -->
        <span
          slot="footer"
          class="dialogFooter"
        >
          <el-button
            type="primary"
            @click="dialogVisible = false"
          >确认</el-button>
          <el-button @click="dialogVisible = false">取消</el-button>
        </span>
      </el-dialog>
    </div>

  </div>
</template>

<script>
import { AccessTokenFailed } from '../../../api/data.js'
import { getWaitingReportDetail } from '../../../api/data.js'
import { getReportCard } from '../../../api/data.js'
export default {
  name: "ReportScan",
  components: {},
  data () {
    return {
      flag: true,
      order_table: [
        [], []
      ],
      show_table: [],
      dialogVisible: false,
      selected_index: 0,
      tab_index: 0,
      report_detail: {},

      //分页数据
      query: {
        pageNum: 1,
        pageSize: 10,
        total: 0
      },
      loading: true,
    }
  },
  methods: {
    viewDetailButtonClick (row) {
      if (this.tab_index == 0)//待办审核
      {
        //从后端拿举报单数据
        getReportCard(row.report_id)
          .then((res) => {
            this.dialogVisible = false;
            this.$router.push({
              name: "DetailReport",
              query: { res: res }
            })
          })
          .catch((err) => {
            if (err.message == "Request failed with status code 403" && this.flag) {
              this.flag = false;
              AccessTokenFailed();
            }
            console.log(err)
          });

        //console.log(this.report_detail);

      }
      else if (this.tab_index == 1)//已完成审核
      {
        //从后端拿举报单数据
        getReportCard(row.report_id)
          .then((res) => {
            this.report_detail = this.JSON_to_myObject(res);
          })
          .catch((err) => {
            if (err.message == "Request failed with status code 403" && this.flag) {
              this.flag = false;
              AccessTokenFailed();
            }
            console.log(err)
          });

        this.dialogVisible = true;
        //console.log(this.report_detail.report_text);
      }
    },
    toConsole () {
      this.dialogVisible = false;
      this.$router.push({
        name: "Console",
      })
    },
    HandleTabsClick (tab) {
      this.tab_index = parseInt(tab.index);
      //parseInt(),string->int
    },
    getPageData () {//获取页面数据的函数
      const DataAll = this.order_table[this.tab_index].slice(0);//深拷贝
      //每次执行方法，将展示的数据清空
      this.show_table = [];
      //第一步：当前页的第一条数据在总数据中的位置
      let strlength = (this.query.pageNum - 1) * this.query.pageSize + 1;
      //第二步：当前页的最后一条数据在总数据中的位置
      let endlength = this.query.pageNum * this.query.pageSize;
      //第三步：此判断很重要，执行时机：当分页的页数在最后一页时，进行重新筛选获取数据时
      //获取本次表格最后一页第一条数据所在的位置的长度
      let resStrLength = 0;
      if (DataAll.length % this.query.pageSize == 0) {
        resStrLength = (parseInt(DataAll.length / this.query.pageSize) - 1) * this.query.pageSize + 1
      } else {
        resStrLength = parseInt(DataAll.length / this.query.pageSize) * this.query.pageSize + 1
      }
      //如果上一次表格的最后一页第一条数据所在的位置的长度 大于 本次表格最后一页第一条数据所在的位置的长度，则将本次表格的最后一页第一条数据所在的位置的长度 为最后长度
      if (strlength > resStrLength) {
        strlength = resStrLength
      }
      //第四步：此判断很重要，当分页的页数切换至最后一页，如果最后一页获取到的数据长度不足最后一页设置的长度，则将设置的长度 以 获取到的数据长度 为最后长度
      if (endlength > DataAll.length) {
        endlength = DataAll.length;
      }
      //第五步：循环获取当前页数的数据，放入展示的数组中
      for (let i = strlength - 1; i < endlength; i++) {
        this.show_table.push(DataAll[i]);
      }
      //数据的总条数
      this.query.total = DataAll.length;
    },
    //切换当前页显示的数据条数，执行方法
    handleSizeChange (val) {
      //console.log(`每页 ${val} 条`);
      this.query.pageSize = val;
      this.getPageData();
    },
    //切换页数，执行方法
    handleCurrentChange (val) {
      //console.log(`当前页: ${val}`);
      this.query.pageNum = val;
      this.getPageData();
    },
    JSON_to_myArray (res) {
      //把JSON转化成对象列表
      let list = JSON.parse(res.data);
      //把后端的变量换成前端起的名字
      let receive_table = [];
      list.forEach(item => {
        receive_table.push({
          report_id: item.ID,
          report_type: item.TYPE,
          date: item.REPORTTIME,
          state: item.ISDONE
        });
      });
      return receive_table;
    },
    JSON_to_myObject (res) {
      //res是只含一个对象的列表(JSON格式)
      //把JSON转化成对象
      let list = JSON.parse(res.data);
      //把后端的变量换成前端起的名字
      let receive_table = [];
      list.forEach(item => {
        receive_table.push({
          report_id: item.ID,
          report_type: item.TYPE,
          date: item.REPORTTIME,
          state: item.ISDONE,
          id1: item.IMFORMERID,
          name1: item.IMFORMERNAME,
          id2: item.DEFENDANTID,
          name2: item.DEFENDANTNAME,
          order_id: item.ORDERID,
          //order_type:
          real: item.ISREAL,
          punish_type: item.PUNITIVEMEASURE,
          //avg_score,
          report_time: item.REPORTTIME,
          audit_time: item.AUDITTIME,
          report_text: item.REPORTTEXT,
        });
      });
      return receive_table[0];
    }
  },
  mounted: function () {
    //从后端拿举报单数据
    getWaitingReportDetail()
      .then((res) => {

        let receive_table = this.JSON_to_myArray(res);

        for (let i = 0; i < receive_table.length; i++) {
          //选择加到哪一个列表里
          let choice = receive_table[i].state == "待审核" ? 0 : 1;
          this.order_table[choice].push(receive_table[i]);
        }


        this.show_table = this.order_table[this.tab_index].slice(0, 10);//深拷贝
        this.query.total = this.order_table[this.tab_index].length;//初始化条目总数
        this.loading = false;
      })
      .catch((err) => {
        if (err.message == "Request failed with status code 403" && this.flag) {
          this.flag = false;
          AccessTokenFailed();
        }
        console.log(err)
      });
  },
  watch: {
    tab_index: {
      handler (newVal, oldVal) {
        this.query.total = this.order_table[this.tab_index].length;//更改页面总条数
        this.show_table = this.order_table[this.tab_index].slice(0);//更改显示的条目集合
        this.getPageData();//换页后重新获取页的数据
        console.log(newVal);
        console.log(oldVal);
      }
    }
  }

};
</script>

// lang选择less语法，scoped限制该样式只在本文件使用，不影响其他组件
<style lang="less" scoped>
.head {
  float: left;
  margin-right: 800px;
}
.backButton {
  float: right; // 靠右
}
</style>>
