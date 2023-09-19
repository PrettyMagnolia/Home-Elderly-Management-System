using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jujialaorenfuwuxitong.Models
{
    public class Community
    {
        public string name;
        public string city;
        public string block;
        public string communityId;
        public Community(dynamic obj)
        {
            name = Convert.ToString(obj.name);
            city = Convert.ToString(obj.city);
            block = Convert.ToString(obj.block);
            communityId = Convert.ToString(obj.communityid);
        }
    }
}