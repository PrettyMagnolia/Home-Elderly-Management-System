
<template>
  <div class="views">
    <div>
      <h1 class=head>管理员搜索系统</h1>
    </div>

    <div style="margin-bottom:30px">
      <el-button
        type="primary"
        icon="el-icon-arrow-left"
        @click="toConsole()"
        class=backButton
      >返回</el-button>
    </div>

    <!--搜索栏-->
    <div class=panel>

      <!--信息展示-->
      <div style="float:left;">
        <el-card>
          <h5>高级筛选条件</h5>
          <!--复选框-->
          <el-checkbox
            label="显示老人"
            v-model="bool_showElder"
            @change="postKeyAndFetch()"
          ></el-checkbox>
          <el-checkbox
            label="显示护工"
            v-model="bool_showCarer"
            @change="postKeyAndFetch()"
          ></el-checkbox>
          <el-checkbox
            label="显示医生"
            v-model="bool_showDoctor"
            @change="postKeyAndFetch()"
          ></el-checkbox>
          <el-checkbox
            label="屏蔽记录良好的用户"
            v-model="bool_showBadOnly"
            @change="postKeyAndFetch()"
          ></el-checkbox>

          <br><br>
          <el-input
            placeholder="请输入搜索关键字"
            v-model="input"
            class="input-with-select"
          >
            <el-button
              slot="append"
              icon="el-icon-search"
              @click="postKeyAndFetch()"
            ></el-button>
          </el-input>

          <el-divider></el-divider>
          <!-- table -->
          <el-table
            :data="show_table"
            class="order_table"
            height="450"
            v-loading="loading"
            :default-sort="{prop: 'id', order: 'ascending'}"
            stripe
          >
            <el-table-column
              prop="id"
              label="用户ID"
              sortable
              width="180"
            ></el-table-column>
            <el-table-column
              prop="name"
              label="姓名"
              width="180"
            ></el-table-column>
            <el-table-column
              prop="role"
              label="身份"
              width="180"
            ></el-table-column>
            <el-table-column
              prop="sex"
              label="性别"
              width="180"
            ></el-table-column>
            <el-table-column
              prop="haveBad"
              label="有无不良记录"
              width="280"
            >
              <template slot-scope="scope">
                <el-tag
                  :type="scope.row.haveBad=='有'?'danger':'success'"
                  disable-transitions
                >{{scope.row.haveBad}}</el-tag>
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

      </div>
    </div>

  </div>
</template>

