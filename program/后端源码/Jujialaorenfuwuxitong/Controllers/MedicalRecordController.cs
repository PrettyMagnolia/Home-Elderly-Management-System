using System;
using System.Collections.Generic;
using Oracle.DataAccess.Client;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Jujialaorenfuwuxitong.Models;
using System.Data;
using Newtonsoft.Json.Linq;

namespace Jujialaorenfuwuxitong.Controllers
{
    /// <summary>
    /// 医疗服务表数据
    /// </summary>
    [RoutePrefix("api/medical")]
    public class MedicalRecordController : ApiController
    {
        /// <summary>
        /// 获取服务记录表中所有信息
        /// </summary>
        /// <returns>服务记录列表，json数据</returns>
        [HttpGet]
        [Route("")]
        public IHttpActionResult Get()
        {
            string str = string.Format(
                "select * from medicalrecord");
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
        /// 由医生ID获取该医生所有的服务记录
        /// </summary>
        /// <param name="doctorID">医生ID</param>
        /// <param name="status">服务状态，0表示“未受理”（同时优先级最高）；1表示“已受理”（包括“进行中”和“已完成”）</param>
        /// <returns>医生服务记录列表，json数据</returns>
        [HttpGet]
        [Route("doctor")]
        [TokenCheckFilter]
        public IHttpActionResult GetByDoctor(string doctorID, int status)
        {
            string str = "";
            if (status > 0) // 已受理状态的服务记录
            {
                str = string.Format(
                    "select medicalrecord.serviceid,medicalrecord.elderid," +
                    "to_char(medicalrecord.STARTTIME, 'YYYY-MM-DD') as starttime," +
                    "to_char(medicalrecord.endtime, 'YYYY-MM-DD') as endtime," +
                    "medicalrecord.status,medicalrecord.timeslot,medicalrecord.address," +
                    "senior.name,senior.age,senior.situation,senior.gender,senior.height,senior.weight," +
                    "senior.phone,senior.urgent," +
                    "orders.money,orders.method,orders.orderstatus " +
                    "from medicalrecord,senior,orders " +
                    "where doctorID='{0}' and " +
                    "senior.userID=medicalrecord.elderID and " +
                    "medicalrecord.orderid = orders.orderid and " +
                    "medicalrecord.status!='未受理'"
                    , doctorID);
            }
            else  //“未受理”状态且优先级为0的服务记录
            {
                str = string.Format(
                    "select medicalrecord.serviceid,medicalrecord.elderid," +
                    "to_char(medicalrecord.STARTTIME, 'YYYY-MM-DD') as starttime," +
                    "to_char(medicalrecord.endtime, 'YYYY-MM-DD') as endtime," +
                    "medicalrecord.status,medicalrecord.timeslot,medicalrecord.address," +
                    "senior.name,senior.age,senior.situation,senior.gender,senior.height,senior.weight," +
                    "senior.phone,senior.urgent," +
                    "orders.money,orders.method,orders.orderstatus " +
                    "from medicalrecord,senior,orders " +
                    "where doctorID='{0}' and " +
                    "senior.userID=medicalrecord.elderID and " +
                    "medicalrecord.orderid = orders.orderid and " +
                    "medicalrecord.status='未受理'and " +
                    "medicalrecord.priority=0"
                    , doctorID);
            }
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
        /// 由医生ID获取该医生所有的服务评价信息
        /// </summary>
        /// <param name="doctorID">医生ID</param>
        /// <returns>医生评价信息列表，json数据</returns>
        [HttpGet]
        [Route("doctor/evaluation")]
        [TokenCheckFilter]
        //GET：由医生ID获取该医生所有的服务评价信息
        public IHttpActionResult GetByDoctor2(string doctorID)
        {
            string str = string.Format(
                "select m.serviceid,m.evaluationid,m.elderid,s.name as evaluator,e.grade,e.message,e.time " +
                "from medicalrecord m,senior s,evaluation e  " +
                "where m.doctorid='{0}' and " +
                "m.evalustatus='已评价' and " +
                "m.evaluationid=e.evaluationid and " +
                "m.elderid=s.userid "
                , doctorID);
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
        /// 由老人ID获取该老人所有的服务记录
        /// </summary>
        /// <param name="elderID">老人ID,18位</param>
        /// <returns>老人服务记录列表，json数据</returns>
        [HttpGet]
        [Route("elder")]
        [TokenCheckFilter]
        public IHttpActionResult GetByElder(string elderID)
        {
            string str = string.Format(
                "select medicalrecord.serviceid,medicalrecord.doctorid," +
                "to_char(medicalrecord.STARTTIME, 'YYYY-MM-DD') as starttime,medicalrecord.status," +
                "medicalrecord.timeslot,doctor.name,doctor.phone," +
                "orders.money,orders.orderstatus " +
                "from medicalrecord,doctor,orders " +
                "where elderID='{0}' and " +
                "doctor.userID=medicalrecord.doctorID and " +
                "medicalrecord.orderid=orders.orderid and " +
                "medicalrecord.priority=0"
                , elderID);
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
        /// 由老人ID获取该老人所有的服务评价信息
        /// </summary>
        /// <param name="elderID">老人ID,18位</param>
        /// <returns>老人评价信息列表，json数据</returns>
        [HttpGet]
        [Route("elder/evaluation")]
        [TokenCheckFilter]
        public IHttpActionResult GetByElder2(string elderID)
        {
            string str = string.Format(
                "select m.serviceid, d.name as doctorname,m.starttime,m.endtime,m.evaluationid,m.evalustatus," +
                "e.time as evaluationtime,e.grade,e.message " +
                "from " +
                "(medicalrecord m left join evaluation e on m.evaluationid = e.evaluationid) " +
                "join doctor d on d.userid = m.doctorid " +
                "where m.elderID = '{0}' and " +
                "m.status='已完成'"
                , elderID);
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
        /// 增加新的服务记录信息,并添加一条新订单
        /// </summary>
        /// <param name="obj">
        /// 服务记录信息数据，
        /// 
        /// 示例：{"doctorid":["请查看doctor表数据，doctorid需要在doctor表中存在","**"],
        /// "elderid":"(请查看elder表数据，elderid需要在elder表中存在)","address":"05号楼315室",
        /// "starttime":"2020-09-10 13:00","endtime":"2020-10-10 15:00", "timeslot":"上午",
        /// "evalustatus":"未评价","status":"未受理","orderstatus":"未支付","method":"上门现付","money":"10",}
        /// </param>
        /// <returns>增加成功或失败</returns>
        [HttpPost]
        [Route("")]
        [TokenCheckFilter]
        public IHttpActionResult Post(dynamic obj)
        {
            ////////////////////////////////////////////////////////{"doctorid":["102022080800000001","102022080800000002"],"elderid":"002022080800000001","address":"05号楼315室","starttime":"2020-09-10 13:00","endtime":"2020-10-10 15:00", "timeslot":"上午","evalustatus":"未评价","status":"未受理","orderstatus":"未支付","method":"上门现付","money":"10",}
            /////////插入一条新订单/////////////////////////////////{"doctorid":["102022080800000001","102022080800000002"],"elderid":"002022080800000001","address":"05号楼315室","starttime":"2020-09-10 13:00","endtime":"2020-10-10 15:00", "timeslot":"上午","evalustatus":"未评价","status":"未受理","orderstatus":"未支付","method":"上门现付","money":"10",}
            string orderid = obj.elderid + "0000001";
            string message = "";
            string sql = string.Format(
               "insert /*+ IGNORE_ROW_ON_DUPKEY_INDEX(ORDERS(ORDERID))*/  into " +
               "ORDERS(ORDERID,ORDERSTATUS,METHOD,MONEY) " +
               "VALUES('{0}','{1}','{2}',{3})"
                , orderid, obj.orderstatus, obj.method, obj.money);
            bool re = false;
            try
            {
                OracleConnection conn = OracleHelper.DbConn(ref message, ref re);
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
                int idnum = int.Parse(orderid.Substring(18, 7));
                int count = 0;
                while (count == 0)
                {
                    command = new OracleCommand(sql, conn);
                    count = command.ExecuteNonQuery();
                    idnum += 1;
                    command.Dispose();
                    //ToString("D8")，D表示十进制数，8表示位数
                    sql = sql.Replace(orderid, orderid.Substring(0, 18) + idnum.ToString("D7"));
                    orderid = orderid.Substring(0, 18) + idnum.ToString("D7");
                }
                orderid = orderid.Substring(0, 18) + (idnum-1).ToString("D7");

                ////////////////////////////////////////////////////////
                /////////插入一条新的服务记录///////////////////////////
                string serviceid = "sv" + System.DateTime.Now.ToString("yyyyMMdd") + "00000001";
                int i = 0;
                List<string> doctorid = obj.doctorid.ToObject<List<string>>();
                foreach (string onedoctorid in doctorid)
                {
                    if (i == 0)
                    {
                        string sql1 = string.Format(
                            "INSERT /*+ IGNORE_ROW_ON_DUPKEY_INDEX(medicalrecord(serviceid)) */ INTO " +
                            "medicalrecord(serviceid,doctorid,elderid,starttime,endtime,orderid,timeslot,evalustatus,status,priority,address)  " +
                            "VALUES('{0}','{1}','{2}',to_date('{3}','yyyy-mm-dd hh24:mi'),to_date('{4}','yyyy-mm-dd hh24:mi'),'{5}','{6}','{7}','{8}',{9},'{10}')",
                            serviceid, onedoctorid, obj.elderid, obj.starttime, obj.endtime,
                            orderid, obj.timeslot, obj.evalustatus, obj.status, i,obj.address);
                        idnum = int.Parse(serviceid.Substring(10, 8));
                        count = 0;
                        while (count == 0)
                        {
                            command = new OracleCommand(sql1, conn);
                            count = command.ExecuteNonQuery();
                            message = serviceid;
                            idnum += 1;
                            command.Dispose();
                            //ToString("D8")，D表示十进制数，8表示位数
                            sql1 = sql1.Replace(serviceid, serviceid.Substring(0, 10) + idnum.ToString("D8"));
                            serviceid = serviceid.Substring(0, 10) + idnum.ToString("D8");
                        }
                        serviceid = serviceid.Substring(0, 10) + (idnum-1).ToString("D8");
                    }
                    else
                    {
                        string sql2 = string.Format(
                            "INSERT  INTO " +
                            "medicalrecord(serviceid,doctorid,elderid,starttime,endtime,orderid,timeslot,evalustatus,status,priority,address)  " +
                            "VALUES('{0}','{1}','{2}',to_date('{3}','yyyy-mm-dd hh24:mi'),to_date('{4}','yyyy-mm-dd hh24:mi'),'{5}','{6}','{7}','{8}',{9},'{10}')",
                            serviceid, onedoctorid, obj.elderid, obj.starttime, obj.endtime,
                            orderid, obj.timeslot, obj.evalustatus, obj.status, i, obj.address);
                        if(!OracleHelper.AddSql(sql2, ref message))
                            return BadRequest("添加失败，报错如下：\n" + message);
                    }
                    i++;

                }
                return Ok("成功添加");
            }
            catch (Exception e)
            {
                return BadRequest("添加失败，报错如下：\n" + e.Message);
            }
        }

        /// <summary>
        /// 更新指定服务记录的服务状态
        /// </summary>
        /// <param name="serviceid">要更新的服务记录id，18位</param>
        /// <param name="status">更新后的状态，示例：进行中</param>
        /// <returns>更新成功或失败</returns>
        [HttpPut]
        [Route("status")]
        [TokenCheckFilter]
        public IHttpActionResult PutStatus(string serviceid, string status)
        {
            string sql = string.Format(
                "UPDATE medicalrecord " +
                "SET " +
                "status='{0}' " +
                "WHERE " +
                "serviceid='{1}'"
                ,status,serviceid);
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
        /// 改变支付状态或支付方式
        /// </summary>
        /// <param name="serviceid">要改变的服务id</param>
        /// <param name="method">改变后的支付方式（空则不改变）</param>
        /// <param name="orderstatus">改变后的支付状态（空则不改变）</param>
        /// <returns>更新成功或失败</returns>
        [HttpPut]
        [Route("order")]
        [TokenCheckFilter]
        public IHttpActionResult PutOrder(string serviceid, string method=null, string orderstatus=null)
        {
            string sql = string.Format(
                "select orderid " +
                "from medicalrecord " +
                "where serviceid='{0}' "
                , serviceid);
            string message = "";
            DataTable dt = OracleHelper.SelectSql(sql, ref message);

            string orderid = dt.Rows[0]["orderid"].ToString();
            //更新支付状态或支付方式
            if (method != null || orderstatus != null) {
                sql = "UPDATE orders SET ";
                if (method != null)
                    sql += string.Format("method='{0}'", method);
                if (orderstatus != null)
                    sql += string.Format("orderstatus='{0}'", orderstatus);
                sql+= string.Format("WHERE " +
                    "orderid='{0}'",
                    orderid);
                if (OracleHelper.UpdateSql(sql, ref message))
                {
                    return Ok("成功更新");
                }
                else
                {
                    return BadRequest("更新失败，报错如下：\n" + message);
                }
            }
            else
            {
                return BadRequest("没有要更新的参数");
            }
        }

        /// <summary>
        /// 点击同意后，更新服务状态为'进行中'，并删除相同服务id的其他服务记录
        /// </summary>
        /// <param name="serviceid">要更新的服务记录id</param>
        /// <returns>更新成功或失败</returns>
        [HttpPut]
        [Route("agree")]
        [TokenCheckFilter]
        public IHttpActionResult PutAgree(string serviceid)
        {
            //更新状态
            string sql = string.Format(
                "UPDATE medicalrecord " +
                "SET " +
                "status='进行中' " +
                "WHERE " +
                "serviceid='{0}' and " +
                "priority =0",
                serviceid);
            string message = "";

            if (!OracleHelper.UpdateSql(sql, ref message))
            {
                return BadRequest("更新失败，报错如下：\n" + message);
            }

            //删除记录
            sql = string.Format(
                "DELETE FROM medicalrecord" +
                " WHERE serviceID='{0}' and priority!=0", serviceid);
            if (OracleHelper.UpdateSql(sql, ref message))
            {
                return Ok("成功更新并删除");
            }
            else
            {
                return BadRequest("删除失败，报错如下：\n" + message);
            }
        }

        /// <summary>
        /// 评价后更新对应服务记录为已评价
        /// </summary>
        /// <param name="serviceid">要更新的服务记录id，18位</param>
        /// <param name="evaluationid">服务记录对应评价ID，25位，来自于evaluation表post后的返回值</param>
        /// <returns>更新成功或失败</returns>
        [HttpPut]
        [Route("evaluation")]
        [TokenCheckFilter]
        public IHttpActionResult PutEvaluation(string serviceid, string evaluationid)
        {
            string sql = string.Format(
                "UPDATE medicalrecord " +
                "SET " +
                "evalustatus='{2}',evaluationid='{0}'" +
                "WHERE " +
                "serviceid='{1}'",
                evaluationid,serviceid,"已评价");
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
        /// 拒绝时，由服务ID删除一条优先级最高的服务记录，并把其他服务记录提高一个优先级
        /// </summary>
        /// <param name="serviceID">服务ID</param>
        /// <returns>删除成功或失败</returns>
        [HttpDelete]
        [Route("")]
        [TokenCheckFilter]
        public IHttpActionResult Delete(string serviceID)
        {
            //删除记录
            string sql = string.Format(
                "select serviceid from medicalrecord where serviceid='{0}'"
                , serviceID);
            string message = "";
            DataTable dt = OracleHelper.SelectSql(sql, ref message);
            if (dt.Rows.Count == 1)
            {
                sql = string.Format("update medicalrecord set status='已拒绝' where serviceid='{0}'", serviceID);
                OracleHelper.UpdateSql(sql, ref message);
                return Ok("只剩最后一条记录，更新为已拒绝。");
            }
            else
            {
                sql = string.Format(
                    "DELETE FROM medicalrecord" +
                    " WHERE serviceID='{0}' and priority=0", serviceID);
                message = "";
                if (!OracleHelper.DeleteSql(sql, ref message))
                {
                    return BadRequest("删除失败，报错如下：\n" + message);
                }
            }
            //提高优先级
            sql = string.Format("UPDATE medicalrecord SET priority=priority-1 WHERE serviceid='{0}'", serviceID);

            if (OracleHelper.UpdateSql(sql, ref message))
            {
                return Ok("成功删除并更新");
            }
            else
            {
                return BadRequest("成功删除，但更新失败,没有要更新的数据");
            }
        }


        /// <summary>
        /// 撤回申请，删除所有服务记录
        /// </summary>
        /// <param name="serviceID">服务ID</param>
        /// <returns>删除成功或失败</returns>
        [HttpDelete]
        [Route("all")]
        [TokenCheckFilter]
        public IHttpActionResult DeleteAll(string serviceID)
        {
            string sql = string.Format(
                "DELETE FROM medicalrecord" +
                " WHERE serviceID='{0}'", serviceID);
            string message = "";
            if (!OracleHelper.DeleteSql(sql, ref message))
            {
                return BadRequest("删除失败，报错如下：\n" + message);
            }
            else
                return Ok("成功删除！");
        }
    }
}
