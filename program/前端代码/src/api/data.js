//外部调用的接口
import axios from "./axios";
import cookies from "vue-cookies";
import router from "../router/router"

//hym
//获取页面token，用于data.js中的封装写法
var access_token = '';
export function setToken(str) {
    console.log("赋值token！");
    access_token = str;
    console.log(access_token)
}

//yy
//access token失效
export const AccessTokenFailed = () => {
    //检查有无refresh token
    var refresh_token = window.localStorage.getItem('refresh_token');
    if (refresh_token == null) {
        alert("登录信息过期！");

        //无 refresh token
        //清空所有信息
        window.localStorage.removeItem("refresh_token");
        window.localStorage.removeItem("role");
        cookies.remove("token");
        cookies.remove("role");
        cookies.remove("USERID");
        //直接跳转，重新登录
        router.push({
            name: "Login",
        });
    }
    else {
        //有refresh token

        //进行refresh
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
                    router.push({
                        name: "Login",
                    });
                }
                else {
                    //refresh 成功
                    console.log('refresh 成功');
                    var new_access_token = res.data.access_token;
                    var new_refresh_token = res.data.refresh_toekn;
                    var role = window.localStorage.getItem('role');
                    //分析json得到ID
                    let user_data = JSON.parse(res.data.user);
                    var user_id = user_data[0].USERID;
                    // let static_userID = '002022071200000002'
                    //存cookies
                    cookies.set("token", new_access_token, 'session');
                    cookies.set("role", role, 'session');
                    cookies.set("USERID", user_id, 'session');
                    //refresh token同时更新
                    window.localStorage.setItem("refresh_token", new_refresh_token);
                    //根据role，跳转对应主页
                    switch (role) {
                        //client
                        case "0":
                            router.push({
                                name: "Home",
                            });
                            break
                        case "1":
                            router.push({
                                name: "NurseHome",
                            });
                            break
                        case "2":
                            router.push({
                                name: "DoctorHome",
                            });
                            break
                        case "3":
                            router.push({
                                name: "Console",
                            });
                            break
                    }
                }

            })
            .catch((err) => {
                alert("登录信息过期！");

                console.log(err);
                //refresh 失败
                console.log('refresh 失败');
                //清空local
                window.localStorage.removeItem("refresh_token");
                window.localStorage.removeItem("role");

                router.push({
                    name: "Login",
                });
                //return next('/loginFrame/login');
            });
    }
};

//yy
//client 护理模块的首页，获取护理服务的简介用于展示
export const getClientCare_IndexIntro = (param) => {
    return axios.request({
        url: '/getClientCare_IndexIntro',
        method: 'get',
        data: param
    })
};

//yy
//clien 护理模块，点击某个服务类型，获取该服务类型的评价信息，仅供展示
export const getClientCare_CategoryEva = (param) => {
    return axios.request({
        url: '/getClientCare_CategoryEva',
        method: 'get',
        data: param
    })
};



//yy
//client 护理模块，获取某个服务的所有detail
export const getClientCare_ServiceDetail = (param) => {
    return axios.request({
        url: '/getClientCare_ServiceDetail',
        method: 'get',
        data: param
    })
};





//yy
//client 护理模块，我的服务订单，请求我的订单列表
export const getClientCare_OrderALL = (param) => {
    return axios.request({
        url: '/getClientCare_OrderALL',
        method: 'get',
        data: param
    })
}

//yy
//client 护理模块，获取一个护工的简介
export const getClientCare_NurseSimple = (param) => {
    return axios.request({
        url: '/getClientCare_NurseSimple',
        method: 'get',
        data: param
    })
}

//yy
//登录
export const getLogin = (param) => {
    return axios.request({
        url: '/login',
        method: 'get',
        data: param
    })
}


export const getIndexOrder = (param) => {
    return axios.request({
        url: '/getIndexOrder',
        method: 'post',
        data: param
    })
}



