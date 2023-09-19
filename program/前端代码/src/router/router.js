// 7-30叶登旭 添加医生主页路由
import Vue from 'vue'
import VueRouter from 'vue-router'

import Main from '../views/Main.vue'
import Client from '../views/Client/Client.vue'
import Home from '../views/Client/Home/Index.vue'

import Medical from '../views/Client/Medical/Index.vue'
import User from '../views/Client/User/Index.vue'
import Admin from '../views/Admin/Administrator.vue'

//用户（老人）在护理模块的路由
import CareServiceIntro from '../views/Client/Care/CareServiceIntro.vue'
import CareServiceMine from '../views/Client/Care/CareServiceMine.vue'
import PlaceCareOrder from '../views/Client/Care/PlaceCareOrder.vue'
import ClientOrderFrame from '../views/Client/ClientOrderFrame.vue'
import ClientPayment from '../views/Client/ClientPayment.vue'


import DoctorService from '../views/Client/Medical/DoctorService.vue'
import OnlineDoctor from '../views/Client/Medical/OnlineDoctor.vue'
import HReport from '../views/Client/Medical/HReport.vue'
import DoctorComment from '../views/Client/Medical/DoctorComment.vue'
import MedicalServiceHistory from '../views/Client/Medical/MedicalServiceHistory.vue'



import UserInfor from '../views/Client/User/UserInfor.vue'
import HistoryDisease from '../views/Client/User/HistoryDisease.vue'
import HistoryService from '../views/Client/User/HistoryService.vue'
import ModifyPassword from '../views/Client/User/ModifyPassword.vue'
import ModifyInfor from '../views/Client/User/ModifyInfor.vue'
import DoctorUserInfor from '../views/Client/User/DoctorUserInfor.vue'
import NurseUserInfor from '../views/Client/User/NurseUserInfor.vue'


//管理员模块的路由
import Console from '../views/Admin/Console/Console.vue'
import CarerQualify from '../views/Admin/Carer/CarerQualify.vue'
import OrgQualify from '../views/Admin/Org/OrgQualify.vue'
import OrgStep1 from '../views/Admin/Org/OrgStep1.vue'
import ReportScan from '../views/Admin/Report/ReportScan.vue'
import CarerStep1 from '../views/Admin/Carer/CarerStep1.vue'
import CarerStep2 from '../views/Admin/Carer/CarerStep2.vue'
import CarerStep3 from '../views/Admin/Carer/CarerStep3.vue'
import DetailReport from '../views/Admin/Report/DetailReport.vue'
import Search from '../views/Admin/Info/Search.vue'
import UserInfo from '../views/Admin/Info/UserInfo.vue'
import AdmitCarer from '../views/Admin/Carer/AdmitCarer.vue'
import AdmitReport from '../views/Admin/Report/AdmitReport.vue'
import AdmitOrg from '../views/Admin/Org/AdmitOrg.vue'
//管理员的个人信息模块
import AdminUserInfor from '../views/Admin/AdminInfor/AdminUserInfor.vue'
import AdminModifyInfo from '../views/Admin/AdminInfor/AdminModifyInfo.vue'
import AdminModifyPassword from '../views/Admin/AdminInfor/AdminModifyPassword.vue'



//护工模块的路由
import Nurse from '../views/Nurse/Nurse.vue'
import NurseHome from '../views/Nurse/Home/Index.vue'
import TakeOrder from '../views/Nurse/TakeOrder/Index.vue'
import TakeOrderApply from '../views/Nurse/TakeOrder/TakeOrderApply.vue'
import TakeOrderMine from '../views/Nurse/TakeOrder/TakeOrderMine.vue'
import Nurser from '../views/Nurse/Nurser/Index.vue'
import NurserInfor from '../views/Nurse/Nurser/NurserInfor.vue'
// import NurserHistoryService from '../views/Nurse/Nurser/NurserHistoryService.vue'
import NurseMessage from '../views/Nurse/Message/Index.vue'
import NurseNotice from '../views/Nurse/Message/NurseNotice.vue'
// import NurseOrderInfo from '../views/Nurse/Message/NurseOrderInfo.vue'
import NurserQualification from '../views/Nurse/Nurser/NurserQualification.vue'
import NurseModifyPassword from '../views/Nurse/Nurser/ModifyPassword.vue'
import NurseModifyInfor from '../views/Nurse/Nurser/ModifyInfor.vue'
import AddQualification from '../views/Nurse/Nurser/AddQualification.vue'
import SeeReport from '../views/Nurse/Nurser/SeeReport.vue'


