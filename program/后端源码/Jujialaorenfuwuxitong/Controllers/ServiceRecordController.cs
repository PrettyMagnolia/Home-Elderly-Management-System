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
    [RoutePrefix("api/servicerecord")]
    public class ServiceRecordController : ApiController
    {
        [TokenCheckFilter]
        //[Route("")]
        // GET: api/ServiceRecord
        /// <summary>
        /// 获取所有护理记录的列表
        /// </summary>
        /// <param name="typeID">类型id</param>
        /// <param name="min_start_time">最早开始时间</param>
        /// <param name="max_start_time">最迟开始时间</param>
        /// <param name="min_order_time">最早下单时间</param>
        /// <param name="max_order_time">最迟下单时间</param>
        /// <param name="status">状态</param>
        /// <param name="min_price">最低价格</param>
        /// <param name="max_price">最高价格</param>
        /// <param name="evaluationID">评价id</param>
        /// <param name="elderID">老人id</param>
        /// <param name="nursingworkerId">护工id</param>
        /// <param name="situation">自理情况</param>
        /// <param name="need">需求</param>
        /// <param name="in_wait_nurse_id">含有此待接单护工的id</param>
        /// <param name="not_in_wait_nurse_id">不含有此待接单护工的id</param>
        /// <param name="orderid">订单id</param>
        /// <returns></returns>
        public HttpResponseMessage Get(string typeID = null, string min_start_time = "2000-01-01 00:00:00", string max_start_time= "3000-01-01 00:00:00",
            string min_order_time = "2000-01-01 00:00:00", string max_order_time= "3000-01-01 00:00:00", string status = null, int min_price = 0, 
            int max_price=999999,string evaluationID = null, string elderID=null,string nursingworkerId=null,string situation=null,string need=null,string in_wait_nurse_id=null,string not_in_wait_nurse_id=null
            ,string orderid=null,string updated=null)
        {
            string str = string.Format("select RECORDID,TYPEID,to_char(start_time,'yyyy-mm-dd hh24:mi:ss'),to_char(order_time,'yyyy-mm-dd hh24:mi:ss'), " +
                "SERVICERECORD.address,status,price,evaluationID,elderID,name,nursingworkerID,SERVICERECORD.situation,need,orderid,updated " +
                "from SERVICERECORD,SENIOR Where SENIOR.USERID=elderid and start_time between to_date('{0}','yyyy-mm-dd hh24:mi:ss') and to_date('{1}','yyyy-mm-dd hh24:mi:ss') and " +
                "order_time between to_date('{2}','yyyy-mm-dd hh24:mi:ss') and to_date('{3}','yyyy-mm-dd hh24:mi:ss') and price between {4} and {5}",
                min_start_time,max_start_time,min_order_time,max_order_time,min_price,max_price);
            if (in_wait_nurse_id != null )
                str += " and recordid in (select recordid from servicenurse where nurseid=" + in_wait_nurse_id + ") ";
            if (not_in_wait_nurse_id != null)
                str += " and recordid not in (select recordid from servicenurse where nurseid=" + not_in_wait_nurse_id + ") ";
            if (typeID != null)
                str += " and typeid=" + typeID;
            if (status != null)
                str += " and status=" + status;
            if (evaluationID != null)
                str += " and evaluationID=" + evaluationID;
            if (elderID != null)
                str += " and elderID=" + elderID;
            if (nursingworkerId != null)
                str += " and nursingworkerID=" + nursingworkerId;
            if (situation != null)
                str += " and situation=" + situation;
            if (need != null)
                str += " and need=" + need;
            if (orderid != null)
                str += " and orderid=" + orderid;
            if (updated != null)
                str += " and updated=" + updated;
            string message = "";
            //string json = OracleHelper.SelectSql_v2(str, ref message);
            DataTable dt = OracleHelper.SelectSql(str, ref message);

            
            dt.Columns.Add("service_items", typeof(string));
            dt.Columns.Add("nurse_intro", typeof(string));
            dt.Columns.Add("nurse_wait", typeof(string));
            dt.Columns.Add("typename", typeof(string));
            dt.Columns.Add("order", typeof(string));
            foreach (DataRow row in dt.Rows)
            {
                string sql = string.Format("select serviceid,name,piece_price||'/'||unit,count from service natural join serviceitem " +
                " Where RECORDID='" + row["recordid"] + "'");

                string service_items_json = OracleHelper.SelectSql_v2(sql, ref message);
                string nurse_intro_json = "";
                if (service_items_json == null)
                {
                    return new HttpResponseMessage
                    {
                        Content = new StringContent("{\"status\":\"error\",\"message\":\"" + message + "\"}", Encoding.UTF8, "application/json")
                    };
                }
                service_items_json = service_items_json.Replace("PIECE_PRICE||\\u0027/\\u0027||UNIT", "per_price");
                row["service_items"] = service_items_json;
                var temp = row["nursingworkerid"].ToString();
                if (row["nursingworkerid"].ToString()=="")
                {
                    nurse_intro_json = "{}";
                    
                }
                else
                {
                    string nursesql = string.Format("select photo,name,age,gender,userid from nursingworker" +
                   "         WHERE USERID={0}", row["nursingworkerid"]);
                    nurse_intro_json = OracleHelper.SelectSql_v2(nursesql, ref message);
                    if (nurse_intro_json == null)
                    {
                        return new HttpResponseMessage
                        {
                            Content = new StringContent("{\"status\":\"error\",\"message\":\"" + message + "\"}", Encoding.UTF8, "application/json")
                        };
                    }
                }
                //row["service_items"] = service_items_json;
                row["nurse_intro"] = nurse_intro_json;
                string nursewaitsql = string.Format("select photo,name,age,gender,userid from nursingworker,servicenurse where nurseid=nursingworker.USERID and recordid={0}", row["recordid"]);
                if(in_wait_nurse_id!=null)
                {
                    nursewaitsql += " and nurseid=" + in_wait_nurse_id;
                }
                if (not_in_wait_nurse_id != null)
                {
                    nursewaitsql += " and nurseid!=" + not_in_wait_nurse_id;
                }
                string nurse_wait_json = OracleHelper.SelectSql_v2(nursewaitsql, ref message);
                if (nurse_wait_json == null) {
                    return new HttpResponseMessage
                    {
                        Content = new StringContent("{\"status\":\"error\",\"message\":\"" + message + "\"}", Encoding.UTF8, "application/json")
                    };
                }
                row["nurse_wait"] = nurse_wait_json;
                string typesql = string.Format("select name from servicetype where typeid={0}", row["typeid"]);
                string type_json = OracleHelper.SelectSql_v2(typesql, ref message);
                if (type_json == null) {
                    return new HttpResponseMessage
                    {
                        Content = new StringContent("{\"status\":\"error\",\"message\":\"" + message + "\"}", Encoding.UTF8, "application/json")
                    };
                }
                row["typename"] = type_json;
                string orderstr = string.Format("select * from Orders" +
                "         WHERE ORDERID = '{0}'", row["orderid"]);
                string orderjson = OracleHelper.SelectSql_v2(orderstr, ref message);
                if (orderjson == null)
                {
                    return new HttpResponseMessage
                    {
                        Content = new StringContent("{\"status\":\"error\",\"message\":\"" + message + "\"}", Encoding.UTF8, "application/json")
                    };
                }
                row["order"] = orderjson;


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
            if (json == null)
            {
                return new HttpResponseMessage
                {
                    Content = new StringContent("{\"status\":\"error\",\"message\":\"" + message + "\"}", Encoding.UTF8, "application/json")
                };
            }
            json = json.Replace("TO_CHAR(START_TIME,\\u0027YYYY-MM-DDHH24:MI:SS\\u0027)", "START_TIME");
            json = json.Replace("TO_CHAR(ORDER_TIME,\\u0027YYYY-MM-DDHH24:MI:SS\\u0027)", "ORDER_TIME");
            return new HttpResponseMessage()
            {
                Content = new StringContent("{\"status\":\"success\",\"message\":" + json + "}", Encoding.UTF8, "application/json"),
            };
        }
        [TokenCheckFilter]
        /// <summary>
        /// 获取简介列表，停用
        /// </summary>
        /// <param name="elderID">所属老人id</param>
        /// <returns></returns>
        [Route("simpleList")]
        [HttpGet]
        public HttpResponseMessage GetList(string elderID=null)
        {
            string message = "";
            string str = string.Format("select RECORDID,SERVICERECORD.TYPEID,SERVICETYPE.NAME,to_char(start_time,'yyyy-mm-dd hh24:mi:ss'),to_char(order_time,'yyyy-mm-dd hh24:mi:ss'), " +
    "status,price,NURSINGWORKER.name" +
    " from SERVICERECORD,NURSINGWORKER,SERVICETYPE WHERE NURSINGWORKERID=USERID and SERVICERECORD.TYPEID=SERVICETYPE.TYPEID ");
            if (elderID != null)
            {
                str += " and elderID='" + elderID + "'";
            }
            //string message = "";
            DataTable dt = OracleHelper.SelectSql(str, ref message);
            if (dt == null)
            {
                return new HttpResponseMessage
                {
                    Content = new StringContent("{\"status\":\"error\",\"message\":\"" + message + "\"}", Encoding.UTF8, "application/json")
                };
            }
            dt.Columns.Add("service_items", typeof(string));
            foreach (DataRow row in dt.Rows)
            {
                string sql = string.Format("select serviceid,name,piece_price||'/'||unit,count from service natural join serviceitem " +
                " Where RECORDID='" + row["recordid"] + "'");
                
                string service_items_json = OracleHelper.SelectSql_v2(sql, ref message);

                if (service_items_json == null)
                {
                    return new HttpResponseMessage
                    {
                        Content = new StringContent("{\"status\":\"error\",\"message\":\"" + message + "\"}", Encoding.UTF8, "application/json")
                    };
                }
                service_items_json = service_items_json.Replace("PIECE_PRICE||\\u0027/\\u0027||UNIT", "per_price");
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

            //return json;
            if (json == null)
            {
                return new HttpResponseMessage
                {
                    Content = new StringContent("{\"status\":\"error\",\"message\":\"" + message + "\"}", Encoding.UTF8, "application/json")
                };
            }
            json = json.Replace("TO_CHAR(START_TIME,\\u0027YYYY-MM-DDHH24:MI:SS\\u0027)", "START_TIME");
            json = json.Replace("TO_CHAR(ORDER_TIME,\\u0027YYYY-MM-DDHH24:MI:SS\\u0027)", "ORDER_TIME");
            return new HttpResponseMessage()
            {
                Content = new StringContent("{\"status\":\"success\",\"message\":" + json + "}", Encoding.UTF8, "application/json")
            };
        }
        [TokenCheckFilter]
        // GET: api/ServiceRecord/5
        /// <summary>
        /// 获取指定id护理记录详情，含其包含的护理单项和护工介绍
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        public HttpResponseMessage Get(string id)
        {

            string message = "";
            string sql = string.Format("select serviceid,name,piece_price||'/'||unit,count from service natural join serviceitem " +
                " Where RECORDID='" +id + "'");
            //string message = "";
            string service_items_json = OracleHelper.SelectSql_v2(sql, ref message);

            if (service_items_json == null)
            {
                return new HttpResponseMessage
                {
                    Content = new StringContent("{\"status\":\"error\",\"message\":\"" + message + "\"}", Encoding.UTF8, "application/json")
                };
            }
            service_items_json = service_items_json.Replace("PIECE_PRICE||\\u0027/\\u0027||UNIT", "per_price");
            string str = string.Format("select RECORDID,SERVICERECORD.TYPEID,SERVICETYPE.NAME,to_char(start_time,'yyyy-mm-dd hh24:mi:ss'),to_char(order_time,'yyyy-mm-dd hh24:mi:ss'), " +
    "address,status,price,evaluationid,elderid,nursingworkerid,situation,need,orderid,updated " +
    " from SERVICERECORD,SERVICETYPE WHERE SERVICERECORD.TYPEID=SERVICETYPE.TYPEID AND RECORDID=" + id);
            //string message = "";
            DataTable dt = OracleHelper.SelectSql(str, ref message);
            dt.Columns.Add("service_items", typeof(string));
            dt.Columns.Add("nurse_intro", typeof(string));
            dt.Columns.Add("nurse_wait", typeof(string));
            dt.Columns.Add("order", typeof(string));
            foreach (DataRow row in dt.Rows)
            {
                string nurse_intro_json = "";
                if (row["nursingworkerid"].ToString() != "")
                {
                    string nursesql = string.Format("select photo,name,age,gender,userid from nursingworker" +
                        "         WHERE USERID={0}", row["nursingworkerid"]);
                    nurse_intro_json = OracleHelper.SelectSql_v2(nursesql, ref message);
                    if (nurse_intro_json == null)
                    {
                        return new HttpResponseMessage
                        {
                            Content = new StringContent("{\"status\":\"error\",\"message\":\"" + message + "\"}", Encoding.UTF8, "application/json")
                        };
                    }
                }
                else {
                    nurse_intro_json = "{}";
                }
                string nursewaitsql = string.Format("select photo,name,age,gender,userid from nursingworker,servicenurse where nurseid=nursingworker.USERID and recordid={0}", row["recordid"]);
                string nurse_wait_json = OracleHelper.SelectSql_v2(nursewaitsql, ref message);
                if (nurse_wait_json == null)
                {
                    return new HttpResponseMessage
                    {
                        Content = new StringContent("{\"status\":\"error\",\"message\":\"" + message + "\"}", Encoding.UTF8, "application/json")
                    };
                }
                string orderstr = string.Format("select * from Orders" +
                "         WHERE ORDERID = '{0}'", row["orderid"]);
                string orderjson = OracleHelper.SelectSql_v2(orderstr, ref message);
                if (orderjson == null)
                {
                    return new HttpResponseMessage
                    {
                        Content = new StringContent("{\"status\":\"error\",\"message\":\"" + message + "\"}", Encoding.UTF8, "application/json")
                    };
                }
                row["order"] = orderjson;
                row["nurse_wait"] = nurse_wait_json;
                row["service_items"] = service_items_json;
                row["nurse_intro"] = nurse_intro_json;
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
            
            //return json;
            if (json == null)
            {
                return new HttpResponseMessage
                {
                    Content = new StringContent("{\"status\":\"error\",\"message\":\"" + message + "\"}", Encoding.UTF8, "application/json")
                };
            }
            json = json.Replace("TO_CHAR(START_TIME,\\u0027YYYY-MM-DDHH24:MI:SS\\u0027)", "START_TIME");
            json = json.Replace("TO_CHAR(ORDER_TIME,\\u0027YYYY-MM-DDHH24:MI:SS\\u0027)", "ORDER_TIME");
            return new HttpResponseMessage()
            {
                Content = new StringContent("{\"status\":\"success\",\"message\":" + json + "}", Encoding.UTF8, "application/json")
            };

           
        }
        [TokenCheckFilter]
        // POST: api/ServiceRecord
        /// <summary>
        /// 添加新的护理记录，含护理单项
        /// </summary>
        /// <param name="obj">{"typeid":"","start_time":"yyyy-mm-dd hh24:mi:ss",
        /// "order_time":"yyyy-mm-dd hh24:mi:ss","address":"","status":"","price":0,"evaluationid":"","elderid":"","nursingworkerid":"",
        /// "situation":"","need":"","serviceitems":[{"serviceid":"","count":""},{"serviceid":"","count":""}]}</param>
        /// <returns></returns>
        public HttpResponseMessage Post(dynamic obj)
        {
            //Senior senior = new Senior(obj);
            string id = "00" + System.DateTime.Now.ToString("yyyyMMdd") + "00000001";
            string sql = string.Format("INSERT /*+ IGNORE_ROW_ON_DUPKEY_INDEX(SERVICERECORD(RECORDID)) */ INTO SERVICERECORD" +
                "(RECORDID,TYPEID,START_TIME,ORDER_TIME,ADDRESS,STATUS,PRICE,EVALUATIONID,ELDERID,NURSINGWORKERID,SITUATION,NEED)" +
                "                   VALUES('{0}','{1}',to_date('{2}','yyyy-mm-dd hh24:mi:ss'),to_date('{3}','yyyy-mm-dd hh24:mi:ss'),'{4}','{5}',{6},'{7}','{8}','{9}','{10}','{11}')",
                id, obj.typeid, obj.start_time, obj.order_time, obj.address, obj.status, obj.price, obj.evaluationid, obj.elderid, obj.nursingworkerid, obj.situation, obj.need);
            bool re = false;
            string message = "";
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
                    OracleTransaction tran = conn.BeginTransaction();
                    if (id != null)
                    {
                        OracleCommand command;
                        int idnum = int.Parse(id.Substring(10, 8));
                        int count = 0;
                        while (count == 0)
                        {
                            command = new OracleCommand(sql, conn);
                            count = command.ExecuteNonQuery();
                            message = id;
                            idnum += 1;
                            command.Dispose();
                            //ToString("D8")，D表示十进制数，8表示位数
                            sql = sql.Replace(id, id.Substring(0, 10) + idnum.ToString("D8"));
                            id = id.Substring(0, 10) + idnum.ToString("D8");
                        }
                        re = true;

                    }
                    else
                    {
                        OracleCommand command = new OracleCommand(sql, conn);
                        int count = command.ExecuteNonQuery();
                        if (count > 0)
                        {
                            re = true;
                        }
                        else
                        {
                            re = false;
                            message = "数据插入失败";
                            tran.Rollback();
                        }
                        command.Dispose();
                    }
                    foreach (dynamic item in obj.serviceitems)
                    {
                        try
                        {
                            //sql += string.Format("insert into SERVICEITEM(RECORDID,SERVICEID,COUNT) VALUES('{0}','{1}',{2})\n", item.recordid, item.serviceid, item.count);
                            string str = string.Format("insert into SERVICEITEM(RECORDID,SERVICEID,COUNT) VALUES('{0}','{1}',{2})", message, item.serviceid, item.count);
                            OracleCommand command = new OracleCommand(str, conn);
                            command.ExecuteNonQuery();
                            command.Dispose();
                        }
                        catch (Exception e){
                            {
                                tran.Rollback();
                                conn.Close();
                                return new HttpResponseMessage
                                {
                                    Content = new StringContent("{\"status\":\"error\",\"message\":\"" + e.Message.ToString() + "\"}", Encoding.UTF8, "application/json")
                                };
                            }
                        }
                    }
                    tran.Commit();
                    conn.Close();
                }
            }
            catch (Exception ee)
            {
                re = false;
                message = ee.Message.ToString();
                
            }

            
            
            
            if (re==true)
            {
                id = message;
                message = string.Format(@"{{""recordid"":""{0}"",""typeid"":""{1}"",""start_time"":""{2}"",""order_time"":""{3}"",""address"":""{4}"",""status"":""{5}"",""price"":""{6}"",""evaluationid"":""{7}"",""elderid"":""{8}"",""nursingworkerid"":""{9}"",""situation"":""{10}"",""need"":""{11}"",""serviceitems"":{12}}}",
                id, obj.typeid, obj.start_time, obj.order_time, obj.address, obj.status, obj.price, obj.evaluationid, obj.elderid, obj.nursingworkerid, obj.situation, obj.need,obj.serviceitems);
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
        // PUT: api/ServiceRecord/5
        /// <summary>
        /// 修改指定id的护理记录
        /// </summary>
        /// <param name="id">护理记录id</param>
        /// <param name="obj">{"typeid":"","start_time":"yyyy-mm-dd hh24:mi:ss",
        /// "order_time":"yyyy-mm-dd hh24:mi:ss","address":"","status":"","price":0,"evaluationid":"","elderid":"","nursingworkerid":"",
        /// "situation":"","need":""}</param>
        /// <returns></returns>
        public HttpResponseMessage Put(string id, dynamic obj)
        {
            //Evaluation evaluation = new Evaluation(obj);
            //RECORDID,TYPEID,START_TIME,ORDER_TIME,ADDRESS,STATUS,PRICE,EVALUATIONID,ELDERID,NURSINGWORKERID,SITUATION,NEED
            string sql = string.Format(
                "UPDATE SERVICERECORD " +
                "SET " +
                "TYPEID='{0}'," +
                "START_TIME=to_date('{1}','yyyy-mm-dd hh24:mi:ss')," +
                "ORDER_TIME=to_date('{2}','yyyy-mm-dd hh24:mi:ss')," +
                "ADDRESS='{3}'," +
                "STATUS='{4}'," +
                "PRICE={5}," +
                "EVALUATIONID='{6}'," +
                "ELDERID='{7}'," +
                "NURSINGWORKERID='{8}'," +
                "SITUATION='{9}'," +
                "NEED='{10}'," +
                "UPDATED='{11}'" +
                " WHERE " +
                "recordID='{12}'",
                obj.typeid,obj.start_time, obj.order_time, obj.address, obj.status, obj.price, obj.evaluationid, obj.elderid, obj.nursingworkerid, obj.situation, obj.need,obj.updated,id);
            string message = "";

            if (OracleHelper.UpdateSql(sql, ref message))
            {
                message = string.Format(@"{{""recordid"":""{0}"",""typeid"":""{1}"",""start_time"":{2},""order_time"":""{3}"",""address"":""{4}"",""status"":""{5}"",""price"":""{6}"",""evaluationid"":""{7}"",""elderid"":""{8}"",""nursingworkerid"":""{9}"",""situation"":""{10}"",""need"":""{11}""}}",
                id, obj.typeid, obj.start_time, obj.order_time, obj.address, obj.status, obj.price, obj.evaluationid, obj.elderid, obj.nursingworkerid, obj.situation, obj.need);
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
        // DELETE: api/ServiceRecord/5
        /// <summary>
        /// 删除指定id的护理记录以及他对应的护理单项
        /// </summary>
        /// <param name="id">护理记录id</param>
        /// <returns></returns>
        public HttpResponseMessage Delete(string id)
        {

            string sql = string.Format("DELETE FROM SERVICERECORD WHERE RECORDID=" + id);
            string message = "";
            bool re = false;
            try
            {
                OracleConnection conn = OracleHelper.DbConn(ref message, ref re);
                if (conn == null)
                {
                    re = false;
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
                        return new HttpResponseMessage
                        {
                            Content = new StringContent("{\"status\":\"error\",\"message\":" + message + "}", Encoding.UTF8, "application/json")
                        };
                    }
                    OracleTransaction trans = conn.BeginTransaction();
                    OracleCommand command = new OracleCommand(sql, conn);
                    int count = command.ExecuteNonQuery();
                    if (count > 0)
                    {
                        //re = true;
                        command.Dispose();
                        try
                        {
                            string str = string.Format("DELETE FROM SERVICEITEM WHERE RECORDID=" + id);
                            command = new OracleCommand(str, conn);
                            command.ExecuteNonQuery();
                        }
                        catch(Exception e) {

                            re = false;
                            message = e.Message.ToString();
                            trans.Rollback();
                            return new HttpResponseMessage
                            {
                                Content = new StringContent("{\"status\":\"error\",\"message\":" + message + "}", Encoding.UTF8, "application/json")
                            };
                        }
                        command.Dispose();
                        try
                        {
                            string str = string.Format("DELETE FROM SERVICENURSE WHERE RECORDID=" + id);
                            command = new OracleCommand(str, conn);
                            command.ExecuteNonQuery();
                        }
                        catch(Exception e)
                        {
                            re = false;
                            message = e.Message.ToString();
                            trans.Rollback();
                            return new HttpResponseMessage
                            {
                                Content = new StringContent("{\"status\":\"error\",\"message\":" + message + "}", Encoding.UTF8, "application/json")
                            };
                        }
                        trans.Commit();
                        re = true;

                    }
                    else
                    {
                        re = false;
                        message = "数据删除失败";
                    }
                    command.Dispose();
                    conn.Close();
                }
            }
            catch (Exception ee)
            {
                re = false;
                message = ee.Message.ToString();
                return new HttpResponseMessage
                {
                    Content = new StringContent("{\"status\":\"error\",\"message\":" + message + "}", Encoding.UTF8, "application/json")
                };
            }
            
            if (re==true)
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
                    Content = new StringContent("{\"status\":\"error\",\"message\":" + message + "}", Encoding.UTF8, "application/json")
                };
            }
        }
        //[Route("")]
        [TokenCheckFilter]
        /// <summary>
        /// 增加护理单项
        /// </summary>
        /// <param name="obj">{"recordid":"","serviceid":"","count":1}</param>
        /// <returns></returns>
        [Route("items")]
        [HttpPost]
        
        public HttpResponseMessage Postitems(dynamic objs)
        {
            string message = "";
            bool status = true;
            foreach (dynamic obj in objs)
            {
                string sql = string.Format("insert into SERVICEITEM(RECORDID,SERVICEID,COUNT) VALUES('{0}','{1}',{2})", obj.recordid, obj.serviceid, obj.count);
                
                if (OracleHelper.AddSql(sql, ref message))
                {

                    
                    
                }
                else
                {
                    return new HttpResponseMessage
                    {
                        Content = new StringContent("{\"status\":\"error\",\"message\":\"" + message + "\"}", Encoding.UTF8, "application/json")
                    };
                }
            }
            message = string.Format(@"[{{""recordid"":""{0}"",""serviceid"":""{1}"",""count"":{2}}", objs[0].recordid, objs[0].serviceid, objs[0].count);
            foreach (dynamic obj in objs) {
                message += string.Format(@",{{""recordid"":""{0}"",""serviceid"":""{1}"",""count"":{2}}",
                      obj.recordid, obj.serviceid, obj.count);
            }
            message += "]";
            return new HttpResponseMessage
            {
                Content = new StringContent("{\"status\":\"true\",\"message\":" + message + "}", Encoding.UTF8, "application/json")
            };
        }
        [TokenCheckFilter]
        /// <summary>
        /// 添加接单护士
        /// </summary>
        /// <param name="obj">{"recordid":"","nurseid":""}</param>
        /// <returns></returns>
        [Route("nurse")]
        [HttpPost]
        public HttpResponseMessage PostNurse(dynamic obj) {
            string message = "";
            string sql = string.Format("insert into SERVICENURSE(RECORDID,NURSEID) VALUES('{0}','{1}')", obj.recordid, obj.nurseid);
            string str = string.Format("update SERVICERECORD SET UPDATED=1 WHERE RECORDID={0}", obj.recordid);
            OracleHelper.UpdateSql(str, ref message);
            if (OracleHelper.AddSql(sql, ref message))
            {
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
        /// <summary>
        /// 删除护士接单
        /// </summary>
        /// <param name="obj">["recordid":"","nurseid":""]</param>
        /// <returns></returns>
        [Route("nurse")]
        
        public HttpResponseMessage DeleteNurse(dynamic obj)
        {
            string message = "";
            string sql = string.Format("DELETE FROM SERVICENURSE WHERE RECORDID='{0}' AND NURSEID='{1}'", obj.recordid, obj.nurseid);
            string str = string.Format("update SERVICERECORD SET UPDATED=1 WHERE RECORDID={0}", obj.recordid);
            OracleHelper.UpdateSql(str, ref message);
            if (OracleHelper.DeleteSql(sql, ref message))
            {
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
        /// <summary>
        /// 确认护士接单
        /// </summary>
        /// <param name="obj">{"recordid":"","nurseid":"","elderid":"","money":"",""}</param>
        /// <returns></returns>
        [Route("nurse")]
        [HttpPut]
        public HttpResponseMessage NureseConfirm(dynamic obj) {
            string message = "";
            bool re = false;
            //string sql = String.Format("Delete from SERVICENURSE WHERE recordid={0}",obj.recordid);
            
            string id = obj.elderid + "0000001";
            //string message = "";
            string ordersql = string.Format("insert /*+ IGNORE_ROW_ON_DUPKEY_INDEX(ORDERS(ORDERID)) */  into ORDERS(ORDERID,ORDERSTATUS,METHOD,MONEY) VALUES('{0}','{1}','{2}',{3})", id, "未支付", "线上支付", obj.money);

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
                    OracleTransaction tran = conn.BeginTransaction();

                    /*OracleCommand command = new OracleCommand(sql, conn);
                    int count = command.ExecuteNonQuery();
                    if (count > 0)
                    {
                        re = true;
                    }
                    else
                    {
                        re = false;
                        message = "数据删除失败";
                        tran.Rollback();
                        return new HttpResponseMessage
                        {
                            Content = new StringContent("{\"status\":\"error\",\"message\":\"" + message + "\"}", Encoding.UTF8, "application/json")
                        };
                    }
                    command.Dispose();
                    */
                    int count=0;
                    OracleCommand command;
                    try
                    {
                        int idnum = int.Parse(id.Substring(18, 7));
                        count = 0;
                        while (count == 0)
                        {
                            command = new OracleCommand(ordersql, conn);
                            count = command.ExecuteNonQuery();
                            message = id;
                            idnum += 1;
                            command.Dispose();
                            //ToString("D8")，D表示十进制数，8表示位数
                            ordersql = ordersql.Replace(id, id.Substring(0, 18) + idnum.ToString("D7"));
                            id = id.Substring(0, 18) + idnum.ToString("D7");
                        }
                        id = message;
                    }
                    catch(Exception e)
                    {
                        tran.Rollback();
                        conn.Close();
                        return new HttpResponseMessage
                        {
                            Content = new StringContent("{\"status\":\"error\",\"message\":\"" + e.Message.ToString() + "\"}", Encoding.UTF8, "application/json")
                        };
                    }
                    string str = String.Format("UPDATE SERVICERECORD SET" +
                " nursingworkerid={0}," +
                " status=1," +
                " orderid={1}," +
                " updated=2 " +
                " WHERE recordid={2}", obj.nurseid, id, obj.recordid);
                    command = new OracleCommand(str, conn);
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        tran.Rollback();
                        conn.Close();
                        return new HttpResponseMessage
                        {
                            Content = new StringContent("{\"status\":\"error\",\"message\":\"" + e.Message.ToString() + "\"}", Encoding.UTF8, "application/json")
                        };
                    }
                    command.Dispose();
                    re = true;
                    tran.Commit();
                    conn.Close();
                }
            }
            catch (Exception ee)
            {
                re = false;
                message = ee.Message.ToString();

            }




            if (re == true)
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
        /// 评论
        /// </summary>
        /// <param name="obj">{"time":"","grade":"","message":""}</param>
        /// <param name="recordid"></param>
        /// <returns></returns>
        [Route("{recordid}/evaluation")]
        [HttpPost]
        public HttpResponseMessage PostEvaluation(dynamic obj,string recordid) {
            string id = obj.id + "0000001";
            string message = "";
            string sql = string.Format("insert /*+ IGNORE_ROW_ON_DUPKEY_INDEX(EVALUATION(EVALUATIONID)) */  into EVALUATION(EVALUATIONID,TIME,GRADE,message) VALUES('{0}',to_date('{1}','yyyy-mm-dd hh24:mi:ss'),'{2}','{3}')", id, obj.time, obj.grade, obj.message);
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
                    OracleTransaction trans = conn.BeginTransaction();
                    try
                    {
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
                        
                        string serviceRecordSql = string.Format("UPDATE SERVICERECORD SET EVALUATIONID='{0}' WHERE RECORDID='{1}'", message, recordid);
                        command = new OracleCommand(serviceRecordSql, conn);
                        command.ExecuteNonQuery();
                        trans.Commit();
                        re = true;
                    }
                    catch (Exception e) {
                        message = e.Message.ToString();
                        re = false;
                        trans.Rollback();
                    }
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
                message = string.Format(@"{{""id"":""{0}"",""time"":""{1}"",""grade"":""{2}"",""message"":""{3}""}}",
                id, obj.time, obj.grade, obj.message);
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
        /// <summary>
        /// 修改订单状态为已支付
        /// </summary>
        /// <param name="recordid">记录id</param>
        /// <returns></returns>
        [TokenCheckFilter]

        [Route("{recordid}/order")]
        [HttpPut]
        public HttpResponseMessage PutOrder(string recordid)
        {
            string message = "";
            string sql = "UPDATE ORDERS SET ORDERSTATUS='已支付' WHERE ORDERID in (select orderid from SERVICERECORD WHERE recordid=" + recordid + ")";
            if (OracleHelper.UpdateSql(sql,ref message))
            {
                return new HttpResponseMessage
                {
                    Content = new StringContent("{\"status\":\"true\",\"message\":\"" + message + "\"}", Encoding.UTF8, "application/json")
                };
            }
            return new HttpResponseMessage
            {
                Content = new StringContent("{\"status\":\"error\",\"message\":\"" + message + "\"}", Encoding.UTF8, "application/json")
            };
        }
    }
}
