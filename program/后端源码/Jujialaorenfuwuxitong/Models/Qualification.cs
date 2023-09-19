using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jujialaorenfuwuxitong.Models
{
    public class Qualification
    {
        public string workerid;
        public string role;
        public string title;
        public string proof;
        public string granttime;
        public string uploadtime;
        public string adminitorid;
        public string isdone;
        public string audittime;
        public string institutionid;
        public string institutionname;
        public Qualification(dynamic obj)
        {
            workerid = Convert.ToString(obj.workerid);
            role = Convert.ToString(obj.role);
            title = Convert.ToString(obj.title);
            proof = Convert.ToString(obj.proof);
            granttime = Convert.ToString(obj.granttime);
            uploadtime = Convert.ToString(obj.uploadtime);
            adminitorid = Convert.ToString(obj.adminitorid);
            isdone = Convert.ToString(obj.isdone);
            audittime = Convert.ToString(obj.audittime);
            institutionid = Convert.ToString(obj.institutionid);
            institutionname = Convert.ToString(obj.institutionname);
        }
    }
}