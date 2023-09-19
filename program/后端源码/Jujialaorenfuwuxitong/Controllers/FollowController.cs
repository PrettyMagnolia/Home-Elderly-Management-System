using System.Collections.Generic;
using System.Data;
using System.Net.Http;
using System.Web.Http;
using Jujialaorenfuwuxitong.Models;
using System.Text; //允许使用Encoding
using System.Web.Script.Serialization;

namespace Jujialaorenfuwuxitong.Controllers
{/// <summary>
/// 关注表的数据
/// </summary>
    public class FollowController : BaseApiController
    {
        /// <summary>
        /// 获取某位用户关注的所有用户ID
        /// </summary>
        /// <param name="followerID">关注者的用户ID</param>
        /// <returns>关注列表，错误时返回错误信息</returns>
        [HttpGet]
        [Route("api/Follow/followerID")]
        [TokenCheckFilter]
        public IHttpActionResult GetFollowing(string followerID)
        {
            string str = string.Format("select userid, name from follow, doctor " +
                "where followerid='{0}' and followingid=userid", followerID);
            string message = "";
            DataTable data_doctor = OracleHelper.SelectSql(str, ref message);
            if (data_doctor !=null && data_doctor.Rows.Count > 0)
            {
                data_doctor.Columns.Add(new DataColumn("role", typeof(string)));
                for (int i = 0; i < data_doctor.Rows.Count; i++)
                    data_doctor.Rows[i]["role"] = "医生";
            }
            
            str = string.Format("select userid, name from follow, nursingworker " +
                "where followerid='{0}' and followingid=userid", followerID);
            message = "";
            DataTable data_nurse = OracleHelper.SelectSql(str, ref message);
            if (data_nurse != null && data_nurse.Rows.Count > 0)
            {
                data_nurse.Columns.Add(new DataColumn("role", typeof(string)));
                for (int i = 0; i < data_nurse.Rows.Count; i++)
                    data_nurse.Rows[i]["role"] = "护工";
                data_doctor.Merge(data_nurse); //合并表
            }

            str = string.Format("select userid, name from follow, senior " +
                "where followerid='{0}' and followingid=userid", followerID);
            message = "";
            DataTable data_senior = OracleHelper.SelectSql(str, ref message);
            if (data_senior != null && data_senior.Rows.Count > 0)
            {
                data_senior.Columns.Add(new DataColumn("role", typeof(string)));
                for (int i = 0; i < data_senior.Rows.Count; i++)
                    data_senior.Rows[i]["role"] = "老人";
                data_doctor.Merge(data_senior); //合并表
            }

            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            List<Dictionary<string, object>> parentRow = new List<Dictionary<string, object>>();
            Dictionary<string, object> childRow;
            if (data_doctor != null && data_doctor.Rows.Count > 0)
            {
                foreach (DataRow row in data_doctor.Rows)
                {
                    childRow = new Dictionary<string, object>();
                    foreach (DataColumn col in data_doctor.Columns)
                    {
                        childRow.Add(col.ColumnName, row[col]);
                    }
                    parentRow.Add(childRow);
                }
            }
            string json = jsSerializer.Serialize(parentRow);
            json = json.Replace("\\\"", "");
            if (json == null)
            {
                return Json("{\"error\":\"" + message + "\"}");
            }
            return Json(json);
        }

