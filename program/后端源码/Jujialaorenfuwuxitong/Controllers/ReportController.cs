using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;
using Jujialaorenfuwuxitong.Models;
using Oracle.DataAccess.Client;

namespace Jujialaorenfuwuxitong.Controllers
{
    public class ReportController : ApiController
    {
        [TokenCheckFilter]
        // GET: api/Report
        /// <summary>
        /// 获取所有举报单信息
        /// </summary>
        /// <returns></returns>
        public IHttpActionResult Get()
        {
            string str = "select * from report";
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
        // GET: api/Report
        /// <summary>
        /// 根据举报单id返回举报单信息
        /// </summary>
        /// <param name="id">举报单id</param>
        /// <returns></returns>
        public IHttpActionResult Get(string id)
        {
            string str = string.Format("select * from report" +
                "         WHERE ID={0}", id);
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
        // POST: api/Report
        /// <summary>
        /// 新增举报单
        /// </summary>
        /// <param name="obj">举报单信息{
        ///"ID":"10086",
        ///"imformerID":"123",
        ///"defendantID":"1001",
        ///"orderID":"1024",
        ///"type":"服务态度差",
        ///"reportTime":"20220730",
        ///"auditTime":"20220731",
        ///"isReal":"是",
        ///"punitiveMeasure":"封号3天",
        ///"reportText":"无",
        ///"isDone":"是",
        ///"imformerName":"a",
        /// "defendantName":"b"
        ///}</param>
        /// <returns></returns>
        public string Post(dynamic obj)
        {
            Report report = new Report(obj);
            string id = "44" + System.DateTime.Now.ToString("yyyyMMdd") + "00000001";
            string sql = string.Format("INSERT /*+ IGNORE_ROW_ON_DUPKEY_INDEX(REPORT(ID)) */ INTO REPORT(ID,IMFORMERID,DEFENDANTID,ORDERID,TYPE,REPORTTIME,AUDITTIME,ISREAL,PUNITIVEMEASURE,REPORTTEXT,ISDONE,IMFORMERNAME,DEFENDANTNAME,PHOTO)" +
                "                   VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}')",
                id, report.imformerID, report.defendantID, report.orderID, report.type, report.reportTime, report.auditTime, report.isReal, report.punitiveMeasure, report.reportText, report.isDone, report.imformerName, report.defendantName, report.photo);
            string message = "";

            if (OracleHelper.AddSql(sql, ref message,id))
            {
                return "true";
            }
            else
            {
                Console.WriteLine(message);
                return message;
            }
        }

        [TokenCheckFilter]
        // PUT: api/Report
        /// <summary>
        /// 根据id修改举报单信息
        /// </summary>
        /// <param name="id">举报单id</param>
        /// <param name="obj">修改后的信息</param>
        /// <returns></returns>
        public Boolean Put(string id, dynamic obj)
        {
            Report report = new Report(obj);
            string sql = string.Format(
                "UPDATE REPORT " +
                "SET " +
                "IMFORMERID='{0}'," +
                "DEFENDANTID='{1}'," +
                "ORDERID='{2}'," +
                "TYPE='{3}'," +
                "REPORTTIME='{4}'," +
                "AUDITTIME='{5}'," +
                "ISREAL='{6}'," +
                "PUNITIVEMEASURE='{7}'," +
                "REPORTTEXT='{8}'," +
                "ISDONE='{9}'," +
                "IMFORMERNAME='{10}'," +
                "DEFENDANTNAME='{11}'," +
                "PHOTO='{12}'" +
                "WHERE " +
                "ID='{13}'",
                report.imformerID, report.defendantID, report.orderID, report.type, report.reportTime, report.auditTime, report.isReal, report.punitiveMeasure, report.reportText, report.isDone, report.imformerName, report.defendantName, report.photo, id);
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

        [Route("api/report/{id}/photo")]
        [HttpPost]
        public HttpResponseMessage PostPhoto(string id)
        {
            string message = "出现错误！联系管理员";

            var files = HttpContext.Current.Request.Files;//首先先确定请求里夹带的文件数量

            if (files.AllKeys.Any())//如果存在文件
            {
                string name = files[0].FileName;
                string suffix = name.Substring(name.IndexOf(".") + 1);

                string newName = id.ToString() + "." + suffix; //将文件名改为以id命名

                using (HttpClient client = new HttpClient())
                {
                    HttpContextBase HttpContext = (HttpContextBase)Request.Properties["MS_HttpContext"];

                    var text = HttpContext.Request.Files[0].InputStream;//获取到文件流

                    //string strPath = @"\photo\report\" + name;
                    string strPath = @"\photo\report\" + newName;

                    StreamHelp.StreamToFile(text, strPath);//需要用到下一步的帮助类将其保存为文件

                    string sql = string.Format("UPDATE REPORT SET PHOTO='{0}' WHERE ID='{1}'", strPath.Replace("." + suffix, @"\" + suffix), id);
                    if (OracleHelper.UpdateSql(sql, ref message))
                    {
                        message = strPath.Replace("." + suffix, @"\" + suffix);
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
            }
            return new HttpResponseMessage
            {
                Content = new StringContent("{\"status\":\"error\",\"message\":\"" + message + "\"}", Encoding.UTF8, "application/json")
            };
        }
    }
}
