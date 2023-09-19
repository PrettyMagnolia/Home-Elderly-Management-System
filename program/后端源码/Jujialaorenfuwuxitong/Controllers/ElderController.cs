using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using Jujialaorenfuwuxitong.Models;
using System.Data;
using System.Web.Script.Serialization;
using System.Text;
using System.Web;
using System.Linq;

namespace Jujialaorenfuwuxitong.Controllers
{/// <summary>
/// 老人表的数据
/// </summary>
    public class ElderController : BaseApiController
    {
        [TokenCheckFilter]
        // GET: api/Elder
        /// <summary>
        /// 获取老人列表
        /// </summary>
        /// <param name="minage">最小年龄</param>
        /// <param name="maxage">最大年龄</param>
        /// <param name="sex">性别</param>
        /// <param name="minh">最小身高</param>
        /// <param name="maxh">最大身高</param>
        /// <param name="maxw">最大体重</param>
        /// <param name="minw">最小体重</param>
        /// <param name="st">自理情况</param>
        /// <param name="add">住址</param>
        /// <param name="bantime">封禁时间，yyyy-mm-dd</param>
        /// <param name="phone">电话号</param>
        /// <returns>老人列表，错误时返回错误信息</returns>
        public HttpResponseMessage GetElder(string minage="0",string maxage="999",string sex=null,string minh="0",string maxh="999",string maxw="999", string minw = "0"
            , string st=null,string add=null,string phone=null,string bantime=null)
        {

            string str = "select userid,password,name,idcardno,age,phone,height,weight,address,urgent,situation,communityid,gender,photo,to_char(bantime,'yyyy-mm-dd') from senior";
            str += " Where 1=1";
            if (minage != "0" || maxage != "999")
                str += " and age between " + minage + " and " + maxage;
            if (minh != "0" || maxh != "999")
                str += " and height between " + minh + " and " + maxh;
            if(minw!="0"||maxw!="999")
                str +=" and weight between " + minw + " and " + maxw;
            if (sex!=null)
                str += " and gender='" + sex+"'";
            if (st != null)
                str += " and stuation='" + st+"'";
            if (add != null)
                str += " and address='" + add + "'";
            if (phone != null)
                str += " and phone='" + phone + "'";
            if (bantime != null)
                str += " and bantime >= to_date('" + bantime + "','yyyy-mm-dd')";
            Console.WriteLine(str);
            string message="";
            string json = OracleHelper.SelectSql_v2(str, ref message);
            if (json == null)
            {
                return new HttpResponseMessage
                {
                    Content = new StringContent("{\"status\":\"error\",\"message\":\"" + message + "\"}", Encoding.UTF8, "application/json")
                };
            }
            json=json.Replace("TO_CHAR(BANTIME,\\u0027YYYY-MM-DD\\u0027)", "bantime");
            return new HttpResponseMessage()
            {
                Content = new StringContent("{\"status\":\"success\",\"message\":" + json + "}", Encoding.UTF8, "application/json"),
            };

        }

