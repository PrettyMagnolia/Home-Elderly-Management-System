using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jujialaorenfuwuxitong.Models
{
    public class Report
    {
        public string ID;
        public string imformerID;//控告人ID（老人）
        public string imformerName;
        public string defendantID;//被告人ID（护工）
        public string defendantName;
        public string orderID;//订单ID

        public string type;//举报原因
        public string reportTime;//举报时间
        public string auditTime;//审核时间
        public string isReal;//举报是否属实
        public string punitiveMeasure;//惩罚措施
        public string reportText;//举报详述

        public string isDone;//举报单是否审核完成
        public string photo;

        public Report(dynamic obj)
        {
            ID = Convert.ToString(obj.ID);
            imformerID = Convert.ToString(obj.imformerID);
            imformerName = Convert.ToString(obj.imformerName);
            defendantID = Convert.ToString(obj.defendantID);
            defendantName = Convert.ToString(obj.defendantName);
            orderID = Convert.ToString(obj.orderID);
            type = Convert.ToString(obj.type);
            reportTime = Convert.ToString(obj.reportTime);
            auditTime = Convert.ToString(obj.auditTime);
            isReal = Convert.ToString(obj.isReal);
            punitiveMeasure = Convert.ToString(obj.punitiveMeasure);
            reportText = Convert.ToString(obj.reportText);

            isDone = Convert.ToString(obj.isDone);
            photo = Convert.ToString(obj.photo);
        }
    }
}
