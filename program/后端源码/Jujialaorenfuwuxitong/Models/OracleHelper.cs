using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using System.Data.OracleClient;
using Oracle.DataAccess.Client;
using System.Configuration;
using System.Data;
using System.Web.Script.Serialization;

namespace Jujialaorenfuwuxitong.Models
{
    public class OracleHelper
    {
        /*
         * 链接数据库，在对数据库进行操作前调用
         * 参数:message:用于返回错误信息，re返回是否成功链接数据库
         * 返回值：数据库连接
         */
        public static OracleConnection DbConn(ref string message,ref Boolean re)
        {
            OracleConnection conn;
            re = false;
            message = "链接成功";
            string constr = ConfigurationManager.ConnectionStrings["Oracle"].ConnectionString;
            conn = new OracleConnection(constr);
            try
            {
                conn.Open();

            }
            catch(Exception ex)
            {
                message = ex.Message.ToString();
                re = false;
                return null;
            }
            finally
            {
                conn.Close();
            }
            return conn;
        }
        /*对数据库进行增加操作
         * 参数：sql：数据库语句，message：用于返回错误信息，id：若存在id则会在增加时将id自动递增
         * 返回值：是否成功插入数据
         */
        public static Boolean AddSql(string sql,ref string message,string id=null)
        {
            bool re = false;
            try
            {
                OracleConnection conn = DbConn(ref message, ref re);
                if (conn == null)
                {
                    re = false;
                    //message = "数据库连接对象为空";
                }
                else
                {
                    try
                    {
                        if (conn.State == System.Data.ConnectionState.Closed)
                        {
                            conn.Open();
                        }
                    }
                    catch(Exception ex)
                    {
                        re = false;
                        message = ex.Message.ToString();
                        return re;
                    }
                    if (id != null)
                    {
                        OracleCommand command;
                        int idnum = int.Parse(id.Substring(10,8));
                        int count = 0;
                        while (count == 0)
                        {
                            command = new OracleCommand(sql, conn);
                            count = command.ExecuteNonQuery();
                            message = id;
                            idnum += 1;
                            command.Dispose();
                            //ToString("D8")，D表示十进制数，8表示位数
                            sql = sql.Replace(id, id.Substring(0, 10) + idnum.ToString("D8"));
                            id = id.Substring(0, 10) + idnum.ToString("D8");
                        }
                        re = true;
                        
                    }
                    else
                    {
                        OracleCommand command = new OracleCommand(sql, conn);
                        int count = command.ExecuteNonQuery();
                        if (count > 0)
                        {
                            re = true;
                        }
                        else
                        {
                            re = false;
                            message = "数据插入失败";
                        }
                        command.Dispose();
                    }
                    
                    conn.Close();
                }
            }
            catch(Exception ee)
            {
                re = false;
                message = ee.Message.ToString();
            }
            return re;
        }
        /*查找数据库
         * 参数：sql：sql语句，message：返回错误信息
         * SelectSql返回值：找到的DataTable
         * SelectSql_v2返回值：json字符串
         */
        public static DataTable SelectSql(string sql,ref string message)
        {
            DataTable dt = null;
            OracleConnection conn;
            message = String.Empty;
            string err = "";
            bool re = false;
            try
            {
                conn = DbConn(ref err, ref re);
                if(conn==null)
                {
                    message = err;
                    return null;
                }
                if (conn.State == ConnectionState.Closed)
                {
                    try
                    {
                        conn.Open();
                    }
                    catch(Exception e)
                    {
                        message = e.Message.ToString();
                        return null;
                    }
                }
            }
            catch(Exception e)
            {
                message = e.Message.ToString();
                return null;
            }
            try
            {
                OracleDataAdapter adapter = new OracleDataAdapter(sql, conn);
                DataSet dataSet = new DataSet();
                adapter.Fill(dataSet);
                dt = dataSet.Tables[0];
                conn.Close();
                conn.Dispose();
                adapter.Dispose();
                dataSet.Dispose();
            }
            catch(Exception e)
            {
                message = e.Message.ToString();
                return null;
            }
            return dt;
        }
        public static string SelectSql_v2(string sql,ref string message)
        {
            DataTable dt = null;
            OracleConnection conn;
            message = String.Empty;
            string err = "";
            bool re = false;
            try
            {
                conn = DbConn(ref err, ref re);
                if (conn == null)
                {
                    message = err;
                    return null;
                }
                if (conn.State == ConnectionState.Closed)
                {
                    try
                    {
                        conn.Open();
                    }
                    catch (Exception e)
                    {
                        message = e.Message.ToString();
                        return null;
                    }
                }
            }
            catch (Exception e)
            {
                message = e.Message.ToString();
                return null;
            }
            try
            {
                OracleDataAdapter adapter = new OracleDataAdapter(sql, conn);
                DataSet dataSet = new DataSet();
                adapter.Fill(dataSet);
                dt = dataSet.Tables[0];
                conn.Close();
                conn.Dispose();
                adapter.Dispose();
                dataSet.Dispose();
            }
            catch (Exception e)
            {
                message = e.Message.ToString();
                return null;
            }
       
            //使用JavaScriptSerializer，将查询出的数据表转换为JSON格式
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
            json=json.Replace("\\\"", "");
            return json;
        }

