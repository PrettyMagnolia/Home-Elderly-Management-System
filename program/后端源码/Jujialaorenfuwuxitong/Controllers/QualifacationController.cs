using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Jujialaorenfuwuxitong.Models;
using System.Data;
using System.Text;
using System.Web;

namespace Jujialaorenfuwuxitong.Controllers
{
    /// <summary>
    /// 资质表的数据
    /// </summary>
    public class QualifacationController : BaseApiController
    {
        [TokenCheckFilter]
        //Get: api/Qualification
        /// <summary>
        /// 获得资质表中的全部信息
        /// </summary>
        /// <returns></returns>
        public IHttpActionResult Get()
        {
            string str = "select * from Qualification";
            string message = "";
            string json = OracleHelper.SelectSql_v2(str, ref message);
            if (json == null)
            {
                return Json("{\"error\":\"" + message + "\"}");
            }
            return Json(json);
        }

        [TokenCheckFilter]
        /// <summary>
        /// 获取指定ID的资质信息
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns></returns>
        public IHttpActionResult GetByID(string id)
        {
            string str = string.Format("select * from Qualification" +
                "         where WORKERID='{0}'", id);
            string message = "";
            string json = OracleHelper.SelectSql_v2(str, ref message);
            if (json == null)
            {
                return Json("{\"error\":\"" + message + "\"}");
            }
            return Json(json);
        }

        [TokenCheckFilter]
        /// <summary>
        /// Post: api/Qualifacation
        /// 为指定ID添加新的资质信息，资质上传时间会自动生成
        /// 会检查用户ID是否存在于医生\护工表中
        /// </summary>
        /// <param name="obj">
        /// 必填：workerid，role，title
        /// 可选项：proof，granttime，adminitorid，isdone，audittime，institutionid，institutionname
        /// 示例： {"workerid":"102020091600000001",  "role":"护工"或"医生", "title":"初级护理", 
        ///"proof":"在xx地点参加了初级护理考试", "granttime":"2020-08-14", 
        ///"adminitorid":"", "isdone":"no",
        ///"audittime":, "institutionid":"", "institutionname":""}
        /// </param>
        /// <returns>增加成功或失败，失败时附带错误信息</returns>
        public IHttpActionResult Post(dynamic obj)
        {
            string message = "";
            Qualification qualification = new Qualification(obj);
            if (qualification.workerid == null)
            {
                return BadRequest("未输入用户ID");
            }
            if (qualification.role == "医生")
            {
                string str = string.Format("select * from Doctor where USERID='{0}'", qualification.workerid);
                DataTable dt = OracleHelper.SelectSql(str, ref message);
                if (dt.Rows.Count <= 0)
                {
                    return BadRequest("医生ID不存在");
                }
            }
            else if (qualification.role == "护工")
            {
                string str = string.Format("select * from NursingWorker where USERID='{0}'", qualification.workerid);
                DataTable dt = OracleHelper.SelectSql(str, ref message);
                if (dt.Rows.Count <= 0)
                {
                    return BadRequest("护工ID不存在");
                }
            }
            else
            {
                return BadRequest("输入身份错误");
            }
            string uploadtime = System.DateTime.Now.ToString("yyyy-MM-dd");
            string sql = string.Format("insert into Qualification(WORKERID, ROLE, TITLE, PROOF, GRANTTIME, UPLOADTIME, ADMINITORID, ISDONE, AUDITTIME, INSTITUTIONID, INSTITUTIONNAME)" +
                " values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}')",
                qualification.workerid, qualification.role, qualification.title, qualification.proof,
                qualification.granttime, uploadtime,
                qualification.adminitorid, qualification.isdone, qualification.audittime,
                qualification.institutionid, qualification.institutionname);
            if (OracleHelper.AddSql(sql, ref message))
            {
                return Ok("成功插入");
            }
            else
            {
                return BadRequest(message);
            }
        }

        [TokenCheckFilter]
        /// <summary>
        /// Put: api/Qualifacation
        /// 更新指定ID的某条资质信息
        /// </summary>
        /// <param name="obj">
        /// 必填：workerid，title
        /// 可选项：proof，granttime，adminitorid，isdone，audittime，institutionid，institutionname
        /// 示例： {"workerid":"102020091600000001", "title":"初级护理", 
        ///"proof":"在xx地点参加了初级护理考试", "granttime":"2020-08-14", 
        ///"adminitorid":"123456", "isdone":"yes",
        ///"audittime":"2022-08-20", "institutionid":"", "institutionname":""}
        /// </param>
        /// <returns>成功或失败，失败时附带错误信息</returns>
        public IHttpActionResult Put(dynamic obj)
        {
            Qualification qualification = new Qualification(obj);
            string sql = string.Format(
                "update Qualification " +
                "SET " +
                "PROOF='{0}'," +
                "GRANTTIME='{1}'," +
                "ADMINITORID='{2}'," +
                "ISDONE='{3}'," +
                "AUDITTIME='{4}'," +
                "INSTITUTIONID='{5}'," +
                "INSTITUTIONNAME='{6}'" +
                "where " +
                "WORKERID='{7}' and TITLE='{8}'",
                qualification.proof, qualification.granttime, qualification.adminitorid, qualification.isdone,
                qualification.audittime, qualification.institutionid, qualification.institutionname, qualification.workerid, qualification.title);
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

        [TokenCheckFilter]
        /// <summary>
        /// Delete: api/Qualifacation?id=xx&title=xx
        /// eg: api/Qualifacation?id='012021073100000001'&title='senior'
        /// 根据ID和资质名，删除某条资质信息
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="title">资质名</param>
        /// <returns>删除成功或失败</returns>
        public IHttpActionResult Delete(string id, string title)
        {
            string sql = string.Format("delete from Qualification where WORKERID=" + id + " and TITLE=" + title);
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

        [TokenCheckFilter]
        /// <summary>
        /// 为某用户的某条资质添加照片
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <param name="title">资质名</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Qualifacation/{id}/{title}/photo")]
        public HttpResponseMessage PostPhoto(string id, string title)
        {
            string message = "出现错误！联系管理员";

            var files = HttpContext.Current.Request.Files; //首先先确定请求里夹带的文件数量

            if (files.AllKeys.Any()) //如果存在文件
            {
                string name = files[0].FileName;
                string suffix = name.Substring(name.IndexOf(".") + 1);

                string newName = id + "_" + title + "." + suffix; //将文件名改为：id_title.文件格式

                using (HttpClient client = new HttpClient())
                {
                    HttpContextBase HttpContext = (HttpContextBase)Request.Properties["MS_HttpContext"];

                    var text = HttpContext.Request.Files[0].InputStream; //获取到文件流

                    string strPath = @"\photo\qualification\" + newName;

                    StreamHelp.StreamToFile(text, strPath);//需要用到下一步的帮助类将其保存为文件

                    string sql = string.Format("UPDATE QUALIFICATION SET PHOTO='{0}' WHERE WORKERID='{1}' and TITLE='{2}'"
                        , strPath.Replace("." + suffix, @"\" + suffix), id, title);
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
