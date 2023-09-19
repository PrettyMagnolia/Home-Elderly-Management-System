using Jujialaorenfuwuxitong.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace Jujialaorenfuwuxitong.Controllers
{
    public class NurseTipsController : ApiController
    {
        // GET: api/NurseTips
        public HttpResponseMessage Get()

        {
            string str = "Select TITLE,CONTENT,STATUS,to_char(time,'yyyy-mm-dd hh24:mi:ss') from NurseTips";
            string message = "";
            string json = OracleHelper.SelectSql_v2(str, ref message);
            if (json == null)
            {
                return new HttpResponseMessage
                {
                    Content = new StringContent("{\"status\":\"error\",\"message\":\"" + message + "\"}", Encoding.UTF8, "application/json")
                };
            }
            json = json.Replace("TO_CHAR(TIME,\\u0027YYYY-MM-DD\\u0027)", "time");
            return new HttpResponseMessage()
            {
                Content = new StringContent("{\"status\":\"success\",\"message\":" + json + "}", Encoding.UTF8, "application/json"),
            };
        }

        
    }
}
