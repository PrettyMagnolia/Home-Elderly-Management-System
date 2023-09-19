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
using System.Web;

namespace Jujialaorenfuwuxitong.Controllers
{
    /// <summary>
    /// 医生表的数据
    /// </summary>
    [RoutePrefix("api/Doctor")]
    public class DoctorController : ApiController
    {
        /// <summary>
        /// 获取医生表所有信息
        /// </summary>
        /// <returns>医生信息的所有信息，json数据</returns>
        [HttpGet]
        [Route("")]
        public IHttpActionResult Get()
        {
            string str = string.Format(
                "select userid,password,name,idcardno,gender," +
                "age,phone,field,history, communityid,photo," +
                "to_char(bantime,'yyyy-mm-dd') as bantime  " +
                "from doctor");
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
        /// 检验手机号是否有注册医生
        /// </summary>
        /// <param name="phonenumber"></param>
        /// <returns>true或者false</returns>
        [HttpGet]
        [Route("phone")]
        public IHttpActionResult GetByPhone(string phonenumber)
        {
            string str = string.Format("select * from doctor where phone='{0}'", phonenumber);
            string message = "";
            DataTable dt = OracleHelper.SelectSql(str, ref message);
            if (dt.Rows.Count == 0)
            {
                return Ok("unexist");
            }
            else
            {
                return Ok("exist");
            }
        }


        /// <summary>
        /// 通过社区ID或医生ID筛选医生信息
        /// </summary>
        /// <param name="communityid">社区id（可为空）</param>
        /// <param name="doctorid">医生id（可为空）</param>
        /// <returns>医生信息，json数据</returns>
        [HttpGet]
        [Route("id")]
        [TokenCheckFilter]
        public IHttpActionResult GetByID(string communityid=null,string doctorid=null)
        {
            string str = string.Format("" +
                "select userid,password,name,idcardno,gender," +
                "age,phone,field,history, communityid,photo," +
                "to_char(bantime,'yyyy-mm-dd') as bantime  " +
                "from doctor ");
            if (communityid != null || doctorid != null) {
                str+="where 1=1 ";
                if (communityid != null)
                {
                    str+=string.Format("and communityid='{0}'",communityid);
                }
                if (doctorid != null)
                {
                    str += string.Format("and userid='{0}'", doctorid);
                }
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
        /// 添加一个医生
        /// </summary>
        /// <param name="obj">医生信息，包含除了id外的所有医生信息,
        /// 示例：{"name":"老叶", "pwd":"12345","idcardno":"(18位)",
        /// "age":"40","phone":"133","field":"神经外科","history":"市第一医院工作十年","gender":"男",
        /// "communityid":"(请查看community表数据，communityid需要在community表中存在)",}
        /// </param>
        /// <returns>添加成功或失败</returns>
        [HttpPost]
        [Route("")]
        public IHttpActionResult Post(dynamic obj)
        {
            string[] str = { String.Format("select * from SENIOR WHERE PHONE='{0}'", obj.phone),
                String.Format("select * from NURSINGWORKER WHERE PHONE='{0}'", obj.phone),
                String.Format("select * from DOCTOR WHERE PHONE='{0}'", obj.phone),
                String.Format("select * from MANAGER WHERE PHONE='{0}'", obj.phone)};
            string message = "";
            for (int i = 0; i < str.Length; i++)
            {
                string json = OracleHelper.SelectSql_v2(str[i], ref message);
                if (json != "[]")
                {
                    message = "账号已被注册";
                    return BadRequest("账号已被注册");
                }
            }
            Doctor doctor = new Doctor(obj);
            string id = "10" + System.DateTime.Now.ToString("yyyyMMdd") + "00000001";
            string sql = string.Format(
                "INSERT /*+ IGNORE_ROW_ON_DUPKEY_INDEX(DOCTOR(USERID)) */ " +
                "INTO DOCTOR(USERID,PASSWORD,NAME,IDCARDNO,AGE,PHONE,FIELD,HISTORY,COMMUNITYID,gender) " +
                "VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}') ",
                id, doctor.pwd, doctor.name, doctor.idCardNo, doctor.age, doctor.phone, doctor.field, doctor.history, doctor.communityId,doctor.gender);
            //string message = "";

            if (OracleHelper.AddSql(sql, ref message,id))
            {
                return Ok("成功插入");
            }
            else
            {
                return BadRequest(message);
            }
        }

        /// <summary>
        /// 由医生id修改该医生的信息
        /// </summary>
        /// <param name="id">医生id(params传参)</param>
        /// <param name="obj">医生信息，包含除了id外的所有医生信息，若一项为空则会删除该项原有信息,
        /// 示例：{"name":"老叶", "pwd":"12345","idcardno":"(18位)",
        /// "age":"40","phone":"133","field":"神经外科","history":"市第一医院工作十年",
        /// "communityid":"(请查看community表数据，communityid需要在community表中存在)","gender":"男","bantime":"",}
        ///  </param>
        /// <returns>修改成功或失败</returns>
        [HttpPut]
        [Route("")]
        [TokenCheckFilter]
        public IHttpActionResult Put(string id, dynamic obj)
        {
            Doctor doctor = new Doctor(obj);
            string sql = string.Format(
                "UPDATE DOCTOR " +
                "SET " +
                "NAME='{0}'," +
                "PASSWORD='{1}'," +
                "IDCARDNO='{2}'," +
                "AGE='{3}'," +
                "PHONE='{4}'," +
                "FIELD='{5}'," +
                "HISTORY='{6}'," +
                "COMMUNITYID='{7}'," +
                "GENDER='{8}'," +
                "bantime=to_date('{9}','yyyy-mm-dd') " +
                "WHERE " +
                "USERID='{10}'",
                doctor.name, doctor.pwd, doctor.idCardNo, doctor.age, 
                doctor.phone, doctor.field, doctor.history, doctor.communityId, 
                doctor.gender,doctor.bantime,id);
            string message = "";

            if (OracleHelper.UpdateSql(sql, ref message))
            {
                return Ok("成功更新");
            }
            else
            {
                return BadRequest(message);
            }
        }

        /*
        /// <summary>
        /// 由医生id和封禁天数修改该医生的封禁状态信息
        /// </summary>
        /// <param name="id">医生id</param>
        /// <param name="days">封禁天数（单位为天，最多四位数）
        ///  </param>
        /// <returns>修改成功或失败</returns>
        [HttpPut]
        [Route("disable")]
        public IHttpActionResult PutDisable(string id, int days)
        {
            string sql = string.Format(
                "UPDATE DOCTOR " +
                "SET " +
                "disabletime={0}," +
                "userstatus='被封禁'," +
                "badrecord='有' " +
                "WHERE " +
                "USERID='{1}'",
                days, id); ;
            string message = "";

            if (OracleHelper.UpdateSql(sql, ref message))
            {
                return Ok("成功更新");
            }
            else
            {
                return BadRequest(message);
            }
        }
        */
        /// <summary>
        /// 由医生id删除该医生的信息
        /// </summary>
        /// <param name="id">医生id</param>
        /// <returns>删除成功或失败</returns>
        [HttpDelete]
        [Route("")]
        [TokenCheckFilter]
        public IHttpActionResult Delete(string id)
        {
            string sql = string.Format("DELETE FROM Doctor WHERE USERID=" + id);
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

        /// <summary>
        /// 给医生上传头像
        /// </summary>
        /// <param name="id">医生id</param>
        /// <returns>删除成功或失败</returns>
        [Route("{id}/photo")]
        [HttpPost]
        public HttpResponseMessage PostPhoto(string id)
        {
            string message = "出现错误！联系管理员";

            var files = HttpContext.Current.Request.Files;//首先先确定请求里夹带的文件数量

            if (files.AllKeys.Any())//如果存在文件
            {
                string name = files[0].FileName;
                string suffix = name.Substring(name.IndexOf(".") + 1);
                using (HttpClient client = new HttpClient())
                {
                    HttpContextBase HttpContext = (HttpContextBase)Request.Properties["MS_HttpContext"];

                    var text = HttpContext.Request.Files[0].InputStream;//获取到文件流

                    string strPath = @"\photo\doctor\" + name;
                    StreamHelp.StreamToFile(text, strPath);//需要用到下一步的帮助类将其保存为文件

                    string sql = string.Format("UPDATE DOCTOR SET PHOTO='{0}' WHERE USERID='{1}'", strPath.Replace("." + suffix, @"\" + suffix), id);
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