        public static Boolean UpdateSql(string sql,ref string message)
        {
            bool re = false;
            try
            {
                OracleConnection conn = DbConn(ref message, ref re);
                if (conn == null)
                {
                    re = false;
                    //message = "数据库连接对象为空";
                }
                else
                {
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
                        return re;
                    }
                    OracleCommand command = new OracleCommand(sql, conn);
                    int count = command.ExecuteNonQuery();
                    if (count > 0)
                    {
                        re = true;
                    }
                    else
                    {
                        re = false;
                        message = "数据更新失败";
                    }
                    command.Dispose();
                    conn.Close();
                }
            }
            catch (Exception ee)
            {
                re = false;
                message = ee.Message.ToString();
            }
            return re;
        }
        public static Boolean ExecuteSql(string sql, ref string message)
        {
            bool re = false;
            try
            {
                OracleConnection conn = DbConn(ref message, ref re);
                if (conn == null)
                {
                    re = false;
                    //message = "数据库连接对象为空";
                }
                else
                {
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
                        return re;
                    }
                    OracleCommand command = new OracleCommand(sql, conn);
                    int count = command.ExecuteNonQuery();
                    re = true;
                    command.Dispose();
                    conn.Close();
                }
            }
            catch (Exception ee)
            {
                re = false;
                message = ee.Message.ToString();
            }
            return re;
        }
        public static Boolean DeleteSql(string sql,ref string message)
        {
            bool re = false;
            try
            {
                OracleConnection conn = DbConn(ref message, ref re);
                if (conn == null)
                {
                    re = false;
                }
                else
                {
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
                        return re;
                    }
                    OracleCommand command = new OracleCommand(sql, conn);
                    int count = command.ExecuteNonQuery();
                    if (count > 0)
                    {
                        re = true;
                    }
                    else
                    {
                        re = false;
                        message = "数据删除失败";
                    }
                    command.Dispose();
                    conn.Close();
                }
            }
            catch (Exception ee)
            {
                re = false;
                message = ee.Message.ToString();
            }
            return re;
        }

        public static Boolean TransSql(string sql, ref string message, string id = null)
        {
            bool re = false;
            try
            {
                OracleConnection conn = DbConn(ref message, ref re);
                if (conn == null)
                {
                    re = false;
                    //message = "数据库连接对象为空";
                }
                else
                {
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
                        return re;
                    }
                    OracleTransaction tran = conn.BeginTransaction();
                    if (id != null)
                    {
                        OracleCommand command;
                        int idnum = int.Parse(id.Substring(10, 8));
                        int count = 0;
                        while (count == 0)
                        {
                            command = new OracleCommand(sql, conn);
                            count = command.ExecuteNonQuery();
                            message = id;
                            idnum += 1;
                            command.Dispose();
                            //ToString("D8")，D表示十进制数，8表示位数
                            sql = sql.Replace(id, id.Substring(0, 10) + idnum.ToString("D8"));
                            id = id.Substring(0, 10) + idnum.ToString("D8");
                        }
                        re = true;

                    }
                    else
                    {
                        OracleCommand command = new OracleCommand(sql, conn);
                        int count = command.ExecuteNonQuery();
                        if (count > 0)
                        {
                            re = true;
                        }
                        else
                        {
                            re = false;
                            message = "数据插入失败";
                            tran.Rollback();
                        }
                        command.Dispose();
                    }

                    conn.Close();
                }
            }
            catch (Exception ee)
            {
                re = false;
                message = ee.Message.ToString();
            }
            return re;
        }
        //public static Boolean TransSql(string sql,)
    }
}