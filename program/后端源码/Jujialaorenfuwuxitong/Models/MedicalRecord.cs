using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jujialaorenfuwuxitong.Models
{
    public class MedicalRecord
    {
        public string serviceid;
        public string doctorid;
        public string elderid;
        public string starttime;
        public string endtime;
        public string orderid;
        public string evaluationid;
        public string timeslot;
        public string evalustatus;
        public string status;
        public string priority;
        public string address;

        public MedicalRecord(dynamic obj)
        {
            serviceid = Convert.ToString(obj.serviceid);
            doctorid = Convert.ToString(obj.doctorid);
            elderid = Convert.ToString(obj.elderid);
            starttime = Convert.ToString(obj.starttime);
            endtime = Convert.ToString(obj.endtime);
            orderid = Convert.ToString(obj.orderid);
            evaluationid = Convert.ToString(obj.evaluationid);
            timeslot = Convert.ToString(obj.timeslot);
            evalustatus = Convert.ToString(obj.evalustatus);
            status = Convert.ToString(obj.status);
            priority = Convert.ToString(obj.priority);
            address = Convert.ToString(obj.address);
        }
    }
}