using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jujialaorenfuwuxitong.Models
{
    public class NursingWorker
    {
        public string ID;
        public string name;
        public string pwd;
        public string idCardNo;
        public string gender;
        public string age;
        public string phone;
        public string description;
        public string photo;
        public string userstatus;
        public string institutionid;
        public NursingWorker(dynamic obj)
        {
            name = Convert.ToString(obj.name);
            pwd = Convert.ToString(obj.pwd);
            idCardNo = Convert.ToString(obj.idcardno);
            gender = Convert.ToString(obj.gender);
            age = Convert.ToString(obj.age);
            phone = Convert.ToString(obj.phone);
            description = Convert.ToString(obj.description);
            photo = Convert.ToString(obj.photo);
            userstatus = Convert.ToString(obj.userstatus);
            institutionid = Convert.ToString(obj.institutionid);
        }
    }
}