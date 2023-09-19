using Jujialaorenfuwuxitong.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace Jujialaorenfuwuxitong.Controllers
{
    public class NursingWorkerController : ApiController
    {
        [TokenCheckFilter]
        // GET: api/NursingWorker
        /// <summary>
        /// 获取护士列表
        /// </summary>
        /// <param name="minage">最小年龄</param>
        /// <param name="maxage">最大年龄</param>
        /// <param name="isid">机构id</param>
        /// <param name="userstatus">用户状态</param>
        /// <returns>护士列表</returns>
        public HttpResponseMessage GetNursing(string minage = "0", string maxage = "999",
             string isid = null, string userstatus = null,string phone=null,string bantime=null)
        {

            string str = "select userid,password,name,idcardno,gender,age,phone,description,photo,userstatus,institutionid,to_char(bantime,'yyyy-mm-dd') from nursingworker";
            str += " Where 1=1";
            if (minage!="0"||maxage!="999")
                str+=" and age between " + minage + " and " + maxage;
            if (isid != null)
                str += " and institutionid='" + isid + "'";
            if (userstatus != null)
                str += " and userstatus='" + userstatus + "'";
            if (phone != null)
                str += " and phone='" + phone + "'";
            if (bantime != null)
                str += " and bantime>=to_date('" + bantime + "','yyyy-mm-dd')";
            Console.WriteLine(str);
            string message = "";
            string json = OracleHelper.SelectSql_v2(str, ref message);
            if (json == null)
            {
                return new HttpResponseMessage
                {
                    Content = new StringContent("{\"status\":\"error\",\"message\":\"" + message + "\"}", Encoding.UTF8, "application/json")
                };
            }
            json = json.Replace("TO_CHAR(BANTIME,\\u0027YYYY-MM-DD\\u0027)", "bantime");
            return new HttpResponseMessage()
            {
                Content = new StringContent("{\"status\":\"success\",\"message\":" + json + "}", Encoding.UTF8, "application/json"),
            };
        }
        [TokenCheckFilter]
        // GET: api/NursingWorker/5
        /// <summary>
        /// 获取指定id的护士数据
        /// </summary>
        /// <param name="id">护士id</param>
        /// <returns>单个护士数据</returns>
        public HttpResponseMessage Get(string id)
        {
                string str = string.Format("select userid,password,name,idcardno,gender,age,phone,description,photo,userstatus,institutionid,to_char(bantime,'yyyy-mm-dd') from nursingworker" +
                    "         WHERE USERID={0}", id);
                string message = "";
            string json = OracleHelper.SelectSql_v2(str, ref message);
            if (json == null)
            {
                return new HttpResponseMessage
                {
                    Content = new StringContent("{\"status\":\"error\",\"message\":\"" + message + "\"}", Encoding.UTF8, "application/json")
                };
            }
            json = json.Replace("TO_CHAR(BANTIME,\\u0027YYYY-MM-DD\\u0027)", "bantime");
            return new HttpResponseMessage()
            {
                Content = new StringContent("{\"status\":\"success\",\"message\":" + json + "}", Encoding.UTF8, "application/json"),
            };
        }

        // POST: api/NursingWorker
        /// <summary>
        /// 增加新的护士
        /// </summary>
        /// <param name="obj">密码，姓名，身份证，性别，年龄，电话，简介照片，用户状态，机构id
        /// {"pwd":"","name":"","idcardno":"","gender":"","age":"","phone":"",
        /// "description":"","photo":"","userstatus":"","institutionid":""}</param>
        /// <returns>增加成功或失败,失败时附带失败信息，成功返回添加的信息</returns>
        public HttpResponseMessage Post(dynamic obj)
        {
            string[] str = { String.Format("select * from SENIOR WHERE PHONE='{0}'", obj.phone),
                String.Format("select * from NURSINGWORKER WHERE PHONE='{0}'", obj.phone),
                String.Format("select * from DOCTOR WHERE PHONE='{0}'", obj.phone),
                String.Format("select * from MANAGER WHERE PHONE='{0}'", obj.phone)};
            string message = "";
            for (int i = 0; i < str.Length; i++)
            {
                string json = OracleHelper.SelectSql_v2(str[i], ref message);
                if (json != "[]")
                {
                    message = "账号已被注册";
                    return new HttpResponseMessage
                    {
                        Content = new StringContent("{\"status\":\"error\",\"message\":\"" + message + "\"}", Encoding.UTF8, "application/json")
                    };
                }
            }
            NursingWorker nursingWorker = new NursingWorker(obj);
            string id = "11" + System.DateTime.Now.ToString("yyyyMMdd") + "00000001";
            string sql = string.Format("INSERT /*+ IGNORE_ROW_ON_DUPKEY_INDEX(NURSINGWORKER(USERID)) */ INTO NURSINGWORKER(USERID,PASSWORD,NAME,IDCARDNO,GENDER,AGE,PHONE,DESCRIPTION,photo,userstatus,institutionid)" +
                "                   VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}')",
                id, nursingWorker.pwd, nursingWorker.name, nursingWorker.idCardNo, nursingWorker.gender, nursingWorker.age, nursingWorker.phone, nursingWorker.description, nursingWorker.photo, nursingWorker.userstatus, nursingWorker.institutionid);
            message = "";

            if (OracleHelper.AddSql(sql, ref message,id))
            {
                id = message;
                message = string.Format(@"{{""id"":""{0}"",""name"":""{1}"",""age"":{2},""description"":""{3}"",""gender"":""{4}"",""idCardNo"":""{5}"",""institutionid"":""{6}"",""phone"":""{7}"",""photo"":""{8}"",""pwd"":""{9}"",""userstatus"":""{10}""}}",
                id, nursingWorker.name, nursingWorker.age, nursingWorker.description, nursingWorker.gender, nursingWorker.idCardNo, nursingWorker.institutionid, nursingWorker.phone, nursingWorker.photo, nursingWorker.pwd, nursingWorker.userstatus);
                return new HttpResponseMessage
                {
                    Content = new StringContent("{\"status\":\"true\",\"message\":" + message + "}", Encoding.UTF8, "application/json")
                };
            }
            else
            {
                return new HttpResponseMessage
                {
                    Content = new StringContent("{\"status\":\"error\",\"message\":\"" + message + "\"}", Encoding.UTF8, "application/json")
                };
            }
        }
        [TokenCheckFilter]

        // PUT: api/NursingWorker/5
        /// <summary>
        /// 更改指定id的护士数据
        /// </summary>
        /// <param name="id">护士id</param>
        /// <param name="obj">密码，姓名，身份证，性别，年龄，电话，简介照片，用户状态，机构id(为空则删除该项数据)
        /// {"pwd":"","name":"","idcardno":"","gender":"","age":"","phone":"",
        /// "description":"","photo":"","userstatus":"","institutionid":""}</param>
        /// <returns>成功或失败,失败时附带错误信息,成功返回修改后信息</returns>
        public HttpResponseMessage Put(string id, dynamic obj)
        {
            NursingWorker nursingWorker = new NursingWorker(obj);
            string sql = string.Format(
                "UPDATE NURSINGWORKER " +
                "SET " +
                "NAME='{0}'," +
                "PASSWORD='{1}'," +
                "IDCARDNO='{2}'," +
                "GENDER='{3}'," +
                "AGE='{4}'," +
                "PHONE='{5}'," +
                "DESCRIPTION='{6}'," +
                "PHOTO='{7}'," +
                "userstatus='{8}'," +
                "institutionid='{9}'," +
                "bantime=to_date('{10}','yyyy-mm-dd') " + 
                "WHERE " +
                "USERID='{11}'",
                nursingWorker.name, nursingWorker.pwd, nursingWorker.idCardNo, nursingWorker.gender, nursingWorker.age, nursingWorker.phone, nursingWorker.description, nursingWorker.photo, nursingWorker.userstatus, nursingWorker.institutionid,obj.bantime,id);
            string message = "";

            if (OracleHelper.UpdateSql(sql, ref message))
            {
                message = string.Format(@"{{""id"":""{0}"",""name"":""{1}"",""age"":{2},""description"":""{3}"",""gender"":""{4}"",""idCardNo"":""{5}"",""institutionid"":""{6}"",""phone"":""{7}"",""photo"":""{8}"",""pwd"":""{9}"",""userstatus"":""{10}"",""bantime"":""{11}""}}",
                id, nursingWorker.name, nursingWorker.age, nursingWorker.description, nursingWorker.gender, nursingWorker.idCardNo, nursingWorker.institutionid, nursingWorker.phone, nursingWorker.photo, nursingWorker.pwd, nursingWorker.userstatus, obj.bantime);
                return new HttpResponseMessage
                {
                    Content = new StringContent("{\"status\":\"true\",\"message\":" + message + "}", Encoding.UTF8, "application/json")
                };
            }
            else
            {
                return new HttpResponseMessage
                {
                    Content = new StringContent("{\"status\":\"error\",\"message\":\"" + message + "\"}", Encoding.UTF8, "application/json")
                };
            }
        }
        [TokenCheckFilter]

        // DELETE: api/NursingWorker/5
        /// <summary>
        /// 删除指定id的护士
        /// </summary>
        /// <param name="id">护士id</param>
        /// <returns>成功或失败,失败时附带错误信息</returns>
        public HttpResponseMessage Delete(string id)
        {
            string sql = string.Format("DELETE FROM NURSINGWORKER WHERE USERID=" + id);
            string message = "";
            if (OracleHelper.DeleteSql(sql, ref message))
            {
                return new HttpResponseMessage
                {
                    Content = new StringContent("{\"status\":\"true\",\"message\":\"\"}", Encoding.UTF8, "application/json")
                };
            }
            else
            {
                return new HttpResponseMessage
                {
                    Content = new StringContent("{\"status\":\"error\",\"message\":\"" + message + "\"}", Encoding.UTF8, "application/json")
                };
            }
        }
        [TokenCheckFilter]
        /// <summary>
        /// 获取指定id护士的简介
        /// </summary>
        /// <param name="id">护士id</param>
        /// <returns>失败时附带错误信息</returns>
        [Route("api/nursingworker/{id}/simple")]
        [HttpGet]
        public HttpResponseMessage GetSimple(string id)
        {
            string str = string.Format("select photo,name,age,gender,userid from nursingworker" +
                    "         WHERE USERID={0}", id);
            string message = "";
            string json = OracleHelper.SelectSql_v2(str, ref message);
            if (json == null)
            {
                return new HttpResponseMessage
                {
                    Content = new StringContent("{\"status\":\"error\",\"message\":\"" + message + "\"}", Encoding.UTF8, "application/json")
                };
            }
            return new HttpResponseMessage()
            {
                Content = new StringContent("{\"status\":\"success\",\"message\":" + json + "}", Encoding.UTF8, "application/json"),
            };
        }
        /// <summary>
        /// 添加护士照片
        /// </summary>
        /// <param name="id">护士id</param>
        /// <returns></returns>
        [TokenCheckFilter]
        [Route("api/nursingworker/{id}/photo")]
        [HttpPost]
        public HttpResponseMessage PostPhoto(string id)
        {
            string message = "出现错误！联系管理员";

            var files = HttpContext.Current.Request.Files;//首先先确定请求里夹带的文件数量

            if (files.AllKeys.Any())//如果存在文件
            {
                string name = files[0].FileName;
                string suffix = name.Substring(name.IndexOf(".") + 1);
                using (HttpClient client = new HttpClient())
                {
                    HttpContextBase HttpContext = (HttpContextBase)Request.Properties["MS_HttpContext"];

                    var text = HttpContext.Request.Files[0].InputStream;//获取到文件流

                    string strPath = @"\photo\nursingworker\" + name;
                    StreamHelp.StreamToFile(text, strPath);//需要用到下一步的帮助类将其保存为文件

                    string sql = string.Format("UPDATE NURSINGWORKER SET PHOTO='{0}' WHERE USERID='{1}'", strPath.Replace("." + suffix, @"\" + suffix), id);
                    if (OracleHelper.UpdateSql(sql, ref message))
                    {
                        message = strPath.Replace("." + suffix, @"\" + suffix);
                        return new HttpResponseMessage
                        {
                            Content = new StringContent("{\"status\":\"true\",\"message\":\"" + message + "\"}", Encoding.UTF8, "application/json")
                        };
                    }
                    else
                    {
                        return new HttpResponseMessage
                        {
                            Content = new StringContent("{\"status\":\"error\",\"message\":\"" + message + "\"}", Encoding.UTF8, "application/json")
                        };
                    }
                }
            }
            return new HttpResponseMessage
            {
                Content = new StringContent("{\"status\":\"error\",\"message\":\"" + message + "\"}", Encoding.UTF8, "application/json")
            };
        }
    }
}