//医生模块的路由
import Doctor from '../views/Doctor/Doctor.vue'
import MedicalOrderApply from '../views/Doctor/MedicalUnit/MedicalOrderApply.vue'
import OnlineInteraction from '../views/Doctor/MedicalUnit/OnlineInteraction.vue'
import HealthReport from '../views/Doctor/MedicalUnit/HealthReport.vue'
import ServiceComment from '../views/Doctor/MedicalUnit/ServiceComment.vue'
import ClientManagement from '../views/Doctor/MedicalUnit/ClientManagement.vue'
import DoctorInfor from '../views/Doctor/DoctorSelf/DoctorInfor.vue'
import DoctorHome from '../views/Doctor/Home/Index.vue'
import DoctorModifyPassword from '../views/Doctor/DoctorSelf/ModifyPassword.vue'
import DoctorModifyInfor from '../views/Doctor/DoctorSelf/ModifyInfor.vue'
import DoctorQualification from '../views/Doctor/DoctorSelf/DoctorQualification.vue'
import DoctorAddQualification from '../views/Doctor/DoctorSelf/DoctorAddQualification.vue'

//登录注册的路由
import LoginFrame from '../views/Login/LoginFrame.vue'
import Login from '../views/Login/Login.vue'
import Register from '../views/Login/Register.vue'
import ApplyIns from '../views/Login/ApplyIns.vue'
Vue.use(VueRouter)
import cookies from "vue-cookies";
import axios from 'axios'
// import { use } from 'vue/types/umd'

//此VueRouter是自己自定义引入暴露出来的，即是自定义的，以下的VueRouter同样
// 解决两次访问相同路由地址报错的问题
const originalPush = VueRouter.prototype.push
VueRouter.prototype.push = function push(location) {
    return originalPush.call(this, location).catch(err => err)
}



