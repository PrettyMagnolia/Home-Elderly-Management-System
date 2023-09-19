
<template>
  <div class="views">
    <div style="margin-bottom:30px">

      <h1 class=head>护工资质核验</h1>
      <el-button
        type="primary"
        icon="el-icon-arrow-left"
        @click="toConsole()"
        class=backButton
      >返回</el-button>
    </div>

    <div style="float:left;">
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
          class="show"
          height="500"
          v-loading="loading"
          :default-sort="{prop: 'UPLOADTIME', order: 'descending'}"
          stripe
        >
          <el-table-column
            prop="WORKERID"
            label="申请人ID"
            width="180"
            sortable
          ></el-table-column>

          <el-table-column
            prop="ROLE"
            label="身份"
            width="180"
          >
          </el-table-column>

          <el-table-column
            prop="TITLE"
            label="资质名称"
            width="180"
          >
          </el-table-column>

          <el-table-column
            prop="UPLOADTIME"
            label="上传时间"
            width="180"
            sortable
          ></el-table-column>

          <el-table-column
            prop="ISDONE"
            label="审核状态"
            width="180"
          >
            <template slot-scope="scope">
              <el-tag
                v-if="scope.row.ISDONE=='待审核' || scope.row.ISDONE=='no' || typeof(scope.row.ISDONE)=='undefined'"
                type='info'
                disable-transitions
              >{{scope.row.ISDONE}}</el-tag>
              <el-tag
                v-else-if="scope.row.ISDONE=='审核通过'"
                type='success'
                disable-transitions
              >{{scope.row.ISDONE}}</el-tag>
              <el-tag
                v-else-if="scope.row.ISDONE=='审核不通过'"
                type='danger'
                disable-transitions
              >{{scope.row.ISDONE}}</el-tag>
              <el-tag
                v-else
                type='info'
                disable-transitions
              >{{scope.row.ISDONE}}</el-tag>

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
        title="申请人信息"
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
                用户ID
              </template>
              {{order_detail.WORKERID}}
            </el-descriptions-item>

            <el-descriptions-item>
              <template slot="label">
                <i class="el-icon-user"></i>
                身份
              </template>
              {{order_detail.ROLE}}
            </el-descriptions-item>

            <el-descriptions-item>
              <template slot="label">
                <i class="el-icon-s-custom"></i>
                资质名称
              </template>
              {{order_detail.TITLE}}
            </el-descriptions-item>

            <el-descriptions-item>
              <template slot="label">
                <i class="el-icon-mobile-phone"></i>
                机构编号
              </template>
              {{order_detail.INSTITUTIONID}}
            </el-descriptions-item>

            <el-descriptions-item>
              <template slot="label">
                <i class="el-icon-mobile-phone"></i>
                机构名称
              </template>
              {{order_detail.INSTITUTIONNAME}}
            </el-descriptions-item>

            <el-descriptions-item>
              <template slot="label">
                <i class="el-icon-mobile-phone"></i>
                资格证书获取日期
              </template>
              {{order_detail.GRANTTIME}}
            </el-descriptions-item>

            <el-descriptions-item>
              <template slot="label">
                <i class="el-icon-mobile-phone"></i>
                资质说明
              </template>
              {{order_detail.PROOF}}
            </el-descriptions-item>

            <el-descriptions-item>
              <template slot="label">
                <i class="el-icon-mobile-phone"></i>
                用户申请日期
              </template>
              {{order_detail.UPLOADTIME}}
            </el-descriptions-item>

            <el-descriptions-item>
              <template slot="label">
                <i class="el-icon-mobile-phone"></i>
                审核管理员ID
              </template>
              {{order_detail.ADMINITORID}}
            </el-descriptions-item>

            <el-descriptions-item>
              <template slot="label">
                <i class="el-icon-mobile-phone"></i>
                管理员审核日期
              </template>
              {{order_detail.AUDITTIME}}
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
import { getWaitingCarerDetail } from '../../../api/data.js'
//import { getCarerCard } from '../../../api/data.js'
export default {
  name: "CarerQualify",
  components: {},
  data () {
    return {
      flag: true,
      order_table: [
        [], []
      ],
      order_detail: {},
      show_table: [],
      dialogVisible: false,//显示弹窗
      tab_index: 0,

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
        this.dialogVisible = false;
        this.$router.push({
          name: "CarerStep1",
          query: { id: row.WORKERID, title: row.TITLE }
        })
      }
      else if (this.tab_index == 1)//已完成审核
      {
        this.dialogVisible = true;
        this.order_detail = row;
      }
    },
    toConsole () {
      this.dialogVisible = false;
      this.$router.push({
        name: "Console",
      })
    },
    //根据订单状态，赋予不同的类名
    tableRowClassName ({ row }) {
      if (row.state === "已完成审核") {
        return "placed-row";
      } else if (row.state === "待办审核") {
        return "underway-row";
      }

      console.log(row);
      return "";
    },
    //tab点击事件,@tab-click自动传入小tab，检查log输出，发现有index下标可用
    HandleTabsClick (tab) {
      this.tab_index = parseInt(tab.index);
      //parseInt(),string->int
    },
    getRowState (row) {
      return row.state;
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
  },
  //初始化向后端请求数据
  mounted: function () {
    this.dialogVisible = false;
    getWaitingCarerDetail()
      .then((res) => {
        console.log("后端请求成功！");
        console.log(res);
        let receive_table = JSON.parse(res.data);
        for (let i = 0; i < receive_table.length; i++) {
          let choice = (receive_table[i].ISDONE == "待审核" || receive_table[i].ISDONE == "no") ? 0 : 1;
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
        console.log(err);
        console.log("后端请求失败");
      })


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
</style>>
