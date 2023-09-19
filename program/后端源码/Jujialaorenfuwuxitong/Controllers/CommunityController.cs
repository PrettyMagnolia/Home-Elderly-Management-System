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
    /// 社区表的数据
    /// </summary>
    [RoutePrefix("api/community")]
    public class CommunityController : ApiController
    {
        /// <summary>
        /// 获取社区全部信息
        /// </summary>
        /// <returns>社区信息，包括社区ID、社区名、社区地址，json数据</returns>
        [HttpGet]
        [Route("")]
        public IHttpActionResult Get()
        {
            string str = string.Format("select * from community");

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
        /// 由社区名获取社区信息
        /// </summary>
        /// <param name="name">社区名</param>
        /// <returns>社区信息，包括社区ID、社区名、社区地址，json数据</returns>
        [HttpGet]
        [Route("name")]
        [TokenCheckFilter]
        public IHttpActionResult GetbyName(string name)
        {
            string str = string.Format("select * from community " +
               "WHERE NAME= '{0}'",name);

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
        /// 由社区ID获取社区信息
        /// </summary>
        /// <param name="id">社区ID</param>           
        [HttpGet]
        [Route("id")]
        [TokenCheckFilter]
        public IHttpActionResult GetbyID(string id)
        {
            string str = string.Format("select * from community" +
               "         WHERE communityid= '{0}'", id);

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
        /// 添加一个社区
        /// </summary>
        /// <param name="obj">社区信息
        /// 示例：
        /// {
        ///  "name":"",
        ///  "city":"上海",
        ///  "block":"嘉定区",
        /// }</param>
        /// <returns>添加成功或失败</returns>
        [HttpPost]
        [Route("")]
        [TokenCheckFilter]
        public IHttpActionResult Post(dynamic obj)
        {
            Community community = new Community(obj);
            string id = "cm" + System.DateTime.Now.ToString("yyyyMMdd") + "00000001";
            string sql = string.Format(
                "INSERT /*+ IGNORE_ROW_ON_DUPKEY_INDEX(COMMUNITY(COMMUNITYID)) */ " +
                "INTO community(NAME,communityid,city,block)" +
                "                   VALUES('{0}','{1}','{2}','{3}')",
                 community.name, id, community.city,community.block);
            string message = "";

            if (OracleHelper.AddSql(sql, ref message, id))
            {
                return Ok("成功添加");
            }
            else
            {
                return BadRequest("添加失败，报错如下：\n" + message);
            }
        }

        /// <summary>
        /// 由社区ID删除社区
        /// </summary>
        /// <param name="ID">社区ID</param>
        /// <returns>删除成功或失败</returns>
        [HttpDelete]
        [Route("")]
        [TokenCheckFilter]
        public IHttpActionResult DeleteByID(string ID)
        {
            string sql = string.Format("DELETE FROM community WHERE communityid='" + ID +"'");

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
