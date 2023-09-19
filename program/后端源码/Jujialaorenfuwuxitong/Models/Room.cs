using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jujialaorenfuwuxitong.Models
{
    public class Room
    {
        public string roomid;
        public string starttime;
        public string doctorid;
        public string communityid;
        public string endtime;
        public string password;
        public Room(dynamic obj)
        {
            roomid = Convert.ToString(obj.roomid);
            starttime = Convert.ToString(obj.starttime);
            doctorid = Convert.ToString(obj.doctorid);
            communityid = Convert.ToString(obj.communityid);
            endtime = Convert.ToString(obj.endtime);
            password = Convert.ToString(obj.password);
        }
    }
}