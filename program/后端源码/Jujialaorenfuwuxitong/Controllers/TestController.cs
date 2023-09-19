using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Jujialaorenfuwuxitong.Controllers
{
    public class TestController : ApiController
    {
        // GET api/<controller> 
        public HttpResponseMessage Get()
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, "value");
            response.Content = new StringContent("HelloWorld!");
            return response;
        }
        //加一行注释test一下
    }
}