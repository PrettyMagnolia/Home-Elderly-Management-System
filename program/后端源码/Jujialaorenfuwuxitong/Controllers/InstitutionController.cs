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
    public class InstitutionController : ApiController
    {
        [TokenCheckFilter]
        // GET: api/Institution
        /// <summary>
        /// 获取所有机构信息
        /// </summary>
        /// <returns></returns>
        public IHttpActionResult Get()
        {
            string str = "select * from institution";
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
        // GET: api/Institution
        /// <summary>
        /// 根据机构id返回机构信息
        /// </summary>
        /// <param name="id">机构id</param>
        /// <returns></returns>
        public IHttpActionResult Get(string id)
        {
            string str = string.Format("select * from institution" +
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


        // POST: api/Institution
        /// <summary>
        /// 新增机构
        /// </summary>
        /// <param name="obj">机构信息</param>
        /// <returns></returns>
        public bool Post(dynamic obj)
        {
            Institution institution =new Institution(obj);
            string id = "33" + System.DateTime.Now.ToString("yyyyMMdd") + "00000001";
            string sql = string.Format("INSERT /*+ IGNORE_ROW_ON_DUPKEY_INDEX(INSTITUTION(ID)) */ INTO INSTITUTION(NAME,ID,ADDRESS,ESTABLISHEDTIME,PRINCIPALPHONE,ISDONE,UPLOADTIME,AUDITTIME,PHOTO)" +
                "                   VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}')",
                 institution.name,id,institution.address,institution.establishedTime,institution.principalPhone,institution.isDone,institution.uploadTime,institution.auditTime,institution.photo);
            string message = "";

            if (OracleHelper.AddSql(sql, ref message, id))
            {
                return true;
            }
            else
            {
                Console.WriteLine(message);
                return false;
            }
        }

        [TokenCheckFilter]
        // PUT: api/Institution
        /// <summary>
        /// 根据id修改机构信息
        /// </summary>
        /// <param name="id">机构id</param>
        /// <param name="obj">修改后的信息</param>
        /// <returns></returns>
        public Boolean Put(string id, dynamic obj)
        {
            Institution institution = new Institution(obj);
            string sql = string.Format(
                "UPDATE INSTITUTION " +
                "SET " +
                "NAME='{0}'," +
                "ADDRESS='{1}'," +
                "ESTABLISHEDTIME='{2}'," +
                "PRINCIPALPHONE='{3}'," +
                "ISDONE='{4}'," +
                "UPLOADTIME='{5}'," +
                "AUDITTIME='{6}'," +
                "PHOTO='{7}'" +
                "WHERE " +
                "ID='{8}'",
                institution.name, institution.address, institution.establishedTime, institution.principalPhone, institution.isDone, institution.uploadTime, institution.auditTime, institution.photo, id);
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

        [TokenCheckFilter]
        // DELETE: api/Institution
        /// <summary>
        /// 根据id删除机构信息
        /// </summary>
        /// <param name="id">机构id</param>
        /// <returns></returns>
        public Boolean Delete(string id)
        {
            string sql = string.Format("DELETE FROM INSTITUTION WHERE ID=" + id);
            string message = "";
            if (OracleHelper.DeleteSql(sql, ref message))
            {
                return true;
            }
            else
            {
                Console.WriteLine(message);
                return false;
            }
        }

        [Route("api/institution/{id}/photo")]
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

                    //string strPath = @"\photo\institution\" + name;
                    string strPath = @"\photo\institution\" + newName;

                    StreamHelp.StreamToFile(text, strPath);//需要用到下一步的帮助类将其保存为文件

                    string sql = string.Format("UPDATE INSTITUTION SET PHOTO='{0}' WHERE ID='{1}'", strPath.Replace("." + suffix, @"\" + suffix), id);
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