        // GET: api/Elder/5
        /// <summary>
        /// 获取指定id的老人数据
        /// </summary>
        /// <param name="id">老人id</param>
        /// <returns>老人数据，错误时返回错误信息</returns>
        [TokenCheckFilter]
        [Route("api/elder/v2/{id}")]
        public HttpResponseMessage Get(string id)
        {
            string str = string.Format("select * from senior" +
                "         WHERE USERID={0}",id);
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
        /// 获取指定id的老人数据
        /// </summary>
        /// <param name="id">请求老人的id</param>
        /// <returns></returns>
        [TokenCheckFilter]
        
        public HttpResponseMessage GetById(string id)
        {
            string str = string.Format("select userid,password,name,idcardno,age,phone,height,weight,address,urgent,situation,communityid,gender,photo,to_char(bantime,'yyyy-mm-dd') from senior " +
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

        // POST: api/Elder
        /// <summary>
        /// 增加新的老人
        /// </summary>
        /// <param name="obj">密码，名字，身份证号，性别，年龄，电话，身高，体重，住址，紧急电话，自理情况，社区id
        /// {"pwd":"","name":"","idcardno":"","gender":"","age":"","phone":"",
        /// "height":"","weight":"","address":"","urgent":"","situation":"","communityid":""}</param>
        /// <returns>增加成功或失败,失败时附带错误信息,成功返回添加的信息</returns>
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
            Senior senior= new Senior(obj);
            string id = "00" + System.DateTime.Now.ToString("yyyyMMdd") + "00000001";
            string sql = string.Format("INSERT /*+ IGNORE_ROW_ON_DUPKEY_INDEX(SENIOR(USERID)) */ INTO SENIOR(USERID,PASSWORD,NAME,IDCARDNO,GENDER,AGE,PHONE,HEIGHT,WEIGHT,ADDRESS,URGENT,SITUATION,COMMUNITYID)" +
                "                   VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}')",
                id,senior.pwd,senior.name,senior.idCardNo,senior.gender,senior.age,senior.phone,senior.height,senior.weight,senior.address,senior.urgent,senior.situation,senior.communityId);
            

            if (OracleHelper.AddSql(sql, ref message,id))
            {
                id = message;
                message = string.Format(@"{{""id"":""{0}"",""name"":""{1}"",""address"":{2},""age"":""{3}"",""communityId"":""{4}"",""gender"":""{5}"",""height"":""{6}"",""idCardNo"":""{7}"",""phone"":""{8}"",""pwd"":""{9}"",""situation"":""{10}"",""urgent"":""{11}"",""weight"":""{12}""}}",
                id, senior.name, senior.address,senior.age, senior.communityId,senior.gender,senior.height,senior.idCardNo,senior.phone,senior.pwd,senior.situation,senior.urgent,senior.weight);
                return new HttpResponseMessage
                {
                    Content = new StringContent("{\"status\":\"true\",\"message\":"+message+"}", Encoding.UTF8, "application/json")
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

        // PUT: api/Elder/5
        /// <summary>
        /// 更新指定id的老人数据
        /// </summary>
        /// <param name="id">要更新的老人id</param>
        /// <param name="obj">密码，名字，身份证号，性别，年龄，电话，身高，体重，住址，紧急电话，自理情况，社区id,(为空则删除该项数据)
        /// {"pwd":"","name":"","idcardno":"","gender":"","age":"","phone":"",
        /// "height":"","weight":"","address":"","urgent":"","situation":"","communityid":""</param>
        /// <returns>更新成功或失败,失败时附带错误信息，成功返回修改后的信息</returns>

        public HttpResponseMessage Put(string id, dynamic obj)
        {
            Senior senior = new Senior(obj);
            string sql = string.Format(
                "UPDATE SENIOR " +
                "SET " +
                "NAME='{0}'," +
                "PASSWORD='{1}'," +
                "IDCARDNO='{2}'," +
                "GENDER='{3}'," +
                "AGE='{4}'," +
                "PHONE='{5}'," +
                "HEIGHT='{6}'," +
                "WEIGHT='{7}'," +
                "ADDRESS='{8}'," +
                "URGENT='{9}'," +
                "SITUATION='{10}'," +
                "COMMUNITYID='{11}'," +
                "BANTIME=to_date('{12}','yyyy-mm-dd')" +
                "WHERE " +
                "USERID='{13}'",
                senior.name, senior.pwd, senior.idCardNo, senior.gender, senior.age, senior.phone, senior.height, senior.weight, senior.address, senior.urgent, senior.situation, senior.communityId,obj.bantime,id);
            string message = "";

            if (OracleHelper.UpdateSql(sql, ref message))
            {
                message = string.Format(@"{{""id"":""{0}"",""name"":""{1}"",""address"":{2},""age"":""{3}"",""communityId"":""{4}"",""gender"":""{5}"",""height"":""{6}"",""idCardNo"":""{7}"",""phone"":""{8}"",""pwd"":""{9}"",""situation"":""{10}"",""urgent"":""{11}"",""weight"":""{12}"",""bantime"":""{13}""}}",
                id, senior.name, senior.address, senior.age, senior.communityId, senior.gender, senior.height, senior.idCardNo, senior.phone, senior.pwd, senior.situation, senior.urgent, senior.weight,obj.bantime);
                return new HttpResponseMessage
                {
                    Content = new StringContent("{\"status\":\"true\",\"message\":"+message+"}", Encoding.UTF8, "application/json")
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

        // DELETE: api/Elder/5
        /// <summary>
        /// 删除指定的老人数据
        /// </summary>
        /// <param name="id">老人id</param>
        /// <returns>删除成功或失败，失败时附带错误信息</returns>
        public HttpResponseMessage Delete(string id)
        {
            string sql = string.Format("DELETE FROM SENIOR WHERE USERID=" + id);
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

        /// <summary>
        /// 添加老人照片
        /// </summary>
        /// <param name="id">添加的老人id</param>
        /// <returns></returns>
        [TokenCheckFilter]

        [Route("api/elder/{id}/photo")]
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

                    string strPath = @"\photo\elder\" + name;
                    StreamHelp.StreamToFile(text, strPath);//需要用到下一步的帮助类将其保存为文件

                    string sql = string.Format("UPDATE SENIOR SET PHOTO='{0}' WHERE USERID='{1}'", strPath.Replace("." + suffix, @"\" + suffix), id);
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
