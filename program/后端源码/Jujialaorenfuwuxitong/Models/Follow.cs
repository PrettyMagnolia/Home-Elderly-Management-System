using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jujialaorenfuwuxitong.Models
{
    public class Follow
    {
        public string FOLLOWINGID;
        public string FOLLOWERID;
        public Follow(dynamic obj)
        {
            FOLLOWINGID = Convert.ToString(obj.FOLLOWINGID);
            FOLLOWERID = Convert.ToString(obj.FOLLOWERID);
        }
    }
}