export const getDoctorService = (param) => {
    return axios.request({
        url: '/getDoctorService',
        method: 'post',
        data: param
    })
};

//start of Admin's API
//管理员   hym
export const getReportAuditLog = (param) => {
    return axios.request({
        url: '/API/auditLog/待审核/report',
        method: 'get',
        data: param,
        headers: {
            TokenValue: access_token,
        }
    })
}

export const getInstitutionAuditLog = (param) => {
    return axios.request({
        url: '/API/auditLog/待审核/institution',
        method: 'get',
        data: param,
        headers: {
            TokenValue: access_token,
        }
    })
}

export const getQualificationAuditLog = (param) => {
    return axios.request({
        url: '/API/auditLog/待审核/qualification',
        method: 'get',
        data: param,
        headers: {
            TokenValue: access_token,
        }
    })
}

export const getManager = (param) => {
    return axios.request({
        url: '/API/Manager/' + param,//param传入ID给后端，要用字符串连接更改url，param是string类型的ID
        method: 'get',
        data: param,
        headers: {
            TokenValue: access_token,
        }
    })
}

export const getWaitingCarerDetail = (param) => {
    return axios.request({
        url: '/API/Qualifacation',
        method: 'get',
        data: param,
        headers: {
            TokenValue: access_token,
        }
    })
}

export const getWaitingReportDetail = (param) => {
    return axios.request({
        url: '/API/Report',
        method: 'get',
        data: param,
        headers: {
            TokenValue: access_token,
        }
    })
}

export const getWaitingOrgDetail = (param) => {
    return axios.request({
        url: '/API/Institution',
        method: 'get',
        data: param,
        headers: {
            TokenValue: access_token,
        }
    })
}

export const getCarerCard = (param) => {
    return axios.request({
        url: '/getCarerCard',
        method: 'get',
        data: param,
        headers: {
            TokenValue: access_token,
        }
    })
}

export const postCarerResult = (param) => {
    return axios.request({
        url: '/postCarerResult',
        method: 'post',
        data: param,
        headers: {
            TokenValue: access_token,
        }
    })
}

export const getUserInfoForAdmin = (param) => {
    return axios.request({
        url: '/getUserInfoForAdmin',
        method: 'get',
        data: param,
        headers: {
            TokenValue: access_token,
        }
    })
}

export const getReportCard = (param) => {
    return axios.request({
        url: '/API/Report/' + param,
        method: 'get',
        data: param,
        headers: {
            TokenValue: access_token,
        }
    })
}

export const postReport = (obj) => {
    return axios.request({
        url: '/API/Report',
        method: 'post',
        data: obj,
        headers: {
            TokenValue: access_token,
        }
    })
}

export const postCarer = (param) => {
    return axios.request({
        url: '/API/NursingWorker',
        method: 'post',
        data: param,
        headers: {
            TokenValue: access_token,
        }
    })
}

export const postOrg = (param) => {
    return axios.request({
        url: '/API/Institution',
        method: 'post',
        data: param,
        headers: {
            TokenValue: access_token,
        }
    })
}

export const getOrgCard = (param) => {
    return axios.request({
        url: '/API/Institution/' + param,
        method: 'get',
        data: param,
        headers: {
            TokenValue: access_token,
        }
    })
}

export const postOrgResult = (param) => {
    return axios.request({
        url: '/postOrgResult',
        method: 'post',
        data: param,
        headers: {
            TokenValue: access_token,
        }
    })
}

export const putReportCard = (id, obj) => {
    return axios.request({
        url: '/API/Report/' + id,
        method: 'put',
        data: obj,
        headers: {
            TokenValue: access_token,
        }
    })
}

export const putOrgCard = (id, obj) => {
    return axios.request({
        url: '/API/Institution/' + id,
        method: 'put',
        data: obj,
        headers: {
            TokenValue: access_token,
        }
    })
}

export const getAllElder = (param) => {
    return axios.request({
        url: '/API/Elder',
        method: 'get',
        data: param,
        headers: {
            TokenValue: access_token,
        }
    })
}

