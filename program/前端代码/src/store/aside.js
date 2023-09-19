// 7-30修改医生的主页面侧边栏

// 左侧导航栏数据
export default {
  state: {
    aside_data: [],
    aside_title: '颐养通',//导航栏标题
    isCollapse: false
  },
  mutations: {
    //清空
    clearAsideData (state) {
      state.aside_data = []
    },
    //设置用户的导航栏
    SetClientAsideData (state) {
      state.aside_title = '颐养通';
      state.aside_data = [
        {
          path: '/home',
          name: 'Home',
          label: '首页',
          icon: 's-home',
          url: '/home'
        },
        {
          label: '护理服务',
          icon: 'discover',
          path: '/care',
          children: [
            {
              path: '/careServiceIntro',
              name: 'CareServiceIntro',
              label: '服务介绍',
              icon: 'setting',
              url: '/care/careServiceIntro',
            },
            {
              path: '/CareServiceMine',
              name: 'CareServiceMine',
              label: '我的服务',
              icon: 'setting',
              url: '/care/CareServiceMine',
            },

          ]
        },
        {
          label: '医疗服务',
          icon: 'discover',
          path: '/medical',
          children: [
            {
              path: '/doctorService',
              name: 'DoctorService',
              label: '申请医生上门',
              icon: 'setting',
              url: '/medical/doctorService',
            },
            {
              path: '/onlineDoctor',
              name: 'OnlineDoctor',
              label: '查看线上问诊',
              icon: 'setting',
              url: '/medical/onlineDoctor',
            },
            {
              path: '/hReport',
              name: 'HReport',
              label: '查阅健康报告',
              icon: 'setting',
              url: '/medical/hReport',
            },
            {
              path: '/doctorComment',
              name: 'DoctorComment',
              label: '评价服务',
              icon: 'setting',
              url: '/medical/doctorComment',
            },
            {
              path: '/medicalServiceHistory',
              name: 'MedicalServiceHistory',
              label: '申请记录查看',
              icon: 'setting',
              url: '/medical/medicalServiceHistory',
            }
          ]
        },
        {
          label: '个人中心',
          icon: 'user',
          path: '/user',
          children: [
            {
              path: '/userInfor',
              name: 'UserInfor',
              label: '个人信息中心',
              icon: 'setting',
              url: '/user/userInfor',
            },
            {
              path: '/historyDisease',
              name: 'HistoryDisease',
              label: '既往病史',
              icon: 'setting',
              url: '/user/historyDisease',
            },
            {
              path: '/historyService',
              name: 'HistoryService',
              label: '历史服务订单',
              icon: 'setting',
              url: '/user/historyService',
            }

          ]
        }
      ]
    }
    ,
    SetNurseAsideData (state) {
      state.aside_data = [
        {
          name: 'NurseHome',
          label: '首页',
          icon: 's-home',
          path: '/nursehome'
        },
        {
          label: '接单中心',
          name: 'TakeOrder',
          icon: 'finished',
          path: '/takeorder',
          children: [
            {
              label: '我要接单',
              name: 'TakeOrderApply',
              icon: 'setting',
              path: '/takeorderapply'
            },
            {
              label: '订单历史',
              name: 'TakeOrderMine',
              icon: 'setting',
              path: 'takeordermine'
            }
          ]
        },
        {
          label: '个人中心',
          name: 'Nurser',
          icon: 's-custom',
          path: '/nurser',
          children: [
            {
              name: 'NurserInfor',
              label: '个人信息',
              icon: 'setting',
              path: 'nurserinfor'
            },
            {
              name: 'SeeReport',
              label: '信誉记录',
              icon: 'setting',
              path: 'seereport'
            },
            {
              name: 'NurserQualification',
              label: '资质核验',
              icon: 'setting',
              path: 'nurserqualification'
            }
          ]
        },
        {
          label: '消息中心',
          name: 'NurseMessage',
          icon: 'chat-dot-square',
          path: '/message',
          children: [
            {
              name: 'NurseNotice',
              label: '通知公告',
              icon: 'setting',
              path: 'nursenotice'
            },
            // {
            //   name: 'NurseOrderInfo',
            //   label: '订单申请信息',
            //   icon: 'setting',
            //   path: 'nurseorderinfo'
            // }
          ]
        }
      ]
    },
    SetDoctorAsideData (state) {
      state.aside_data = [
        {
          name: 'DoctorHome',
          label: 'Doctor首页',
          icon: 's-home',
          path: '/doctorHome',
          url: '/doctorHome'
        },
        {
          label: '医疗模块',
          name: 'MedicalUnit',
          icon: 'discover',
          path: '/medicalUnit',
          children: [
            {
              label: '服务申请受理',
              name: 'MedicalOrderApply',
              icon: 'setting',
              path: '/medicalUnit/medicalOrderApply'
            },
            {
              label: '线上问诊管理',
              name: 'OnlineInteraction',
              icon: 'setting',
              path: '/medicalUnit/onlineInteraction'
            },
            {
              label: '健康报告管理',
              name: 'HealthReport',
              icon: 'setting',
              path: '/medicalUnit/healthReport'
            },
            {
              label: '查看服务评价',
              name: 'ServiceComment',
              icon: 'setting',
              path: '/medicalUnit/serviceComment'
            },
            {
              label: '申请人员归档',
              name: 'ClientManagement',
              icon: 'setting',
              path: '/medicalUnit/clientManagement'
            }
          ]
        },
        {
          label: 'Doctor个人中心',
          name: 'DoctorSelf',
          icon: 'discover',
          path: '/doctorSelf',
          children: [
            {
              name: 'DoctorInfor',
              label: '个人信息中心',
              icon: 'setting',
              path: '/doctorInfor'
            },
            {
              name: 'DoctorQualification',
              label: '资质核验',
              icon: 'setting',
              path: '/doctorQualification'
            }
          ]
        }
      ]
    },
    //管理员侧边栏数据
    SetAdminAsideData (state) {
      state.aside_data = [
        {
          name: 'Console',
          label: '管理员首页',
          icon: 's-home',
          path: '/admin/console',
          url: '/admin/console'
        },
        {
          name: 'Search',
          label: '信息管理栏',
          icon: 'search',
          path: '/admin/search',
          url: '/admin/search'
        },

        {
          name: 'AdminVerify',
          label: '审核系统',
          icon: 'view',
          path: '/admin',
          children: [
            {
              name: 'CarerQualify',
              label: '医护人员资质审核',
              icon: 'document-check',
              path: '/carerQualify',
              url: '/carerQualify'
            },
            {
              name: 'OrgQualify',
              label: '机构入驻审核',
              icon: 'document-check',
              path: '/orgQualify',
              url: '/orgQualify'
            },
            {
              name: 'ReportScan',
              label: '举报单审核',
              icon: 'document-check',
              path: '/reportScan',
              url: '/reportScan'
            }
          ]
        },
        {
          name: 'AdminUserInfor',
          label: '个人信息',
          icon: 'user-solid',
          path: '/admin/AdminUserInfor',
          url: '/admin/AdminUserInfor'
        },
      ]
    },
    collapseMenu (state) {
      state.isCollapse = !state.isCollapse
    }
  }
}
