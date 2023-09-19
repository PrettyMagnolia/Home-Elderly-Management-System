import Mock from 'mockjs'
//Client Care
//--yy--
import CareServiceIntroAPI from './mockData/client/care/careServiceIntro'
import CareOrderDetailAPI from './mockData/client/care/careOrderDetail'
import careServiceDetailAPI from './mockData/client/care/careServiceDetail'
import careOrderALLAPI from './mockData/client/care/careServicePage'
import careServiceAllEvaAPI from './mockData/client/care/careServiceAllEva'
//--yy--

import ClientOrderIntroAPI from './mockData/nurse/ClientOrderIntro'
import DoctorOrderIntroAPI from './mockData/doctor/DoctorOrderIntro'


//管理员
import ConsoleAPI from './mockData/admin/consoleDetail'
import WaitingCarerDetailAPI from './mockData/admin/WaitingCarerDetail'
import CarerCardAPI from './mockData/admin/CarerCard'
import SearchResultAPI from './mockData/admin/SearchResult'


//登录注册
import getLoginAPI from './mockData/login/getLogin'

//拦截 /xxxx请求，返回这个调用函数的返回值
//client 护理模块

//--yy--
Mock.mock('/getClientCare_IndexIntro','get',CareServiceIntroAPI.getStaticData)
Mock.mock('/getClientCare_OrderDetail','get',CareOrderDetailAPI.getStaticData)
Mock.mock('/getClientCare_ServiceDetail','get',careServiceDetailAPI.getStaticData)
Mock.mock('/getClientCare_OrderALL','get',careOrderALLAPI.getStaticData)
Mock.mock('/getClientCare_CategoryEva','get',careServiceAllEvaAPI.getStaticData)

//--yy--

Mock.mock('/getIndexOrder','post',ClientOrderIntroAPI.getStaticData)

Mock.mock('/getDoctorService','post',DoctorOrderIntroAPI.getDoctorServiceData)

//管理员
Mock.mock('/getConsoleNum','get',ConsoleAPI.getStaticData)
Mock.mock('/getWaitingCarerDetail','get',WaitingCarerDetailAPI.getStaticData)
Mock.mock('/getCarerCard','get',CarerCardAPI.getStaticData)
Mock.mock('/getSearchResult','post',SearchResultAPI.getStaticData)

//login
Mock.mock('/login','get',getLoginAPI.getStaticData)