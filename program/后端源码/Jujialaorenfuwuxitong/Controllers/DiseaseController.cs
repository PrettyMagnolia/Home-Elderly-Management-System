using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Jujialaorenfuwuxitong.Models;
using System.Data;
using System.Text;

namespace Jujialaorenfuwuxitong.Controllers
{
    /// <summary>
    /// 疾病表的数据
    /// </summary>
    public class DiseaseController : BaseApiController
    {
        [TokenCheckFilter]
        /// <summary>
        /// Get: api/Disease/{id}
        /// 获取疾病学名
        /// </summary>
        /// <param name="id">疾病ID</param>
        /// <returns></returns>
        public HttpResponseMessage Get(string id)
        {
            string str = string.Format("select NAME from Disease where DISEASEID='{0}'", id);
            string message = "";
            DataTable dt = OracleHelper.SelectSql(str, ref message);
            //能找到唯一的疾病学名
            string json;
            if (dt.Rows.Count > 0)
            {
                 json = dt.Rows[0]["NAME"].ToString();
                return new HttpResponseMessage()
                {
                    Content = new StringContent(json, Encoding.UTF8),
                };
            }
            else
            {
                return new HttpResponseMessage()
                {
                    Content = new StringContent("{\"error\":\"" + message + "\"}", Encoding.UTF8),
                };
            }
            
        }

        [TokenCheckFilter]
        /// <summary>
        /// Post: api/Disease
        /// 添加新的疾病学名，并生成疾病ID
        /// </summary>
        /// <param name="obj">疾病学名{"name":""}</param>
        /// <returns></returns>
        public IHttpActionResult Post(dynamic obj)
        {
            string name = Convert.ToString(obj.name);
            string str = string.Format("select DISEASEID from Disease where NAME='{0}'", name);
            string message = "";
            DataTable dt = OracleHelper.SelectSql(str, ref message);
            if (dt.Rows.Count > 0)
            {
                //如果表不为空，则数据库中已经记录过这种疾病
                message = "数据库中已记录该疾病，不必新增";
                return BadRequest(message);
            }
            else
            {
                string num = System.DateTime.Now.ToString("yyyyMMddHHmmss") + "0000";
                str = string.Format("insert into Disease(DISEASEID, NAME)"
                    + "values('{0}', '{1}')", num, name);
                if (OracleHelper.AddSql(str, ref message))
                {
                    return Ok("成功插入");
                }
                else
                {
                    return BadRequest(message);
                }
            }
        }

        [TokenCheckFilter]
        /// <summary>
        /// Delete: api/Disease/{id}
        /// 删除指定ID的疾病条目
        /// </summary>
        /// <param name="id"></param>
        /// <returns>删除成功或失败</returns>
        public IHttpActionResult Delete(string id)
        {
            string sql = string.Format(
                "delete from disease" +
                " WHERE DISEASEID='{0}'", id);
            string message = "";
            if (OracleHelper.DeleteSql(sql, ref message))
            {
                return Ok("成功删除");
            }
            else
            {
                return BadRequest(message);
            }
        }
    }
}
