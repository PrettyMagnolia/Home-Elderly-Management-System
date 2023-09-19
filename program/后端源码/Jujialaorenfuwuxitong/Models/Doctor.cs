using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jujialaorenfuwuxitong.Models
{
    public class Doctor
    {
        //为什么这里都设为string类型呢？
        public string ID;
        public string name;
        public string pwd;
        public string idCardNo;
        public string age;
        public string phone;
        public string field;
        public string history;
        public string communityId;
        public string gender;
        public string bantime;

        public Doctor(dynamic obj)
        {
            name = Convert.ToString(obj.name);
            pwd = Convert.ToString(obj.pwd);
            idCardNo = Convert.ToString(obj.idcardno);
            age = Convert.ToString(obj.age);
            phone = Convert.ToString(obj.phone);
            field = Convert.ToString(obj.field);
            history = Convert.ToString(obj.history);
            communityId = Convert.ToString(obj.communityid);
            gender = Convert.ToString(obj.gender);
            bantime = Convert.ToString(obj.bantime);
        }
    }
}