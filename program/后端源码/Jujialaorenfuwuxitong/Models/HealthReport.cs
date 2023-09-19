using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jujialaorenfuwuxitong.Models;
using System.Data;

namespace Jujialaorenfuwuxitong.Models
{
    public class HealthReport
    {
        public string reportid;
        public string doctorid;
        public string elderid;
        public string rate;
        public string pressure;
        public string healthassess;
        public string advice;
        public string historyassess;
        public string time;
        public string status;

        public HealthReport(dynamic obj)
        {
            reportid = Convert.ToString(obj.reportid);
            doctorid = Convert.ToString(obj.doctorid);
            elderid = Convert.ToString(obj.elderid);
            rate = Convert.ToString(obj.rate);
            pressure = Convert.ToString(obj.pressure);
            healthassess = Convert.ToString(obj.healthassess);
            status = Convert.ToString(obj.status);
            advice = Convert.ToString(obj.advice);
            historyassess = Convert.ToString(obj.historyassess);
            time = Convert.ToString(obj.time);
        }
    }
}