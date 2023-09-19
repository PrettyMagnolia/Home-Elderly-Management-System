using Jujialaorenfuwuxitong.Models;
using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace Jujialaorenfuwuxitong.Controllers
{
    public class OrderController : ApiController
    {
        [TokenCheckFilter]
        // GET: api/Order
        /// <summary>
        /// 获取订单列表
        /// </summary>
        /// <param name="belong">订单所属的id，18位</param>
        /// <param name="method">支付方法</param>
        /// <param name="status">状态</param>
        /// <param name="minmon">最小金额</param>
        /// <param name="maxmon">最大金额</param>
        /// <returns>订单列表，错误时返回错误信息</returns>
        public HttpResponseMessage GetOrders(string belong="",string method=null,string status=null,int minmon=0,int maxmon=99999)
        {
            string str = string.Format("select * from Orders" +
                "         WHERE ORDERID like '{0}%' and money between {1} and {2} ", belong, minmon,maxmon);
            if (method != null)
                str += " and method = " + method;
            if (status != null)
                str += " and orderstatus = " + status;
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
                Content = new StringContent("{\"status\":\"success\",\"message\":" + json + "}", Encoding.UTF8, "application/json"),
            };
        }
        [TokenCheckFilter]
        // GET: api/Order/5
        /// <summary>
        /// 获取指定id的订单
        /// </summary>
        /// <param name="id">订单id,25位</param>
        /// <returns>订单数据，错误时返回错误信息</returns>
        //[Route("")]
        public HttpResponseMessage Get(string id)
        {
            string str = string.Format("select * from Orders" +
                "         WHERE ORDERID = '{0}'", id);
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
                Content = new StringContent("{\"status\":\"success\",\"message\":\"" + json + "\"}", Encoding.UTF8, "application/json"),
            };
        }
        [TokenCheckFilter]
        // POST: api/Order
        /// <summary>
        /// 添加新订单
        /// </summary>
        /// <param name="obj">所属人，状态，支付方式，金额
        /// {id:"",status:"",method:"",money:}</param>
        /// <returns>成功或失败,失败时附带错误信息，成功返回添加的信息</returns>

        public HttpResponseMessage Post(dynamic obj)
        {
            string id = obj.id+"0000001";
            string message = "";
            string sql = string.Format("insert /*+ IGNORE_ROW_ON_DUPKEY_INDEX(ORDERS(ORDERID)) */  into ORDERS(ORDERID,ORDERSTATUS,METHOD,MONEY) VALUES('{0}','{1}','{2}',{3})", id, obj.status, obj.method, obj.money);
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
                    int idnum = int.Parse(id.Substring(18, 7));
                    int count = 0;
                    while (count == 0) 
                    {
                        command = new OracleCommand(sql, conn);
                        count = command.ExecuteNonQuery();
                        message = id;
                        idnum += 1;
                        command.Dispose();
                        //ToString("D8")，D表示十进制数，8表示位数
                        sql = sql.Replace(id, id.Substring(0, 18) + idnum.ToString("D7"));
                        id = id.Substring(0, 18) + idnum.ToString("D7");
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
                message = string.Format(@"{{""id"":""{0}"",""status"":""{1}"",""method"":""{2}"",""money"":{3}}}",
                id,obj.status, obj.method, obj.money);
                return new HttpResponseMessage
                {
                    Content = new StringContent("{\"status\":\"true\",\"message\":\"" + message + "\"}", Encoding.UTF8, "application/json")
                };
            }
            else
            {
                return new HttpResponseMessage
                {
                    Content = new StringContent("{\"status\":\"error\",\"message\":\"" + message + "\"}", Encoding.UTF8,"application/json")
                };
            }
        }
        [TokenCheckFilter]
        // PUT: api/Order/5
        /// <summary>
        /// 更新指定id的订单
        /// </summary>
        /// <param name="id">要更新的订单id</param>
        /// <param name="obj">所属人，状态，支付方式，金额(为空则删除该项数据)
        /// {id:"",status:"",method:"",money:}</param>
        /// <returns>成功或失败,失败时附带错误信息，成功返回修改后信息</returns>
        public HttpResponseMessage Put(string id, dynamic obj)
        {
            Order order = new Order(obj);
            string sql = string.Format(
                "UPDATE ORDERS " +
                "SET " +
                "orderstatus='{0}'," +
                "method='{1}'," +
                "money='{2}'," +
                "WHERE " +
                "ORDERID='{3}'",
                order.orderStatus, order.method, order.money, id);
            string message = "";

            if (OracleHelper.UpdateSql(sql, ref message))
            {
                message = string.Format(@"{{""id"":""{0}"",""status"":""{1}"",""method"":""{2}"",""money"":{3}}}",
                id, order.orderStatus, order.method, order.money);
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
        // DELETE: api/Order/5
        /// <summary>
        /// 删除指定id的订单
        /// </summary>
        /// <param name="id">要删除的订单id</param>
        /// <returns>成功或失败,失败时附带错误信息</returns>
        public HttpResponseMessage Delete(string id)
        {
            string sql = string.Format("DELETE FROM ORDERS WHERE ORDERID='" + id +"'");
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
