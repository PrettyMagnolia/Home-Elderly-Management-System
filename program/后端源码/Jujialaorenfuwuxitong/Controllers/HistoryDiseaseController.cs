using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Jujialaorenfuwuxitong.Models;
using System.Data;

namespace Jujialaorenfuwuxitong.Controllers
{
    /// <summary>
    /// 历史疾病表的数据
    /// </summary>
    [RoutePrefix("api/history")]
    public class HistoryDiseaseController : ApiController
    {
        /// <summary>
        /// 获取既往病史表所有信息
        /// </summary>
        /// <returns>老人历史疾病列表，json数据</returns>
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetbyElder()
        {
            string str = string.Format("select  * from historydisease");

            string message = "";
            DataTable dt = OracleHelper.SelectSql(str, ref message);

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
            return Json(parentRow);
        }

        /// <summary>
        /// 由老人ID获取老人既往病史
        /// </summary>
        /// <param name="elderid">老人ID，18位</param>
        /// <returns>老人历史疾病列表，json数据</returns>
        [HttpGet]
        [Route("id")]
        [TokenCheckFilter]
        public IHttpActionResult GetbyElder(string elderid)
        {
            string str = string.Format(
                "select  to_char(starttime,'YYYY-MM') as starttime,disease.name, disease.diseaseid " +
                "from historydisease,disease " +
                "where elderid = '{0}' and " +
                "historydisease.diseaseid = disease.diseaseid"
                , elderid);

            string message = "";
            DataTable dt = OracleHelper.SelectSql(str, ref message);

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
            return Json(parentRow);
        }

        /// <summary>
        /// 添加一条历史疾病记录
        /// </summary>
        /// <param name="elderid">老人id</param>
        /// <param name="diseasename">疾病名（需要在数据库的疾病表中存在）</param>
        /// <param name="starttime">疾病开始时间，示例："2020-7"</param>
        /// <returns>添加成功或失败</returns>
        [HttpPost]
        [Route("")]
        [TokenCheckFilter]
        public IHttpActionResult Post(string diseasename, string elderid, string starttime)
        {
            HistoryDisease historyDisease = new HistoryDisease();
            historyDisease.elderid = elderid;
            historyDisease.starttime = starttime;
            string message = "";
            string str = string.Format(
                "select  diseaseID " +
                "from disease " +
                "where name = '{0}' "
                , diseasename);
            DataTable dt = OracleHelper.SelectSql(str, ref message);
            if (dt.Rows.Count == 0)  //没有找到该疾病名
            {
                return Content(HttpStatusCode.NotFound, "没有找到该疾病名");
            }
            else
            {
                historyDisease.diseaseid = dt.Rows[0]["DISEASEID"].ToString(); ;
            }

            string sql = string .Format(
                "insert into historydisease(elderid, diseaseid, starttime)"
                 + "  values ('{0}','{1}',to_date('{2}','YYYY-MM'))"
                 , historyDisease.elderid, historyDisease.diseaseid, 
                historyDisease.starttime);

            if (OracleHelper.AddSql(sql, ref message))
            {
                return Ok("成功插入");
            }
            else
            {
                return BadRequest(message);
            }
        }

        /// <summary>
        /// 由疾病ID和老人ID删除历史疾病记录
        /// </summary>
        /// <param name="diseaseID">疾病ID,18位</param>
        /// <param name="elderID">老人ID,18位</param>
        /// <returns>删除成功或失败</returns>
        [HttpDelete]
        [Route("")]
        [TokenCheckFilter]
        public IHttpActionResult Delete(string diseaseID, string elderID)
        {
            string sql = string.Format(
                "DELETE FROM historydisease" +
                " WHERE elderID='{0}' and " +
                "diseaseID='{1}'", elderID, diseaseID);
            string message = "";
            if (OracleHelper.DeleteSql(sql, ref message))
            {
                return Ok("成功删除");
            }
            else
            {
                return BadRequest("删除失败，报错如下：\n" + message);
            }
        }


    }
}