export const getAllNurse = (param) => {
    return axios.request({
        url: '/API/NursingWorker',
        method: 'get',
        data: param,
        headers: {
            TokenValue: access_token,
        }
    })
}

export const getAllDoctor = (param) => {
    return axios.request({
        url: '/API/Doctor',
        method: 'get',
        data: param,
        headers: {
            TokenValue: access_token,
        }
    })
}

export const postQualification = (param) => {
    return axios.request({
        url: '/API/Qualifacation',
        method: 'post',
        data: param,
        headers: {
            TokenValue: access_token,
        }
    })
}

export const getQualificationCard = (uid) => {
    return axios.request({
        url: '/API/Qualifacation/' + uid,
        method: 'get',
        data: uid,
        headers: {
            TokenValue: access_token,
        }
    })
}

export const putNurseDisable = (id, obj) => {
    return axios.request({
        url: '/API/NursingWorker/' + id,
        method: 'put',
        data: obj,
        headers: {
            TokenValue: access_token,
        }
    })
}


export const putDoctorDisable = (uid, myobj) => {
    return axios.request({
        url: '/API/Doctor/',
        method: 'put',
        params: { id: uid },
        data: myobj,
        headers: {
            TokenValue: access_token,
        }
    })
}

export const getNurseCard = (uid) => {
    return axios.request({
        url: '/API/NursingWorker/' + uid,
        method: 'get',
        data: uid,
        headers: {
            TokenValue: access_token,
        }
    })
}

export const getDoctorCard = (uid) => {
    return axios.request({
        url: '/API/Doctor/id',
        method: 'get',
        params: { doctorid: uid },
        headers: {
            TokenValue: access_token,
        }
    })
}

export const putQualification = (obj) => {
    return axios.request({
        url: '/API/Qualifacation',
        method: 'put',
        data: obj,
        headers: {
            TokenValue: access_token,
        }
    })
}

export const getCommunityCard = (cid) => {
    return axios.request({
        url: '/API/community/id',
        method: 'get',
        params: { id: cid },
        headers: {
            TokenValue: access_token,
        }
    })
}//end of Admin's API

// lyf
// 护工首页获取指定ID的护工信息
export const getNurseInfo = (uid) => {
    return axios.request({
        url: "/API/NursingWorker" + "/" + uid,
        method: 'get',
        data: uid
    })
}

// lyf
// 护工订单界面获取所有护理类型
export const getAllCareType = () => {
    return axios.request({
        url: "/API/ServiceType",
        method: 'get',
    })
}

// lyf
// 护工接单界面获取所有能够接单的订单信息
export const getOrderInfo = (uid, filter_info) => {
    return axios.request({
        url: "/API/ServiceRecord",
        method: 'get',
        params: {
            status: 0,
            min_order_time: filter_info.start_time,
            max_order_time: filter_info.end_time,
            typeID: filter_info.typeid,
            not_in_wait_nurse_id: uid
        },
    })
}

// lyf
// 护工订单界面根据老人ID获取对应的老人信息
export const getElderInfoByID = (id) => {
    return axios.request({
        url: "/API/Elder" + "/" + id,
        method: 'get',
    })
}

// lyf
// 护工我的订单界面获取该护工正在进行中和已完成的订单信息
export const getOrderInfoByNursingWorkerID = (nwid, filter_info) => {
    return axios.request({
        url: "/API/ServiceRecord",
        method: 'get',
        params: {
            nursingworkerId: nwid,
            status: filter_info.state,
            typeID: filter_info.typeid
        },
    })
}

// lyf
// 护工查看订单界面获取该护工已经申请的订单信息
export const getOrderInfoByNursingWorkerID_1 = (uid, filter_info) => {
    return axios.request({
        url: "/API/ServiceRecord",
        method: 'get',
        params: {
            status: 0,
            typeID: filter_info.typeid,
            not_in_wait_nurse_id: uid
        },
    })
}




