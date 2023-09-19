using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Jujialaorenfuwuxitong.Models;

namespace Jujialaorenfuwuxitong.Controllers
{
    [RoutePrefix("api/file")]
    public class FileController : ApiController
    {
        [HttpGet]
        [Route("photo/{type}/{name}/{suffix}")]
        public HttpResponseMessage GetPhoto(string type,string name,string suffix) {
            string path = @"C:\photo\" + type + @"\" + name + "." + suffix;
            var img = StreamHelp.FileToStream(path);
            var res = new HttpResponseMessage { Content = new StreamContent(img) };
            return res;
        }

    }
}
