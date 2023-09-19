using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jujialaorenfuwuxitong.Models
{
    public class HistoryDisease
    {
        public string elderid;
        public string diseaseid;
        public string starttime;
        public HistoryDisease(dynamic obj)
        {
            elderid = Convert.ToString(obj.elderid);
            diseaseid = Convert.ToString(obj.diseaseid);
            starttime = Convert.ToString(obj.starttime);
        }
        public HistoryDisease()
        {
            ;
        }
    }
}