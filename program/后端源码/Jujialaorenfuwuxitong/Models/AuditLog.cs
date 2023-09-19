using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jujialaorenfuwuxitong.Models
{
    public class AuditLog
    {
        public string ID;//默认记录表ID("log")
        public string nurseToAudit;
        public string reportToAudit;
        public string instituitonToAudit;

        public AuditLog(dynamic obj)
        {
            ID = Convert.ToString(obj.defaultID);
            nurseToAudit=Convert.ToString(obj.nurseToAudit);
            reportToAudit=Convert.ToString(obj.reportToAudit);
            instituitonToAudit=Convert.ToString(obj.instituitonToAudit);
        }
    }
}