<script>
import { AccessTokenFailed } from '../../../api/data.js'
import { getAllElder } from '../../../api/data.js'
import { getAllNurse } from '../../../api/data.js'
import { getAllDoctor } from '../../../api/data.js'
//import axios from "../../../api/axios";
// 引入原生axios 实现并行请求处理
import Axios from "axios";
export default {
  name: "Search",
  components: {},
  data () {
    return {
      input: '',
      select: '',
      order_table: [],
      show_table: [],

      //复选框的几个布尔变量
      bool_showElder: true,
      bool_showCarer: true,
      bool_showDoctor: true,
      bool_showBadOnly: false,


      //分页数据
      query: {
        pageNum: 1,
        pageSize: 10,
        total: 0
      },
      loading: true,
    };
  },
  methods: {
    toConsole () {
      this.dialogVisible = false;
      this.$router.push({
        name: "Console",
      })
    },
    //得到符合条件的信息
    postKeyAndFetch () {
      this.show_table = this.searchResource();
    },
    //查看用户详细信息
    viewDetailButtonClick (row) {
      this.dialogVisible = false;

      this.$router.push({
        name: "UserInfo",
        query: { userInfo: JSON.stringify(row) }
      })
    },
    //tab点击事件,@tab-click自动传入小tab，检查log输出，发现有index下标可用
    HandleTabsClick (tab) {
      this.tab_index = parseInt(tab.index);
      //parseInt(),string->int
    },

    searchResource () {
      this.loading = true;
      //函数返回值为筛选后的列表
      const search = this.input;
      let result = this.order_table.slice(0);//深拷贝
      let i = 0;
      if (search)//有内容才执行关键字筛选
      {
        result = result.filter(data => {
          return Object.keys(data).some(key => {
            return String(data[key]).toLowerCase().indexOf(search) > -1
          })
        })

      }
      //复选框筛选
      for (i = 0; i < result.length; i++) {
        console.log(result.length);
        console.log(i);

        if ((result[i].role == "老人" && !this.bool_showElder) ||
          (result[i].role == "护工" && !this.bool_showCarer) ||
          (result[i].role == "医生" && !this.bool_showDoctor) ||
          (result[i].haveBad == "无" && this.bool_showBadOnly)) {
          //删除不符合条件的项
          result.splice(i, 1);
          i--;//！！！！注意result.length动态变化！
          continue;
        }
      }

      //分页参数的更改
      this.query.total = result.length;//总页数设置
      this.loading = false;
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
    getHaveBad (bantime) {
      if (bantime == null || typeof (bantime) == 'undefined' || bantime == '')
        return '无';
      else
        return '有';
    },
    getState (bantime) {
      if (bantime == null || typeof (bantime) == 'undefined' || bantime == '')
        return "正常";

      //处理当前时间
      let currentDate = new Date();
      let currentYear = currentDate.getFullYear();
      let currentMonth = currentDate.getMonth() + 1;
      let currentDay = currentDate.getDate();

      //处理封禁的截止时间
      let previousYear = parseInt(bantime.substring(0, 4), 10);
      let previousMonth = parseInt(bantime.substring(5, 7), 10);
      let previousDay = parseInt(bantime.substring(8, 10), 10);

      if (currentYear < previousYear)
        return "被封禁";
      else {
        if (currentMonth < previousMonth)
          return "被封禁";
        else {
          if (currentDay < previousDay)
            return "被封禁";
        }
      }

      return "正常";
    },
    getAllUser () {//返回值：所有用户的列表
      let list = [];


      Axios.all([getAllElder(), getAllNurse(), getAllDoctor()])
        .then(Axios.spread((resElder, resNurse, resDoctor) => {
          console.log("后端请求成功!");
          //所有老人
          let elderList = resElder.data.message;
          //console.log(res.data.message);
          elderList.forEach(item => {
            //计算不良记录标记
            let my_haveBad = this.getHaveBad(item.bantime);
            //计算用户状态
            let my_state = this.getState(item.bantime);
            //构造对象
            this.getState(item.bantime);
            list.push({
              id: item.USERID,
              name: item.NAME,
              state: my_state,
              role: '老人',
              sex: item.GENDER,
              age: item.AGE,
              haveBad: my_haveBad
            })
          })

          let nurseList = resNurse.data.message;
          nurseList.forEach(item => {
            //计算不良记录标记
            let my_haveBad = this.getHaveBad(item.bantime);
            //计算用户状态
            let my_state = this.getState(item.bantime);
            //构造对象
            list.push({
              id: item.USERID,
              name: item.NAME,
              state: my_state,
              role: '护工',
              sex: item.GENDER,
              age: item.AGE,
              haveBad: my_haveBad
            })
          })

          let doctorList = resDoctor.data;//这里得到的res.data是对象类型，不是JSON!
          doctorList.forEach(item => {
            //计算不良记录标记
            let my_haveBad = this.getHaveBad(item.BANTIME);
            //计算用户状态
            let my_state = this.getState(item.BANTIME);
            list.push({
              id: item.USERID,
              name: item.NAME,
              state: my_state,//这个要改
              role: "医生",
              sex: item.GENDER,
              age: item.AGE,
              haveBad: my_haveBad//这个要改
            })
          })
          console.log("所有用户信息后端请求成功！");
          this.show_table = this.order_table.slice(0, 10);

          //初始化分页
          //this.getPageData();
          this.query.total = this.order_table.length;//总页数设置
          this.loading = false;
        }))
        .catch((err) => {
          if (err.message == "Request failed with status code 403" && this.flag) {
            this.flag = false;
            AccessTokenFailed();
          }
          console.log(err);
          console.log("后端请求失败");
        });

      return list;
    }
  },
  mounted: function () {
    this.order_table = this.getAllUser();

  },
  watch: {
    input: {
      handler (newVal, oldVal) {
        this.postKeyAndFetch();
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

.panel {
  float: left;
  //width:800px;
  //height:200px;
  //border:0px solid #000;
  margin-right: 50px;
  //background-color:#EDEDED;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.12), 0 0 6px rgba(0, 0, 0, 0.04);
  //display:inline-block;
}

.el-select .el-input {
  width: 130px;
}
.input-with-select .el-input-group__prepend {
  background-color: #fff;
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
</style>