        /// <summary>
        /// 获取关注某位用户的所有用户ID
        /// </summary>
        /// <param name="followingID">被关注者的用户ID</param>
        /// <returns>粉丝列表，错误时返回错误信息</returns>
        [HttpGet]
        [Route("api/Follow/followingID")]
        [TokenCheckFilter]
        public IHttpActionResult GetFollower(string followingID)
        {
            string str = string.Format("select userid, name from follow, doctor " +
                "where followingid='{0}' and followerid=userid", followingID);
            string message = "";
            DataTable data_doctor = OracleHelper.SelectSql(str, ref message);
            if (data_doctor != null && data_doctor.Rows.Count > 0)
            {
                data_doctor.Columns.Add(new DataColumn("role", typeof(string)));
                for (int i = 0; i < data_doctor.Rows.Count; i++)
                    data_doctor.Rows[i]["role"] = "医生";
            }

            str = string.Format("select userid, name from follow, nursingworker " +
                "where followingid='{0}' and followerid=userid", followingID);
            message = "";
            DataTable data_nurse = OracleHelper.SelectSql(str, ref message);
            if (data_nurse != null && data_nurse.Rows.Count > 0)
            {
                data_nurse.Columns.Add(new DataColumn("role", typeof(string)));
                for (int i = 0; i < data_nurse.Rows.Count; i++)
                    data_nurse.Rows[i]["role"] = "护工";
                data_doctor.Merge(data_nurse);
            }

            str = string.Format("select userid, name from follow, senior " +
                "where followingid='{0}' and followerid=userid", followingID);
            message = "";
            DataTable data_senior = OracleHelper.SelectSql(str, ref message);
            if (data_senior != null && data_senior.Rows.Count > 0)
            {
                data_senior.Columns.Add(new DataColumn("role", typeof(string)));
                for (int i = 0; i < data_senior.Rows.Count; i++)
                    data_senior.Rows[i]["role"] = "老人";
                data_doctor.Merge(data_senior);
            }

            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            List<Dictionary<string, object>> parentRow = new List<Dictionary<string, object>>();
            Dictionary<string, object> childRow;
            foreach (DataRow row in data_doctor.Rows)
            {
                childRow = new Dictionary<string, object>();
                foreach (DataColumn col in data_doctor.Columns)
                {
                    childRow.Add(col.ColumnName, row[col]);
                }
                parentRow.Add(childRow);
            }
            string json = jsSerializer.Serialize(parentRow);
            json = json.Replace("\\\"", "");
            if (json == null)
            {
                return Json("{\"error\":\"" + message + "\"}");
            }
            return Json(json);
        }

        //POST: api/Follow
        /// <summary>
        /// 增加新的 被关注者-关注者
        /// </summary>
        /// <param name="obj">被关注者ID，关注者ID</param>
        /// {"FOLLOWINGID":"", "FOLLOWERID":""}
        /// <returns>增加成功或失败，失败时附带错误信息</returns>
        [TokenCheckFilter]
        public HttpResponseMessage Post(dynamic obj)
        {
            Follow follow = new Follow(obj);
            string sql = string.Format("insert into Follow(FOLLOWINGID, FOLLOWERID)" +
                "values('{0}', '{1}')", follow.FOLLOWINGID, follow.FOLLOWERID);
            string message = "";
            if (OracleHelper.AddSql(sql, ref message)) //仅插入一行
            {
                return new HttpResponseMessage
                {
                    Content = new StringContent("{\"status\":\"true\",\"message\":\"\"}", Encoding.UTF8)
                };
            }
            else
            {
                return new HttpResponseMessage
                {
                    Content = new StringContent("{\"status\":\"error\",\"message\":\"" + message + "\"}", Encoding.UTF8)
                };
            }
        }

        [TokenCheckFilter]
        //Delete: api/Follow?FOLLOWINGID=xx&FOLLOWERID=xx
        /// <summary>
        /// 删除制定的被关注者-关注者关系
        /// </summary>
        /// <param name="followerID">被关注者ID</param>
        /// <param name="followingID">关注者ID</param>
        /// <returns>增加成功或失败，失败时附带错误信息</returns>
        public HttpResponseMessage Delete(string followerID, string followingID)
        {
            string sql = "delete from Follow where FOLLOWINGID='" + followingID + "' and FOLLOWERID='" + followerID + "'";
            string message = "";
            if (OracleHelper.DeleteSql(sql, ref message))
            {
                return new HttpResponseMessage
                {
                    Content = new StringContent("{\"status\":\"true\",\"message\":\"\"}", Encoding.UTF8)
                };
            }
            else
            {
                return new HttpResponseMessage
                {
                    Content = new StringContent("{\"status\":\"error\",\"message\":\"" + message + "\"}", Encoding.UTF8)
                };
            }
        }
    }
}