//路由配置项
const routes = [
    {
        path: '/',
        name: 'Main',
        component: Main,
        children: [
            //用户路由
            {
                path: '/client',
                name: 'Client',
                component: Client,
                children: [
                    {
                        path: '/',
                        name: 'Home',
                        component: Home,
                        meta: {
                            needAuth: true,
                            title: '首页',
                        }
                    },//护理模块
                    {
                        path: 'careServiceIntro',
                        name: 'CareServiceIntro',
                        component: CareServiceIntro,
                        meta: {
                            needAuth: true,
                            title: '护理介绍',
                        }
                    },
                    {
                        path: 'careServiceMine',
                        name: 'CareServiceMine',
                        component: CareServiceMine,
                        meta: {
                            needAuth: true,
                            title: '我的护理服务',
                        }
                    },



                    // 此处为医疗模块 
                    {
                        path: 'medical',
                        name: 'Medical',
                        component: Medical,
                        children: [
                            {
                                path: 'doctorService',
                                name: 'DoctorService',
                                component: DoctorService,
                                meta: {
                                    needAuth: true,
                                    // title: '',
                                }
                            },
                            {
                                path: 'onlineDoctor',
                                name: 'OnlineDoctor',
                                component: OnlineDoctor,
                                meta: {
                                    needAuth: true,
                                    // title: '',
                                }
                            },
                            {
                                path: 'hReport',
                                name: 'HReport',
                                component: HReport,
                                meta: {
                                    needAuth: true,
                                    // title: '',
                                }
                            },
                            {
                                path: 'doctorComment',
                                name: 'DoctorComment',
                                component: DoctorComment,
                                meta: {
                                    needAuth: true,
                                    // title: '',
                                }
                            },
                            {
                                path: 'medicalServiceHistory',
                                name: 'MedicalServiceHistory',
                                component: MedicalServiceHistory,
                                meta: {
                                    needAuth: true,
                                    // title: '',
                                }
                            }
                        ],
                        meta: {
                            needAuth: true,
                            // title: '',
                        }
                    },
                    {
                        path: 'user',
                        name: 'User',
                        component: User,
                        children: [
                            {
                                path: 'userInfor',
                                name: 'UserInfor',
                                component: UserInfor,
                                meta: {
                                    needAuth: true,
                                    // title: '',
                                }
                            },
                            {
                                path: 'doctorInfor',
                                name: 'DoctorUserInfor',
                                component: DoctorUserInfor,
                                meta: {
                                    needAuth: true,
                                    // title: '',
                                }
                            },
                            {
                                path: 'nurseInfor',
                                name: 'NurseUserInfor',
                                component: NurseUserInfor,
                                meta: {
                                    needAuth: true,
                                    // title: '',
                                }
                            },
                            {
                                path: 'historyDisease',
                                name: 'HistoryDisease',
                                component: HistoryDisease,
                                meta: {
                                    needAuth: true,
                                    // title: '',
                                }
                            },
                            {
                                path: 'historyService',
                                name: 'HistoryService',
                                component: HistoryService,
                                meta: {
                                    needAuth: true,
                                    // title: '',
                                }
                            },
                            {
                                path: 'modifyPassword',
                                name: 'ModifyPassword',
                                component: ModifyPassword,
                                meta: {
                                    needAuth: true,
                                    // title: '',
                                }
                            },
                            {
                                path: 'modifyInfor',
                                name: 'ModifyInfor',
                                component: ModifyInfor,
                                meta: {
                                    needAuth: true,
                                    // title: '',
                                }
                            },
                        ],
                        meta: {
                            needAuth: true,
                            // title: '',
                        }
                    },

                ],

                meta: {
                    needAuth: true,//该路由是否需要Authorize认证,
                    title: '颐养通',
                }
            },
            //管理员路由
            {
                path: '/admin',
                name: 'Admin',
                component: Admin,
                children: [
                    //个人信息展示
                    {
                        path: 'adminUserInfor',
                        name: 'AdminUserInfor',
                        component: AdminUserInfor,
                        meta: {
                            needAuth: true,
                            // title: '',
                        }
                    },
                    //个人信息修改
                    {
                        path: 'adminModifyInfo',
                        name: 'AdminModifyInfo',
                        component: AdminModifyInfo,
                        meta: {
                            needAuth: true,
                            // title: '',
                        }
                    },
                    //密码修改
                    {
                        path: 'adminModifyPassword',
                        name: 'AdminModifyPassword',
                        component: AdminModifyPassword,
                        meta: {
                            needAuth: true,
                            // title: '',
                        }
                    },
                    {
                        path: 'console',
                        name: 'Console',
                        component: Console,
                        meta: {
                            needAuth: true,
                            // title: '',
                        }
                    },
                    {
                        path: 'carerQualify',
                        name: 'CarerQualify',
                        component: CarerQualify,
                        meta: {
                            needAuth: true,
                            // title: '',
                        }
                    },
                    {
                        path: 'orgQualify',
                        name: 'OrgQualify',
                        component: OrgQualify,
                        meta: {
                            needAuth: true,
                            // title: '',
                        }
                    },
                    {
                        path: 'reportScan',
                        name: 'ReportScan',
                        component: ReportScan,
                        meta: {
                            needAuth: true,
                            // title: '',
                        }
                    },
                    {
                        path: 'carerStep1',
                        name: 'CarerStep1',
                        component: CarerStep1,
                        meta: {
                            needAuth: true,
                            // title: '',
                        }
                    },
                    {
                        path: 'carerStep2',
                        name: 'CarerStep2',
                        component: CarerStep2,
                        meta: {
                            needAuth: true,
                            // title: '',
                        }
                    },
                    {
                        path: 'carerStep3',
                        name: 'CarerStep3',
                        component: CarerStep3,
                        meta: {
                            needAuth: true,
                            // title: '',
                        }
                    },
                    {
                        path: 'detailReport',
                        name: 'DetailReport',
                        component: DetailReport,
                        meta: {
                            needAuth: true,
                            // title: '',
                        }
                    },
                    {
                        path: 'search',
                        name: 'Search',
                        component: Search,
                        meta: {
                            needAuth: true,
                            // title: '',
                        }
                    },
                    {
                        path: 'userInfo',
                        name: 'UserInfo',
                        component: UserInfo,
                        meta: {
                            needAuth: true,
                            // title: '',
                        }
                    },
                    {
                        path: 'orgStep1',
                        name: 'OrgStep1',
                        component: OrgStep1,
                        meta: {
                            needAuth: true,
                            // title: '',
                        }
                    },
                    {
                        path: 'admitCarer',
                        name: 'AdmitCarer',
                        component: AdmitCarer,
                        meta: {
                            needAuth: true,
                            // title: '',
                        }
                    },
                    {
                        path: 'admin',
                        name: 'Admin',
                        component: Admin,
                        meta: {
                            needAuth: true,
                            // title: '',
                        }
                    },
                    {
                        path: 'admitReport',
                        name: 'AdmitReport',
                        component: AdmitReport,
                        meta: {
                            needAuth: true,
                            // title: '',
                        }
                    },
                    {
                        path: 'admitOrg',
                        name: 'AdmitOrg',
                        component: AdmitOrg,
                        meta: {
                            needAuth: true,
                            // title: '',
                        }
                    }
                ],
                meta: {
                    needAuth: true,
                    // title: '',
                }
            },
            //护工路由
            {
                path: '/nurse',
                name: 'Nurse',
                component: Nurse,
                children:
                    [
                        // 护工首页
                        {
                            path: '/',
                            name: 'NurseHome',
                            component: NurseHome,
                            meta: {
                                needAuth: true,
                                // title: '',
                            }
                        },
                        // 订单页
                        {
                            path: 'takeorder',
                            name: 'TakeOrder',
                            component: TakeOrder,
                            children:
                                [
                                    {
                                        path: 'takeorderapply',
                                        name: 'TakeOrderApply',
                                        component: TakeOrderApply,
                                        meta: {
                                            needAuth: true,
                                            // title: '',
                                        }
                                    },
                                    {
                                        path: 'takeordermine',
                                        name: 'TakeOrderMine',
                                        component: TakeOrderMine,
                                        meta: {
                                            needAuth: true,
                                            // title: '',
                                        }
                                    }
                                ],
                            meta: {
                                needAuth: true,
                                // title: '',
                            }
                        },
                        //个人信息页
                        {
                            path: 'nurser',
                            name: 'Nurser',
                            component: Nurser,
                            children:
                                [
                                    {
                                        path: 'nurseinfor',
                                        name: 'NurserInfor',
                                        component: NurserInfor,
                                        meta: {
                                            needAuth: true,
                                        }
                                    },
                                    {
                                        path: 'seereport',
                                        name: 'SeeReport',
                                        component: SeeReport,
                                        meta: {
                                            needAuth: true,
                                        }
                                    },
                                    {
                                        path: 'nurserqualification',
                                        name: 'NurserQualification',
                                        component: NurserQualification,
                                        meta: {
                                            needAuth: true,
                                        }
                                    },
                                    {
                                        path: 'nurseModifyPassword',
                                        name: 'NurseModifyPassword',
                                        component: NurseModifyPassword,
                                        meta: {
                                            needAuth: true,
                                        }
                                    },
                                    {
                                        path: 'ModifyInfor',
                                        name: 'NurseModifyInfor',
                                        component: NurseModifyInfor,
                                        meta: {
                                            needAuth: true,
                                        }
                                    },
                                    {
                                        path: 'addqualification',
                                        name: 'AddQualification',
                                        component: AddQualification,
                                        meta: {
                                            needAuth: true,
                                        }
                                    }
                                ],
                            meta: {
                                needAuth: true,
                            }
                        },
                        // 消息中心页
                        {
                            path: 'nursemessage',
                            name: 'NurseMessage',
                            component: NurseMessage,
                            children:
                                [
                                    {
                                        path: 'nursenotice',
                                        name: 'NurseNotice',
                                        component: NurseNotice,
                                        meta: {
                                            needAuth: true,
                                        }
                                    },
                                    // {
                                    //     path: 'nurseorderinfo',
                                    //     name: 'NurseOrderInfo',
                                    //     component: NurseOrderInfo,
                                    //     meta: {
                                    //         needAuth: true,
                                    //     }
                                    // }
                                ],
                            meta: {
                                needAuth: true,
                            }
                        }

                    ],
                meta: {
                    needAuth: true,
                }
            },
            //医生路由
            {
                path: '/doctor',
                name: 'Doctor',
                component: Doctor,
                children: [
                    {
                        path: '/',
                        name: 'DoctorHome',
                        component: DoctorHome,
                        meta: {
                            needAuth: true,
                            // title: '',
                        }
                    },
                    {
                        path: 'medicalOrderApply',
                        name: 'MedicalOrderApply',
                        component: MedicalOrderApply,
                        meta: {
                            needAuth: true,
                            // title: '',
                        }
                    },
                    {
                        path: 'onlineInteraction',
                        name: 'OnlineInteraction',
                        component: OnlineInteraction,
                        meta: {
                            needAuth: true,
                            // title: '',
                        }
                    },
                    {
                        path: 'healthReport',
                        name: 'HealthReport',
                        component: HealthReport,
                        meta: {
                            needAuth: true,
                            // title: '',
                        }
                    },
                    {
                        path: 'serviceComment',
                        name: 'ServiceComment',
                        component: ServiceComment,
                        meta: {
                            needAuth: true,
                            // title: '',
                        }
                    },
                    {
                        path: 'clientManagement',
                        name: 'ClientManagement',
                        component: ClientManagement,
                        meta: {
                            needAuth: true,
                            // title: '',
                        }
                    },
                    {
                        path: 'doctorInfor',
                        name: 'DoctorInfor',
                        component: DoctorInfor,
                        meta: {
                            needAuth: true,
                            // title: '',
                        }
                    },
                    {
                        path: 'doctorQualification',
                        name: 'DoctorQualification',
                        component: DoctorQualification,
                        meta: {
                            needAuth: true,
                            // title: '',
                        }
                    },
                    {
                        path: 'doctoraddQualification',
                        name: 'DoctorAddQualification',
                        component: DoctorAddQualification,
                        meta: {
                            needAuth: true,
                            // title: '',
                        }
                    },
                    {
                        path: 'doctorModifyPassword',
                        name: 'DoctorModifyPassword',
                        component: DoctorModifyPassword,
                        meta: {
                            needAuth: true,
                            // title: '',
                        }
                    },
                    {
                        path: 'doctorModifyInfor',
                        name: 'DoctorModifyInfor',
                        component: DoctorModifyInfor,
                        meta: {
                            needAuth: true,
                            // title: '',
                        }
                    }
                ], meta: {
                    needAuth: true,
                    // title: '',
                }
            }
        ],
        meta: {
            needAuth: true,
        }

    },

    //client 护理模块，单独的页面框架
    {
        path: '/clientOrderFrame',
        name: 'ClientOrderFrame',
        component: ClientOrderFrame,
        children: [
            {
                path: 'placeCareOrder',
                name: 'PlaceCareOrder',
                component: PlaceCareOrder,
                meta: {
                    needAuth: true,
                    // title: '',
                }
            },
            {
                path: 'clientPayment',
                name: 'ClientPayment',
                component: ClientPayment,
                meta: {
                    needAuth: true,
                    // title: '',
                }
            },
        ],
        meta: {
            needAuth: true,
            // title: '',
        }
    },
    //登录、注册、忘记密码的路由
    {
        path: '/loginFrame',
        name: 'LoginFrame',
        component: LoginFrame,
        children: [
            {
                path: 'login',
                name: 'Login',
                component: Login,
                meta: {
                    needAuth: false,
                }
            },
            {
                path: 'register',
                name: 'Register',
                component: Register,
                meta: {
                    needAuth: false,
                }
            },
            {
                path: 'applyIns',
                name: 'ApplyIns',
                component: ApplyIns,
                meta: {
                    needAuth: false,
                }
            },
        ]
    },

]

