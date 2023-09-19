using Jujialaorenfuwuxitong.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace Jujialaorenfuwuxitong.Controllers
{
    public class ServiceController : ApiController
    {
        [TokenCheckFilter]
        // GET: api/Service
        /// <summary>
        /// 获取护理列表
        /// </summary>
        /// <param name="minmon">最小单价金额</param>
        /// <param name="maxmon">最大单价金额</param>
        /// <param name="type">类型</param>
        /// <param name="unit">计价单位</param>>
        /// <returns>护理列表,错误时返回错误信息</returns>
        public HttpResponseMessage Get(int minmon = 0, int maxmon = 9999, string type = null,string unit=null)
        {
            string str = string.Format("select serviceid,name,piece_price,unit,type,content, piece_price||'/'||unit from service" +
                "         WHERE piece_price between {0} and {1} ", minmon, maxmon);
            if (type != null)
                str += " and type = '" + type+"'";
            if (unit != null)
                str += "and unit='" + unit+"'";
            string message = "";
            string json = OracleHelper.SelectSql_v2(str, ref message);
            if (json == null)
            {
                return new HttpResponseMessage
                {
                    Content = new StringContent("{\"status\":\"error\",\"message\":" + message + "}", Encoding.UTF8, "application/json")
                };
            }
            json = json.Replace("PIECE_PRICE||\\u0027/\\u0027||UNIT", "per_price");
            return new HttpResponseMessage()
            {
                Content = new StringContent("{\"status\":\"success\",\"message\":" + json + "}", Encoding.UTF8, "application/json")
            };
        }
        [TokenCheckFilter]
        // GET: api/Service/5
        /// <summary>
        /// 获取指定id的护理选项
        /// </summary>
        /// <param name="id">指定护理的id</param>
        /// <returns>护理数据,错误时返回错误信息</returns>
        public HttpResponseMessage Get(string id)
        {
            string str = string.Format("select * from SERVICE" +
                "         WHERE SERVICEID= '{0}'", id);
            string message = "";
            string json = OracleHelper.SelectSql_v2(str, ref message);
            if (json == null)
            {
                return new HttpResponseMessage
                {
                    Content = new StringContent("{\"status\":\"error\",\"message\":" + message + "}", Encoding.UTF8, "application/json")
                };
            }
            return new HttpResponseMessage()
            {
                Content = new StringContent("{\"status\":\"success\",\"message\":" + json + "}", Encoding.UTF8, "application/json")
            };
            // return "value";
        }
        [TokenCheckFilter]
        // POST: api/Service
        /// <summary>
        /// 添加新的护理选项
        /// </summary>
        /// <param name="obj">名称，单价，计价单位，类型，简介
        /// {"name":"","piece_price":0,"unit":"","type":"","content":""}</param>
        /// <returns>成功或失败,失败时附带错误信息,成功返回添加的信息</returns>
        public HttpResponseMessage Post(dynamic obj)
        {
            Service service = new Service(obj);
            string id = "12" + System.DateTime.Now.ToString("yyyyMMdd") + "00000001";
            string sql = string.Format("INSERT /*+ IGNORE_ROW_ON_DUPKEY_INDEX(SERVICE(SERVICEID)) */ INTO SERVICE(SERVICEID,name,piece_price,unit,type,content)" +
                "                   VALUES('{0}','{1}',{2},'{3}','{4}','{5}')",
                id, service.name, service.piece_price, service.unit, service.type, service.content);
            string message = "";

            if (OracleHelper.AddSql(sql, ref message, id))
            {
                id = message;
                message = string.Format(@"{{""id"":""{0}"",""name"":""{1}"",""piece_price"":{2},""type"":""{3}"",""unit"":""{4}"",""content"":""{5}""}}",
                id, service.name, service.piece_price, service.type, service.unit,service.content);
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
        // PUT: api/Service/5
        /// <summary>
        /// 修改指定id的护理数据
        /// </summary>
        /// <param name="id">要修改的护理id</param>
        /// <param name="obj">名称，单价，计价单位，类型，简介(为空则删除该项数据)
        /// {"name":"","piece_price":0,"unit":"","type":"","content":""}</param>
        /// <returns>成功或失败,失败时附带错误信息，成功返回修改后信息</returns>
        public HttpResponseMessage Put(string id,dynamic obj)
        {
            Service service= new Service(obj);
            string sql = string.Format(
                "UPDATE service " +
                "SET " +
                "NAME='{0}'," +
                "piece_price={1}," +
                "unit='{2}'," +
                "type='{3}'," +
                "content='{4}'," +
                "WHERE " +
                "SERVICEID='{5}'",
                service.name, service.piece_price, service.unit, service.type, service.content, id);
            string message = "";

            if (OracleHelper.UpdateSql(sql, ref message))
            {

                message = string.Format(@"{{""id"":""{0}"",""name"":""{1}"",""piece_price"":{2},""type"":""{3}"",""unit"":""{4}"",""content"":""{5}""}}",
                id, service.name, service.piece_price, service.type, service.unit, service.content);
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
        // DELETE: api/Service/5
        /// <summary>
        /// 删除指定id的护理
        /// </summary>
        /// <param name="id"></param>
        /// <returns>成功或失败,失败时附带错误信息</returns>
        public HttpResponseMessage Delete(string id)
        {
            string sql = string.Format("DELETE FROM SERVICE WHERE SERVICEID=" + id);
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
    }
}
