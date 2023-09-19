using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jujialaorenfuwuxitong.Models
{
    public class Institution
    {
        public string ID;
        public string name;
        public string address;
        public string establishedTime;//机构创建时间
        public string principalPhone;//负责人联系方式
        public string isDone; //是否完成
        public string uploadTime; //上传时间
        public string auditTime; //审核时间

        public string photo;
        public Institution(dynamic obj)
        {
            ID =Convert.ToString(obj.ID);
            name = Convert.ToString(obj.name);
            address = Convert.ToString(obj.address);
            establishedTime = Convert.ToString(obj.establishedTime);
            principalPhone = Convert.ToString(obj.principalPhone);
            isDone = Convert.ToString(obj.isDone);
            uploadTime = Convert.ToString(obj.uploadTime);
            auditTime = Convert.ToString(obj.auditTime);
            photo = Convert.ToString(obj.photo);
        }
    }
}