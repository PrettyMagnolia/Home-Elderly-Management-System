using Jujialaorenfuwuxitong.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Security;

namespace Jujialaorenfuwuxitong.Controllers
{
    [AllowAnonymous]
    //[Route("")]
    public class UserController : BaseApiController
    {
        [HttpGet]
        public object Login(string uName, string uPassword,string rule)
        {
            string str = "";
            switch (rule) 
            {
                case "0":
                    str = string.Format("select * from senior" +
                "         WHERE phone={0} and password={1}", uName, uPassword);
                    break;
                case "1":
                    str = string.Format("select * from nursingworker" +
                "         WHERE phone={0} and password={1}", uName, uPassword);
                    break;
                case "2":
                    str = string.Format("select * from doctor" +
                "         WHERE phone={0} and password={1}", uName, uPassword);
                    break;
                case "3":
                    str = string.Format("select * from manager" +
                "         WHERE phone={0} and pwd={1}", uName, uPassword);
                    break;
            }


            string message = "";
            string json = OracleHelper.SelectSql_v2(str, ref message);
            if (json == null)
            {
                return new HttpResponseMessage()
                {
                    Content = new StringContent("{\"error\":\"" + message + "\"}", Encoding.UTF8),
                };
            }
            else if (json=="[]")
            {
                return Json(new {data = "", msg = "用户名密码错误" });
            }
            //FormsAuthenticationTicket token = new FormsAuthenticationTicket(1, uName, DateTime.Now, DateTime.Now.AddMinutes(30), true, "", FormsAuthentication.FormsCookiePath);
            string token = uName  + "?" + System.DateTime.Now.ToString();
            string refreshtoken = rule+"&"+uName + "?" + System.DateTime.Now.AddDays(3).ToString();
            string _refreshtoken = Base64Helper.Base64Encode(refreshtoken);
            string sql = string.Format("MERGE INTO TOKEN USING (SELECT COUNT(*) CNT from TOKEN WHERE UNAME='{0}') T2 ON (T2.CNT<>0) WHEN MATCHED THEN" +
                " UPDATE SET TOKEN.REFRESHTOKEN='{1}' WHEN NOT MATCHED THEN INSERT VALUES('{2}','{3}')", uName, _refreshtoken, uName, _refreshtoken);
            if (!OracleHelper.ExecuteSql(sql, ref message))
            {
                return new HttpResponseMessage()
                {
                    Content = new StringContent("{\"error\":\"" + message + "\"}", Encoding.UTF8),
                };
            }
            
            //返回登录结果、用户信息、用户验证票据信息
            //var _token = FormsAuthentication.Encrypt(token);
            //将身份信息保存在session中，验证当前请求是否是有效请求
            var _token = Base64Helper.Base64Encode(token);
            LoginID = uName;
            TokenValue = _token;
            HttpContext.Current.Session[LoginID] = _token;
            //var temp = HttpContext.Current.Session[LoginID];
            return Json(new { access_token = _token, msg = "登录成功！", user = json,refresh_toekn=_refreshtoken });
        }
        [Route("api/user/refresh")]
        [HttpGet]
        public object RefreshToken(string userrefreshtoken)
        {
            string str = Base64Helper.Base64Decode(userrefreshtoken);
            string rule = str.Substring(0, str.IndexOf("&"));
            string uName = str.Substring(str.IndexOf("&")+1, str.IndexOf("?")-str.IndexOf("&")-1);
            DateTime dateTime = Convert.ToDateTime(str.Substring(str.IndexOf("?")+1));
            if(System.DateTime.Now.Day- dateTime.Day >= 3)
            {
                return new HttpResponseMessage { Content = new StringContent("{\"stauts\":\"error\",\"message\":\"过期了\"}", Encoding.UTF8) };
            }
            string sql = string.Format("select * from TOKEN WHERE UNAME='{0}' and REFRESHTOKEN='{1}'", uName, userrefreshtoken);
            string message = "";
            
            string json = OracleHelper.SelectSql_v2(sql, ref message);
            if (json == null)
            {
                return new HttpResponseMessage()
                {
                    Content = new StringContent("{\"error\":\"" + message + "\"}", Encoding.UTF8),
                };
            }
            else if (json == "[]")
            {
                return Json(new { data = "", msg = "用户名密码错误" });
            }
            string jsonstr = "";
            switch (rule)
            {
                case "0":
                    jsonstr = string.Format("select * from senior" +
                "         WHERE phone={0}", uName);
                    break;
                case "1":
                    jsonstr = string.Format("select * from nursingworker" +
                "         WHERE phone={0}", uName);
                    break;
                case "2":
                    jsonstr = string.Format("select * from doctor" +
                "         WHERE phone={0}", uName );
                    break;
                case "3":
                    jsonstr = string.Format("select * from manager" +
                "         WHERE phone={0}", uName);
                    break;
            }


            
            string userjson = OracleHelper.SelectSql_v2(jsonstr, ref message);
            if (userjson == null)
            {
                return new HttpResponseMessage()
                {
                    Content = new StringContent("{\"error\":\"" + message + "\"}", Encoding.UTF8),
                };
            }
            //FormsAuthenticationTicket token = new FormsAuthenticationTicket(1, uName, DateTime.Now, DateTime.Now.AddMinutes(30), true, "", FormsAuthentication.FormsCookiePath);
            string token = uName + "?" + System.DateTime.Now.ToString();
            string refreshtoken = rule+"&"+uName + "?" + System.DateTime.Now.AddDays(3).ToString();
            string _refreshtoken = Base64Helper.Base64Encode(refreshtoken);
            string str1 = string.Format("MERGE INTO TOKEN T1 USING (SELECT COUNT(*) CNT from T1 WHERE UNAME='{0}') T2 ON (T2.CNT<>0) WHEN MATCHED THEN" +
                " UPDATE SET T1.REFRESHTOKEN='{1}' WHEN NOT MATCHED THEN INSERT VALUES('{2}','{3}')", uName, _refreshtoken,uName,_refreshtoken);
            OracleHelper.AddSql(str1, ref message);
            //返回登录结果、用户信息、用户验证票据信息
            //var _token = FormsAuthentication.Encrypt(token);
            //将身份信息保存在session中，验证当前请求是否是有效请求
            var _token = Base64Helper.Base64Encode(token);
            LoginID = uName;
            TokenValue = _token;
            HttpContext.Current.Session[LoginID] = _token;
            //var temp = HttpContext.Current.Session[LoginID];
            return Json(new { access_token = _token, msg = "登录成功！", user = userjson, refresh_toekn = _refreshtoken });
        }
    }
}
