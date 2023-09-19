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
    /// 会议室数据
    /// </summary>
    [RoutePrefix("api/room")]
    public class RoomController : ApiController
    {
        /// <summary>
        /// 获取所有会议信息
        /// </summary>
        /// <returns>会议信息，包括会议ID、开始时间、结束时间、密码、医生id，json数据</returns>
        [Route("")]
        [HttpGet]
        public IHttpActionResult Get()
        {
            string sql = string.Format(
                "select * from room");
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
        /// 由社区ID获取社区中未结束的所有会议信息（会议结束时间迟于当前时间）
        /// </summary>
        /// <param name="communityid">社区id，18位</param>
        /// <returns>会议信息，包括会议ID、开始时间、结束时间、密码、医生id，json数据</returns>
        [Route("community")]
        [HttpGet]
        [TokenCheckFilter]
        public IHttpActionResult GetByCommunity(string communityid)
        {
            DateTime dateTime = DateTime.Now;

            string sql = string.Format(
                "select r.roomid,to_char(r.starttime,'yyyy-mm-dd hh24:mi') starttime," +
                "to_char(r.endtime,'yyyy-mm-dd hh24:mi') endtime,r.doctorid,r.password as roompassword," +
                "d.name as doctorname,d.field " +
                "from room r,doctor d " +
                "where r.communityid = '{0}' and " +
                "r.doctorid = d.userid and " +
                "r.endtime > to_date('{1}','yyyy-mm-dd hh24:mi:ss')", communityid, dateTime);
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
        /// 由医生ID获取该医生未结束的所有会议信息（会议结束时间迟于当前时间）
        /// </summary>
        /// <param name="doctorid">医生id,18位</param>
        /// <returns>会议信息，包括会议ID、开始时间、结束时间、密码、医生id，json数据</returns>
        [Route("doctor")]
        [HttpGet]
        [TokenCheckFilter]
        public IHttpActionResult GetByDoctor(string doctorid)
        {
            DateTime dateTime = DateTime.Now;

            string sql = string.Format(
                "select roomid,to_char(starttime,'yyyy-mm-dd hh24:mi') starttime," +
                "to_char(endtime,'yyyy-mm-dd hh24:mi') endtime,doctorid,password " +
                "from room " +
                "where doctorid = '{0}' and " +
                "endtime > to_date('{1}','yyyy-mm-dd hh24:mi:ss')", doctorid, dateTime);
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
        /// 由会议ID返回会议信息和医生信息
        /// </summary>
        /// <param name="roomid">会议id,18位</param>
        /// <returns>会议信息</returns>
        [Route("id")]
        [HttpGet]
        [TokenCheckFilter]
        public IHttpActionResult GetByRoom(string roomid)
        {
            string sql = string.Format(
                "select to_char(starttime,'yyyy-mm-dd hh24:mi') starttime," +
                "to_char(endtime,'yyyy-mm-dd hh24:mi') endtime,room.password," +
                "doctor.name,doctor.field " +
                "from room,doctor " +
                "where roomid = '{0}' and " +
                "room.doctorid=doctor.userid",roomid);
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
        /// 添加一个会议室
        /// </summary>
        /// <param name="obj">会议室信息
        /// 示例：
        /// {
        ///  "starttime":"2020-07-01 13:00",
        ///  "endtime":"2020-07-01 15:00",
        ///  "roomid":"123456789",
        ///  "password":"123456",
        ///  "doctorid":"(请查看doctor表数据，doctorid需要在doctor表中存在)",
        /// }
        /// </param>
        /// <returns>添加成功或失败</returns>
        [HttpPost]
        [Route("")]
        [TokenCheckFilter]
        public IHttpActionResult Post(dynamic obj)
        {
            Room room = new Room(obj);

            //由obj中的doctorid获取对应的communityid
            string sql = string.Format(
                "select communityid " +
                "from doctor " +
                "where userid='{0}'", obj.doctorid);
            string message = "";
            DataTable dt = OracleHelper.SelectSql(sql, ref message);
            if (dt == null)
            {
                return BadRequest("查找医生id失败，报错如下：\n" + message);
            }
            else
                room.communityid = dt.Rows[0]["communityid"].ToString();

            //插入会议信息
            sql = string.Format(
                "INSERT /*+ IGNORE_ROW_ON_DUPKEY_INDEX(ROOM(ROOMID)) */ " +
                "INTO room(roomid,communityid,starttime," +
                "endtime,password,doctorid)" +
                "                   VALUES('{0}','{1}',to_date('{2}','yyyy-mm-dd hh24:mi')," +
                "to_date('{3}','yyyy-mm-dd hh24:mi'),'{4}','{5}')",
                 room.roomid,room.communityid, room.starttime, room.endtime,room.password,room.doctorid);
            message = "";

            if (OracleHelper.AddSql(sql, ref message))
            {
                return Ok("成功添加");
            }
            else
            {
                return BadRequest("添加失败，报错如下：\n" + message);
            }
        }

        /// <summary>
        /// 由会议ID删除会议
        /// </summary>
        /// <param name="roomID">会议ID,18位</param>
        /// <returns>删除成功或失败</returns>
        [HttpDelete]
        [Route("")]
        [TokenCheckFilter]
        public IHttpActionResult Delete(string roomID)
        {
            string sql = string.Format(
                "DELETE FROM room" +
                " WHERE roomID='{0}'", roomID);
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
