using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Configuration;
using System.Data.OracleClient;
using Jujialaorenfuwuxitong.Models;
using System.Data;
using System.Web.Script.Serialization;
using System.Text;

namespace Jujialaorenfuwuxitong.Controllers
{
    public class AuditLogController : ApiController
    {
        [TokenCheckFilter]
        [Route("api/auditlog/{isDone}/report")]
        [HttpGet]
        /// <summary>
        /// GET: api/auditlog/{isDone}/report
        /// 获取待审核举报单数
        /// </summary>
        /// <returns></returns>
        public IHttpActionResult GetReport(string isDone)
        {
            string str = string.Format(
                "select count(*) " +
                "from report " +
                "where " +
                "ISDONE='{0}'",
                isDone);
            string message = "";
            DataTable dt = OracleHelper.SelectSql(str, ref message);
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
            return Json(json);

        }

        [TokenCheckFilter]
        [Route("api/auditlog/{isDone}/institution")]
        [HttpGet]
        /// <summary>
        /// GET: api/auditlog/{isDone}/institution
        /// 获取待审核机构数
        /// </summary>
        /// <returns></returns>
        public IHttpActionResult GetInstitution(string isDone)
        {
            string str = string.Format(
                "select count(*) " +
                "from institution " +
                "where " +
                "ISDONE='{0}'",
                isDone);
            string message = "";
            DataTable dt = OracleHelper.SelectSql(str, ref message);
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
            return Json(json);

        }

        [TokenCheckFilter]
        [Route("api/auditlog/{isdone}/qualification")]
        [HttpGet]
        /// <summary>
        /// GET: api/auditlog/{isdone}/qualification
        /// 获取待审核数目信息
        /// </summary>
        /// <returns></returns>
        public IHttpActionResult GetQualification(string isDone)
        {
            string str = string.Format(
                "select count(*) " +
                "from qualification " +
                "where " +
                "ISDONE='{0}'",
                isDone);
            string message = "";
            DataTable dt = OracleHelper.SelectSql(str, ref message);
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
            return Json(json);

        }


        [TokenCheckFilter]
        // PUT: api/AuditLog
        /// <summary>
        /// 修改待审核数目（护工、机构、举报单）
        /// </summary>
        /// <param name="id">默认ID=>"log"</param>
        /// <param name="obj">修改后的信息</param>
        /// <returns></returns>
        public Boolean Put(string id,dynamic obj)
        {
            AuditLog auditLog = new AuditLog(obj);
            string sql = string.Format(
                "UPDATE AUDITLOG " +
                "SET " +
                "NURSETOAUDIT='{0}'," +
                "REPORTTOAUDIT='{1}'," +
                "INSTITUITONTOAUDIT='{2}'" +
                "WHERE " +
                "ID='{3}'",
                auditLog.nurseToAudit, auditLog.reportToAudit, auditLog.instituitonToAudit, id);
            string message = "";

            if (OracleHelper.UpdateSql(sql, ref message))
            {
                return true;
            }
            else
            {
                Console.WriteLine(message);
                return false;
            }
        }
    }
}
