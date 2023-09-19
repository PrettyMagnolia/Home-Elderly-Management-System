using Jujialaorenfuwuxitong.Models;
using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace Jujialaorenfuwuxitong.Controllers
{
    public class EvaluationController : ApiController
    {
        [TokenCheckFilter]
        // GET: api/Evaluation
        /// <summary>
        /// 返回评论列表
        /// </summary>
        /// <param name="belong">评论人id，18位</param>
        /// <param name="mintime">评论的最早时间</param>
        /// <param name="maxtime">评论的最晚时间</param>
        /// <param name="grade">评分</param>
        /// <returns>评论列表,错误时返回错误信息</returns>
        public HttpResponseMessage GetEvaluations(string belong= "", string mintime = "2000-01-01 00:00:00",string maxtime="3000-01-01 00:00:00", string grade = null)
        {
            string str = string.Format("select EVALUATIONID,to_char(time,'yyyy-mm-dd hh24:mi:ss'),grade,message from EVALUATION" +
                "         WHERE EVALUATIONID like '{0}%'  and time between to_date('{1}','yyyy-mm-dd hh24:mi:ss') and to_date('{2}','yyyy-mm-dd hh24:mi:ss')", belong, mintime,maxtime);
            if (grade != null)
                str += " and grade= " + grade;
            string message = "";
            string json = OracleHelper.SelectSql_v2(str, ref message);
            json=json.Replace("TO_CHAR(TIME,\\u0027YYYY-MM-DDHH24:MI:SS\\u0027)", "TIME");
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
        [TokenCheckFilter]
        // GET: api/Evaluation/5
        /// <summary>
        /// 返回指定id的评论
        /// </summary>
        /// <param name="id">评论id，25位</param>
        /// <returns>评论的数据，错误时返回错误信息</returns>
        //[Route("")]
        public HttpResponseMessage Get(string id)
        {
            string str = string.Format("select EVALUATIONID,to_char(time,'yyyy-mm-dd hh24:mi:ss'),grade,message from EVALUATION" +
                "         WHERE EVALUATIONID = '{0}'", id);
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
                Content = new StringContent("{\"status\":\"success\",\"message\":" + json + "}", Encoding.UTF8, "application/json"),
            };
        }
        [TokenCheckFilter]

        // POST: api/Evaluation
        /// <summary>
        /// 添加新评论
        /// </summary>
        /// <param name="obj">评论人id，时间，评分，评价
        /// {"id":"","time":"1900-01-01 00:00:00","grade":'5',"message":""}</param>
        /// <returns>成功或失败,失败时附带错误信息,成功返回添加后的信息</returns>
        public HttpResponseMessage Post(dynamic obj)
        {
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

        // PUT: api/Evaluation/5
        /// <summary>
        /// 更新某条评论内容
        /// </summary>
        /// <param name="id">要更新的评论id</param>
        /// <param name="obj">时间，评分，评价(为空则删除该项数据)
        /// {"time":"1900-01-01 00:00:00","grade":'5',"message":""}</param>
        /// <returns>成功或失败,失败时附带错误信息,成功返回修改后的信息</returns>
        [TokenCheckFilter]
        public HttpResponseMessage Put(string id, dynamic obj)
        {
            Evaluation evaluation = new Evaluation(obj);
            string sql = string.Format(
                "UPDATE evaluation " +
                "SET " +
                "grade='{0}'," +
                "message='{1}'," +
                "time=to_date('{2}','yyyy-mm-dd hh24:mi:ss')," +
                "WHERE " +
                "evaluationID='{3}'",
                evaluation.grade, evaluation.message, evaluation.time, id);
            string message = "";

            if (OracleHelper.UpdateSql(sql, ref message))
            {
                message = string.Format(@"{{""id"":""{0}"",""time"":""{1}"",""grade"":""{2}"",""message"":""{3}""}}",
                id, evaluation.time, evaluation.grade, evaluation.message);
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

        // DELETE: api/Evaluation/5
        /// <summary>
        /// 删除指定id的评论
        /// </summary>
        /// <param name="id">删除评论的id</param>
        /// <returns>成功或失败,失败时附带错误信息</returns>
        public HttpResponseMessage Delete(string id)
        {
            string sql = string.Format("DELETE FROM Evaluation WHERE EvaluationID=" + id);
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
