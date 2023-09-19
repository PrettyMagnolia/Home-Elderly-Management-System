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
    /// 健康报告表的数据
    /// </summary>
    [RoutePrefix("api/health")]
    public class HealthReportController : ApiController
    {
        /// <summary>
        /// 获取所有的健康报告表信息
        /// </summary>
        /// <returns>健康报告记录列表，json数据</returns>
        [Route("")]
        public IHttpActionResult Get()
        {
            string sql = string.Format(
                "select * from healthreport");
            string message = "";

            DataTable dt = OracleHelper.SelectSql(sql, ref message);
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
        /// 由社区ID获取该社区中老人所有的健康报告记录的简易信息
        /// </summary>
        /// <param name="communityid">社区ID</param>
        /// <returns>健康报告记录列表，json数据</returns>
        [Route("community")]
        [TokenCheckFilter]
        public IHttpActionResult GetByCommunity(string communityid)
        {
            string sql = string.Format(
                "select r.reportid,to_char(r.time,'YYYY-MM-DD') as time," +
                "r.status,s.name as eldername,d.name as doctorname " +
                "from (healthreport r join doctor d on r.doctorid=d.userid) " +
                "right join senior s on s.userid=r.elderid " +
                "where s.communityid = '{0}' "
                , communityid);
            string message = "";

            DataTable dt = OracleHelper.SelectSql(sql, ref message);
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
        /// 由老人ID获取该老人所有的健康报告记录的简易信息
        /// </summary>
        /// <param name="elderid">老人ID</param>
        /// <returns>健康报告记录列表，json数据</returns>
        [Route("elder")]
        [TokenCheckFilter]
        public IHttpActionResult GetByElder(string elderid)
        {
            string sql = string.Format(
                "select r.reportid,r.doctorid,to_char(r.time,'YYYY-MM-DD') as time,d.name as doctorname " +
                "from healthreport r,doctor d " +
                "where r.doctorid=d.userid and " +
                "r.elderid='{0}'"
                , elderid);
            string message = "";

            DataTable dt = OracleHelper.SelectSql(sql, ref message);
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
        /// 由报告ID获取该报告中的详细健康报告信息与老人信息
        /// </summary>
        /// <param name="reportid">报告ID(18位)</param>
        /// <returns>详细服务报告信息，json数据</returns>
        [Route("reportid")]
        [TokenCheckFilter]
        public IHttpActionResult GetByReport(string reportid)
        {
            string sql = string.Format(
                "select r.rate,r.pressure,r.healthassess,r.advice,r.historyassess," +
                "s.name,s.age,s.gender,s.height,s.weight,s.situation " +
                "from senior s,healthreport r " +
                "where r.reportid = '{0}' and " +
                "s.userid=r.elderid "
                , reportid);
            string message = "";

            DataTable dt = OracleHelper.SelectSql(sql, ref message);
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
        /// 增加新的健康报告信息
        /// </summary>
        /// <param name="obj">健康报告数据，
        /// 示例：{"doctorid":"(请查看doctor表数据，doctorid需要在doctor表中存在)", 
        /// "elderid":"(请查看elder表数据，elderid需要在elder表中存在)","rate":"70",
        /// "pressure":"121","healthassess":"身体不错","advice":"多喝水","historyassess":"有好转",
        /// "status":"正在编辑",}
        /// </param>
        [HttpPost]
        [Route("")]
        [TokenCheckFilter]
        public IHttpActionResult Post(dynamic obj)
        {
            HealthReport healthReport = new HealthReport(obj);
            string time = DateTime.Now.ToString("yyyy-MM-dd");
            string reportid = "hr" + System.DateTime.Now.ToString("yyyyMMdd") + "00000001";
            string sql = string.Format(
                "INSERT /*+ IGNORE_ROW_ON_DUPKEY_INDEX(healthreport(reportid)) */ INTO " +
                "healthreport(reportid,doctorid,elderid,rate,pressure," +
                "healthassess,advice,historyassess,time,status)  " +
                "VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}',to_date('{8}','yyyy-mm-dd'),'{9}')",
                reportid,healthReport.doctorid,healthReport.elderid,healthReport.rate,healthReport.pressure,
                healthReport.healthassess,healthReport.advice,healthReport.historyassess,time,
                healthReport.status);
            string message = "";

            if (OracleHelper.AddSql(sql, ref message,reportid))
            {
                return Ok("成功添加");
            }
            else
            {
                //还需对插入失败的做id变换
                return BadRequest("添加失败，报错如下：\n" + message);
            }
        }


        /// <summary>
        /// 更新指定id的健康报告
        /// </summary>
        /// <param name="reportid">要更新的健康报告id</param>
        /// <param name="obj">健康报告数据
        /// 示例：{"rate":"79","pressure":"121","healthassess":"有问题","advice":"多喝水",
        /// "historyassess":"有好转","status":"已提交",}（不填则将原数据置为空）
        /// </param>
        [HttpPut]
        [Route("")]
        [TokenCheckFilter]
        public IHttpActionResult Put(string reportid, dynamic obj)
        {
            HealthReport healthReport = new HealthReport(obj);
            healthReport.time = DateTime.Now.ToString("yyyy-MM-dd");
            string sql = string.Format(
                "UPDATE healthreport " +
                "SET " +
                "rate='{0}'," +
                "pressure='{1}'," +
                "healthassess='{2}'," +
                "advice='{3}'," +
                "historyassess='{4}'," +
                "time=to_date('{5}','yyyy-mm-dd')," +
                "status='{6}' " +
                "WHERE " +
                "reportid='{7}'",
                healthReport.rate,healthReport.pressure,
                healthReport.healthassess,healthReport.advice,
                healthReport.historyassess,healthReport.time,
                healthReport.status,reportid);
            string message = "";

            if (OracleHelper.UpdateSql(sql, ref message))
            {
                return Ok("成功更新");
            }
            else
            {
                return BadRequest("更新失败，报错如下：\n" + message);

            }
        }

        /// <summary>
        /// 由报告ID删除报告
        /// </summary>
        /// <param name="reportID">报告ID,18位</param>
        /// <returns>删除成功或失败</returns>
        [HttpDelete]
        [Route("")]
        [TokenCheckFilter]
        public IHttpActionResult Delete(string reportID)
        {
            string sql = string.Format(
                "DELETE FROM healthreport" +
                " WHERE reportID='{0}'", reportID);
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
