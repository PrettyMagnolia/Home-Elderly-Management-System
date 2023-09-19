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
using Jujialaorenfuwuxitong.Models;
using Oracle.DataAccess.Client;

namespace Jujialaorenfuwuxitong.Controllers
{
    public class ServiceTypeController : ApiController
    {
        [TokenCheckFilter]
        // GET: api/ServiceType
        /// <summary>
        /// 获取护理类型列表
        /// </summary>
        /// <returns></returns>
        /// 
        public HttpResponseMessage Get()
        {
            string str = string.Format("select * from ServiceType");
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
                Content = new StringContent("{\"status\":\"success\",\"message\":" + json + "}", Encoding.UTF8, "application/json")
            };
        }
        [TokenCheckFilter]
        // GET: api/ServiceType/5
        /// <summary>
        /// 获取指定id护理类型的详情，含他属下的护理单项
        /// </summary>
        /// <param name="id">护理类型id</param>
        /// <returns></returns>
        /// 
        public HttpResponseMessage Get(string id)
        {
            string sql = string.Format("select serviceid,name,piece_price,unit,type,content, piece_price||'/'||unit from service" +
                "         WHERE type='" + id + "'");
            string message = "";
            string service_items_json = OracleHelper.SelectSql_v2(sql, ref message);

            if (service_items_json == null)
            {
                return new HttpResponseMessage
                {
                    Content = new StringContent("{\"status\":\"error\",\"message\":\"" + message + "\"}", Encoding.UTF8, "application/json")
                };
            }
            service_items_json = service_items_json.Replace("PIECE_PRICE||\\u0027/\\u0027||UNIT", "per_price");
            string str = string.Format("select * from ServiceType WHERE typeid=" + id);
            //string message = "";
            DataTable dt = OracleHelper.SelectSql(str, ref message);
            dt.Columns.Add("service_items", typeof(string));
            foreach (DataRow row in dt.Rows)
            {
                row["service_items"] = service_items_json;
            }
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            List<Dictionary<string, object>> parentRow = new List<Dictionary<string, object>>();
            Dictionary<string, object> childRow;
            foreach (DataRow row in dt.Rows)
            {
                childRow = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    childRow.Add(col.ColumnName, row[col]);
                }
                parentRow.Add(childRow);
            }
            string json = jsSerializer.Serialize(parentRow);
            //json = json.Replace("\\\"", "");
            //return json;
            if (json == null)
            {
                return new HttpResponseMessage
                {
                    Content = new StringContent("{\"status\":\"error\",\"message\":\"" + message + "\"}", Encoding.UTF8, "application/json")
                };
            }
            return new HttpResponseMessage()
            {
                Content = new StringContent("{\"status\":\"success\",\"message\":" + json + "}", Encoding.UTF8, "application/json")
            };
        }
        [TokenCheckFilter]
        // POST: api/ServiceType
        /// <summary>
        /// 增加新的护理类型
        /// </summary>
        /// <param name="obj">{"name":"", "photo":"", "introduce":"","tips":""}</param>
        /// <returns></returns>
        public HttpResponseMessage Post(dynamic obj)
        {
            string id = "1";
            string message = "";
            string sql = string.Format("insert /*+ IGNORE_ROW_ON_DUPKEY_INDEX(SERVICETYPE(TYPEID)) */  into SERVICETYPE(TYPEID,NAME,PHOTO,INTRODUCE,TIPS) VALUES('{0}','{1}','{2}','{3}','{4}')", id, obj.name, obj.photo, obj.introduce, obj.tips);
            bool re = false;
            try
            {
                OracleConnection conn = OracleHelper.DbConn(ref message, ref re);
                if (conn == null)
                {
                    re = false;
                    //message = "数据库连接对象为空";
                }
                else
                {
                    try
                    {
                        if (conn.State == System.Data.ConnectionState.Closed)
                        {
                            conn.Open();
                        }
                    }
                    catch (Exception ex)
                    {
                        re = false;
                        message = ex.Message.ToString();
                    }
                    OracleCommand command;
                    int idnum = int.Parse(id);
                    int count = 0;
                    while (count == 0)
                    {
                        command = new OracleCommand(sql, conn);
                        count = command.ExecuteNonQuery();
                        message = id;
                        idnum += 1;
                        command.Dispose();
                        //ToString("D8")，D表示十进制数，8表示位数
                        sql = sql.Replace(id, idnum.ToString());
                        id = idnum.ToString();
                    }
                    re = true;
                }

            }
            catch (Exception e)
            {
                message = e.Message;
                re = false;
            }
            if (re)
            {
                id = message;
                message = string.Format(@"{{""id"":""{0}"",""NAME"":""{1}"",""PHOTO"":""{2}"",""INTRODUCE"":""{3}"",""TIPS"":""{4}""}}",
                id, obj.name, obj.photo, obj.introduce, obj.tips);
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
        [TokenCheckFilter]
        // PUT: api/ServiceType/5
        /// <summary>
        /// 修改指定id的护理类型
        /// </summary>
        /// <param name="id">护理类型id</param>
        /// <param name="obj">{"name":"", "photo":"", "introduce":"","tips":""}</param>
        /// <returns></returns>
        public HttpResponseMessage Put(string id, dynamic obj)
        {
            //Service service = new Service(obj);
            string sql = string.Format(
                "UPDATE serviceTYPE " +
                "SET " +
                "NAME='{0}'," +
                "PHOTO='{1}'," +
                "INTRODUCE='{2}'," +
                "TIPS='{3}'" +
                "WHERE " +
                "TYPEID='{4}'",
                obj.name, obj.photo, obj.introduce, obj.tips, id);
            string message = "";

            if (OracleHelper.UpdateSql(sql, ref message))
            {

                message = string.Format(@"{{""id"":""{0}"",""name"":""{1}"",""photo"":{2},""introduce"":""{3}"",""tips"":""{4}""}}",
                id, obj.name, obj.photo, obj.introduce, obj.tips);
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
        [TokenCheckFilter]
        // DELETE: api/ServiceType/5
        /// <summary>
        /// 删除指定id的护理类型
        /// </summary>
        /// <param name="id">护理类型id</param>
        /// <returns></returns>
        public HttpResponseMessage Delete(string id)
        {
            string sql = string.Format("DELETE FROM SERVICETYPE WHERE TYPEID=" + id);
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
        [Route("api/servicetype/{id}/evaluation")]
        public HttpResponseMessage GetEvaluation(string id)
        {

            string str = string.Format("select to_char(time,'yyyy-mm-dd hh24:mi:ss'),GRADE,MESSAGE,PHONE from EVALUATION,SERVICERECORD,SENIOR WHERE SENIOR.userid=SUBSTR(evaluation.EVALUATIONID,0,18) and evaluation.evaluationid=servicerecord.evaluationid and TYPEID={0}", id);
            string message = "";
            string json = OracleHelper.SelectSql_v2(str, ref message);
            json = json.Replace("TO_CHAR(TIME,\\u0027YYYY-MM-DDHH24:MI:SS\\u0027)", "TIME");
            if (json == null)
            {
                return new HttpResponseMessage
                {
                    Content = new StringContent("{\"status\":\"error\",\"message\":\"" + message + "\"}", Encoding.UTF8, "application/json")
                };
            }
            return new HttpResponseMessage()
            {
                Content = new StringContent("{\"status\":\"success\",\"message\":" + json + "}", Encoding.UTF8, "application/json")
            };
        }
        [TokenCheckFilter]
        [Route("api/servicetype/{id}/photo")]
        [HttpPost]
        public HttpResponseMessage PostPhoto(string id)
        {
            string message = "出现错误！联系管理员";

            var files = HttpContext.Current.Request.Files;//首先先确定请求里夹带的文件数量
            
            if (files.AllKeys.Any())//如果存在文件
            {
                string name = files[0].FileName;
                string suffix = name.Substring(name.IndexOf(".")+1);
                using (HttpClient client = new HttpClient())
                {
                    HttpContextBase HttpContext = (HttpContextBase)Request.Properties["MS_HttpContext"];

                    var text = HttpContext.Request.Files[0].InputStream;//获取到文件流

                    string strPath = @"\photo\servicetype\" + name;
                    StreamHelp.StreamToFile(text, strPath);//需要用到下一步的帮助类将其保存为文件
                    
                    string sql = string.Format("UPDATE SERVICETYPE SET PHOTO='{0}' WHERE TYPEID='{1}'",strPath.Replace("."+suffix,@"\"+suffix),id);
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
    
