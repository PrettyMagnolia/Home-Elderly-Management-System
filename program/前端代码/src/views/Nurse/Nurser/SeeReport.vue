
<template>
  <div class="views">
    <el-table
      :data="show_table"
      style="width: 100%"
      v-loading="report_loading">
      <template slot="empty">
        <el-tag type="success">您的信誉良好，没有被举报记录</el-tag>
      </template>
      <el-table-column
      type="index"
      width="50">
      </el-table-column>
      <el-table-column
        prop="ID"
        label="举报单ID"
        width="220"
        sortable
      ></el-table-column>
      <el-table-column
        prop="TYPE"
        label="举报原因"
        width="250"
        sortable>
      </el-table-column>
      <el-table-column
        prop="REPORTTIME"
        label="举报日期"
        width="150"
        sortable>
      </el-table-column>
      <el-table-column
        prop="ISDONE"
        label="审核状态"
        width="150">
        <template slot-scope="scope">
          <el-tag>
            {{ scope.row.ISDONE }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column
        prop="AUDITTIME"
        label="审核日期"
        width="150"
        sortable>
      </el-table-column>
      <el-table-column
        prop="PUNITIVEMEASURE"
        label="惩罚类型"
        width="150">
        <template slot-scope="scope">
          <el-tag>
            封号{{ scope.row.PUNITIVEMEASURE }}天
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column align="center" fixed="right">
          <template slot-scope="scope">
            <el-button
              @click="viewDetailButtonClick(scope.row)"
              type="primary"
              size="mini">
              查看详情
            </el-button>
          </template>
      </el-table-column>
    </el-table>
    
    

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
            {{report_detail.ID}}
          </el-descriptions-item>

          <el-descriptions-item>
            <template slot="label">
              <i class="el-icon-user"></i>
              举报类型
            </template>
            {{report_detail.TYPE}}
          </el-descriptions-item>

          <el-descriptions-item>
            <template slot="label">
              <i class="el-icon-user"></i>
              举报人ID
            </template>
            {{report_detail.IMFORMERID}}
          </el-descriptions-item>

          <el-descriptions-item>
            <template slot="label">
              <i class="el-icon-s-custom"></i>
              举报人姓名
            </template>
            {{report_detail.IMFORMERNAME}}
          </el-descriptions-item>

          <el-descriptions-item>
            <template slot="label">
              <i class="el-icon-s-custom"></i>
              被举报人ID
            </template>
            {{report_detail.DEFENDANTID}}
          </el-descriptions-item>

          <el-descriptions-item>
            <template slot="label">
              <i class="el-icon-s-custom"></i>
              被举报人姓名
            </template>
            {{report_detail.DEFENDANTNAME}}
          </el-descriptions-item>

          <el-descriptions-item>
            <template slot="label">
              <i class="el-icon-mobile-phone"></i>
              订单编号
            </template>
            {{report_detail.ORDERID}}
          </el-descriptions-item>

          <el-descriptions-item>
            <template slot="label">
              <i class="el-icon-mobile-phone"></i>
              举报是否属实
            </template>
            {{report_detail.ISREAL}}
          </el-descriptions-item>

          <el-descriptions-item>
            <template slot="label">
              <i class="el-icon-mobile-phone"></i>
              惩罚类型
            </template>
            封号{{report_detail.PUNITIVEMEASURE}}天
          </el-descriptions-item>

          <el-descriptions-item>
            <template slot="label">
              <i class="el-icon-mobile-phone"></i>
              举报时间
            </template>
            {{report_detail.REPORTTIME}}
          </el-descriptions-item>

          <el-descriptions-item>
            <template slot="label">
              <i class="el-icon-mobile-phone"></i>
              审核时间
            </template>
            {{report_detail.AUDITTIME}}
          </el-descriptions-item>

          <el-descriptions-item>
            <template slot="label">
              <i class="el-icon-mobile-phone"></i>
              审核状态
            </template>
            {{report_detail.ISDONE}}
          </el-descriptions-item>

          <el-descriptions-item>
            <template slot="label">
              <i class="el-icon-mobile-phone"></i>
              举报详述
            </template>
            {{report_detail.REPORTTEXT}}
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
</template>

<script>
import { AccessTokenFailed } from '../../../api/data.js'
//import {getUserInfoForAdmin} from '../../../api/data.js'
import { getReportCard } from '../../../api/data.js'
import { getWaitingReportDetail } from '../../../api/data.js'
import { setToken } from '../../../api/data.js'
export default {
  name: "SeeReport",
  components: {},
  data () {
    return {
      my_uid:null,
      access_token:null,
      order_detail: {},//用户个人信息
      tag_type_state: '',
      tag_type_good: '',

      order_table: [],//举报单列表
      show_table: [],//展示的举报单列表
      report_detail: {},

      dialogVisible: false,
      report_num: 0,
      input: '',//暂时没用
      //分页数据
      query: {
        pageNum: 1,
        pageSize: 10,
        total: 0
      },
      report_loading: true,
    };
  },
  methods: {
    toBack () {
      this.dialogVisible = false;
      this.$router.push({
        name: "Search",
      })
    },
    //设置标签颜色
    setTagColor () {
      //设置状态的标签颜色
      if (this.order_detail.state == "正常")
        this.tag_type_state = "success";
      else if (this.order_detail.state == "被封禁")
        this.tag_type_state = "danger";
      else
        this.tag_type_state = "info";

      //设置有无不良记录的标签颜色
      if (this.order_detail.haveBad == "无")
        this.tag_type_good = "success";
      else if (this.order_detail.haveBad == "有")
        this.tag_type_good = "danger";
      else
        this.tag_type_good = "info";
    },
    //查看详细的举报单信息
    viewDetailButtonClick (row) {
      getReportCard(row.ID)
        .then((res) => {
          this.report_detail = JSON.parse(res.data)[0];
          this.dialogVisible = true;
        })
        .catch((err) => {
          if (err.message == "Request failed with status code 403" && this.flag) {
            this.flag = false;
            AccessTokenFailed();
          }
          console.log(err);
        })
    },
    HandleTabsClick (tab) {
      this.tab_index = parseInt(tab.index);
      //parseInt(),string->int
    },
    getUserReport () {//返回值为列表
      let list = [];//仅存储与该用户关联的举报单，即筛选后的列表
      let temp;//存储所有的举报单
      getWaitingReportDetail()
        .then((res) => {
          temp = JSON.parse(res.data);

          //筛选
          for (let i = 0; i < temp.length; i++) {
            if (temp[i].DEFENDANTID == this.my_uid && temp[i].ISDONE == "审核通过")
              list.push(temp[i]);
          }
          this.report_num = list.length;
          console.log("list",list);
        })
        .catch((err) => {
          if (err.message == "Request failed with status code 403" && this.flag) {
            this.flag = false;
            AccessTokenFailed();
          }
          this.$message.error('后端请求失败！');
          console.log(err);
        })

      return list;
    },
    postKeyAndFetch () {
      this.show_table = this.searchResource();
    },
    searchResource () {
      //函数返回值为筛选后的列表
      const search = this.input;
      let result = this.order_table.slice(0);//深拷贝

      if (search)//有内容才执行关键字筛选
      {
        result = result.filter(data => {
          return Object.keys(data).some(key => {
            return String(data[key]).toLowerCase().indexOf(search) > -1
          })
        })

      }

      //分页参数的更改
      this.query.total = result.length;//总页数设置

      return result;
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
    getPageData () {
      this.postKeyAndFetch();
      const DataAll = this.show_table.slice(0);//深拷贝
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


  },
  mounted: function () {
    this.my_uid = this.$cookies.get("USERID");
    this.access_token = this.$cookies.get("token");
    setToken(this.access_token);
    //获取上一个页面传来的该用户信息
    // this.order_detail = JSON.parse(this.$route.query.userInfo);
    this.setTagColor();//设置标签颜色
    //获取该用户的所有举报单
    this.order_table = this.getUserReport();
    this.show_table = this.order_table;
    console.log(this.show_table)

    //初始化分页
    //this.getPageData();
    this.query.total = this.show_table.length;//总页数设置
    this.report_loading = false;
  }

};
</script>

// lang选择less语法，scoped限制该样式只在本文件使用，不影响其他组件
<style lang="less" scoped>
</style>
