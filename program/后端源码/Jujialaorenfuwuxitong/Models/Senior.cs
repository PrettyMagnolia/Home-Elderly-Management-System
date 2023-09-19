using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jujialaorenfuwuxitong.Models
{
    public class Senior
    {
        public string ID;
        public string name;
        public string pwd;
        public string idCardNo;
        public string gender;
        public string age;
        public string phone;
        public string height;
        public string weight;
        public string address;
        public string urgent;
        public string situation;
        public string communityId;
        public Senior(dynamic obj)
        {
            name = Convert.ToString(obj.name);
            pwd = Convert.ToString(obj.pwd);
            idCardNo = Convert.ToString(obj.idcardno);
            gender = Convert.ToString(obj.gender);
            age = Convert.ToString(obj.age);
            phone = Convert.ToString(obj.phone);
            height = Convert.ToString(obj.height);
            weight = Convert.ToString(obj.weight);
            address = Convert.ToString(obj.address);
            urgent = Convert.ToString(obj.urgent);
            situation = Convert.ToString(obj.situation);
            communityId = Convert.ToString(obj.communityid);
        }
    }
}