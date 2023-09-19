using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jujialaorenfuwuxitong.Models
{
    public class Manager
    {
        public string ID;
        public string name;
        public string pwd;
        public string idCardNo;
        public string gender;
        public string age;
        public string phone;
        public string position;
        public Manager(dynamic obj)
        {
            name = Convert.ToString(obj.name);
            pwd = Convert.ToString(obj.pwd);
            idCardNo = Convert.ToString(obj.idcardno);
            gender = Convert.ToString(obj.gender);
            age = Convert.ToString(obj.age);
            phone = Convert.ToString(obj.phone);
            position = Convert.ToString(obj.position);
        }
    }
}