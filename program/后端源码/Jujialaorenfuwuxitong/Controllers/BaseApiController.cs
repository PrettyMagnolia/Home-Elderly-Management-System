using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Jujialaorenfuwuxitong.Controllers
{
    public class BaseApiController : ApiController
    {

        /// <summary>
        /// 构造函数赋值
        /// </summary>
        public BaseApiController()
        {
            //TokenValue = HttpContext.Current.Session[LoginID] ?? "";
            //HttpContext.Current.Request.Headers.Add("TokenValue", TokenValue.ToString());
        }
        /// <summary>
        /// 数据库上下文
        /// </summary>
        //public WYDBContext db = WYDBContextFactory.GetDbContext();
        /// <summary>
        /// token值 登录后赋值请求api的时候添加到header中
        /// </summary>
        public static object TokenValue { get; set; } = "";
        /// <summary>
        /// 登录者账号
        /// </summary>
        public static string LoginID { get; set; } = "";
    }
}