const router = new VueRouter({
    mode: 'history',
    routes: routes
})


//全局前置路由
//来自from路由，目标to路由，next放行函数
router.beforeEach((to, from, next) => {

    console.log('to.name', to.name);
    //needAuth是否需要登录认证
    //不需登录直接进入
    if (!to.meta.needAuth) {
        console.log('不需登录直接进入')

        return next();
    } else {
        //检查cookies有无
        var token = cookies.get('token');

        if (token == null) {
            //无cookies，是重新进入会话
            //检查是否自动登录
            console.log('无cookies，检查是否自动登录');
            //检查local 有无refresh token
            var refresh_token = window.localStorage.getItem('refresh_token');
            if (refresh_token == null) {
                //无refresh,非自动登录，跳转login
                console.log('无refresh');
                return next('/loginFrame/login');
            }
            else {
                console.log('有refresh token');

                //有refresh，进行自动登录
                axios
                    .request({
                        url: "/API/user/refresh",
                        method: "get",
                        params: {
                            userrefreshtoken: refresh_token,
                        },
                    })
                    .then((res) => {
                        console.log("Refresh", res);
                        //保存access token、user ID、role到cookies
                        if (res.data.access_token == undefined) {
                            //refresh 失败
                            console.log('refresh 失败');
                            //清空local
                            window.localStorage.removeItem("refresh_token");
                            window.localStorage.removeItem("role");

                            return next('/loginFrame/login');
                        }
                        else {
                            //refresh 成功
                            console.log('refresh 成功');
                            var new_access_token = res.data.access_token;
                            var role = window.localStorage.getItem('role');
                            //分析json得到ID
                            let user_data = JSON.parse(res.data.user);
                            var user_id = user_data[0].USERID;
                            // let static_userID = '002022071200000002'
                            //存cookies
                            cookies.set("token", new_access_token, 'session');
                            cookies.set("role", role, 'session');
                            cookies.set("USERID", user_id, 'session');
                            //根据role，跳转对应主页
                            switch (role) {
                                //client
                                case "0":
                                    return next({ name: "Home" });
                                case "1":
                                    return next({ name: "NurseHome" });
                                case "2":
                                    return next({ name: "DoctorHome" });
                                case "3":
                                    return next({ name: "Console" });
                            }
                        }

                    })
                    .catch((err) => {
                        console.log(err);
                        //refresh 失败
                        console.log('refresh 失败');
                        //清空local
                        window.localStorage.removeItem("refresh_token");
                        window.localStorage.removeItem("role");

                        return next('/loginFrame/login');
                    });
            }
        }
        else {
            console.log('cookies 存在，进入页面.')
            var role = cookies.get('role');
            if (to.fullPath == '/') {
                //根据role，跳转对应主页
                switch (role) {
                    //client
                    case "0":
                        return next({ name: "Home" });
                    case "1":
                        return next({ name: "NurseHome" });
                    case "2":
                        return next({ name: "DoctorHome" });
                    case "3":
                        return next({ name: "Console" });
                }
            }
            //禁止不同角色串门

            console.log('to.fullPath', to.fullPath);
            if ((role == "0" && (to.fullPath == '/client' || to.fullPath.includes('/client/') || to.fullPath.includes('/clientOrderFrame'))) ||
                (role == "1" && (to.fullPath == '/nurse' || to.fullPath.includes('/nurse/'))) ||
                (role == "2" && (to.fullPath == '/doctor' || to.fullPath.includes('/doctor/'))) ||
                (role == "3" && (to.fullPath == '/admin' || to.fullPath.includes('/admin/')))) {
                //不要进无用的根路由
                if (from.name == 'Main') {

                    //根据role，跳转对应主页
                    switch (role) {
                        //client
                        case "0":
                            return next({ name: "Home" });
                        case "1":
                            return next({ name: "NurseHome" });
                        case "2":
                            return next({ name: "DoctorHome" });
                        case "3":
                            return next({ name: "Console" });
                    }
                }
                return next();
            }
            else {
                console.log('禁止串门,来自', from.fullPath);
                next(false);
                // return next(from.fullPath);
            }
        }
    }

    //检查local storage
    // var local_token = window.localStorage.getItem('token');

    // //有token
    // if (local_token != null) {
    //     //  获取存储token的开始时间
    //     const local_token_time = parseInt(window.localStorage.getItem('tokenStartTime'));
    //     console.log('local_token_time', local_token_time)
    //     //过期时间 7 天
    //     // const timeOver = 7 * 24 * 3600 * 1000;
    //     const timeOver = 3 * 24 * 60 * 60 * 1000;//

    //     let date = new Date().getTime();
    //     // console.log('date:', date);
    //     // console.log('local_token_time:', local_token_time);
    //     // console.log('timeOver:', timeOver);

    //     //local storage token 过期
    //     if (date - local_token_time > timeOver) {
    //         local_token = null;
    //         window.localStorage.removeItem("token");
    //         window.localStorage.removeItem("tokenStartTime");
    //         window.localStorage.removeItem("role");
    //         window.localStorage.removeItem("USERID");

    //     }
    //     //token未过期
    //     else {
    //         let local_role = window.localStorage.getItem('role');
    //         let local_userid = window.localStorage.getItem('USERID');
    //         //过期时间和local storage同步
    //         let left_time = parseInt((timeOver - date + local_token_time) / 1000);
    //         console.log('left_time:', left_time);
    //         //存入cookies
    //         cookies.set("token", local_token, left_time);
    //         cookies.set("role", local_role, left_time);
    //         cookies.set("USERID", local_userid, left_time);

    //     }
    // }


    // //needAuth是否需要登录认证
    // //不需登录直接进入
    // if (!to.meta.needAuth) {
    //     return next();
    // }
    // //需要登录 才可进入
    // else {
    //     //检查cookies
    //     //token
    //     var token = cookies.get('token');

    //     //没有token
    //     if (token == null) {
    //         //转到login界面
    //         console.log('cookies没有token，跳转登录');
    //         return next('/loginFrame/login');
    //     }
    //     else {
    //         console.log('cookies 检测，token存在，进入页面.')
    //         return next();
    //     }


    //     // // 首先读token
    //     // var token = window.localStorage.getItem('token');
    //     // //没有token，转到login界面
    //     // if (token == null) {
    //     //     return next('/loginFrame/login');
    //     // }

    //     // //  获取存储token的开始时间
    //     // const tokenStartTime = window.localStorage.getItem('tokenStartTime');
    //     // //过期时间 7 天
    //     // const timeOver = 7 * 24 * 3600 * 1000;
    //     // let date = new Date().getTime();
    //     // //token 过期，转到login界面
    //     // if (date - tokenStartTime > timeOver) {
    //     //     token = null;
    //     //     window.localStorage.removeItem("token");
    //     //     window.localStorage.removeItem("tokenStartTime");
    //     //     alert('登录状态过期，请重新登录');
    //     //     return next('/loginFrame/login');
    //     // }
    //     // //token未过期，放行
    //     // else {
    //     //     console.log('local storage 检测，token存在且未过期，进入页面.')
    //     //     return next();
    //     // }
    // }





})


//全局后置路由
//修改标题用
router.afterEach((to, from) => {
    document.title = to.meta.title || '颐养通';
})
//对外暴露router
export default router