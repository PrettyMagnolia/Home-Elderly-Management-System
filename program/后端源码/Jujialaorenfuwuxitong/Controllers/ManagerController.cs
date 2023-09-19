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
    public class ManagerController : ApiController
    {
        [TokenCheckFilter]
        // GET: api/Manager
        /// <summary>
        /// 获取所有管理员信息
        /// </summary>
        /// <returns></returns>
        public IHttpActionResult Get()
        {
            string str = "select * from manager";
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
        // GET: api/Manager
        /// <summary>
        /// 根据管理员id返回管理员信息
        /// </summary>
        /// <param name="id">管理员id</param>
        /// <returns></returns>
        public IHttpActionResult Get(string id)
        {
            string str = string.Format("select * from manager" +
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

        // POST: api/Manager
        /// <summary>
        /// 新增管理员
        /// </summary>
        /// <param name="obj">管理员信息
        /// 示例：
        /// "ID":"12300",
        /// "name":"lucky_",
        /// "pwd":"1",
        /// "idCardNo":"2",
        /// "gender":"male",
        /// "age":"18",
        /// "phone":"110",
        /// "position":"leader"
        /// </param>
        /// <returns></returns>
        public bool Post(dynamic obj)
        {
            Manager manager = new Manager(obj);
            string id = "22" + System.DateTime.Now.ToString("yyyyMMdd") + "00000001";
            string sql = string.Format("INSERT /*+ IGNORE_ROW_ON_DUPKEY_INDEX(MANAGER(ID)) */ INTO MANAGER(ID,NAME,PWD,IDCARDNO,GENDER,AGE,PHONE,POSITION)" +
                "                   VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')",
                id,manager.name, manager.pwd, manager.idCardNo, manager.gender, manager.age, manager.phone, manager.position);
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
        // PUT: api/Manager
        /// <summary>
        /// 根据id修改管理员信息
        /// </summary>
        /// <param name="id">管理员id</param>
        /// <param name="obj">修改后的信息
        /// 示例：
        /// "ID":"12300",
        /// "name":"lucky_",
        /// "pwd":"1",
        /// "idCardNo":"2",
        /// "gender":"male",
        /// "age":"18",
        /// "phone":"110",
        /// "position":"leader"
        /// </param>
        /// <returns></returns>
        public Boolean Put(string id, dynamic obj)
        {
            Manager manager = new Manager(obj);
            string sql = string.Format(
                "UPDATE MANAGER " +
                "SET " +
                "NAME='{0}'," +
                "PWD='{1}'," +
                "IDCARDNO='{2}'," +
                "GENDER='{3}'," +
                "AGE='{4}'," +
                "PHONE='{5}'," +
                "POSITION='{6}'" +
                "WHERE " +
                "ID='{7}'",
                manager.name, manager.pwd, manager.idCardNo, manager.gender, manager.age, manager.phone,manager.position, id); ;
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
        // DELETE: api/Manager
        /// <summary>
        /// 根据id删除管理员信息
        /// </summary>
        /// <param name="id">管理员id</param>
        /// <returns></returns>
        public Boolean Delete(string id)
        {
            string sql = string.Format("DELETE FROM MANAGER WHERE ID=" + id);
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


    }
}
