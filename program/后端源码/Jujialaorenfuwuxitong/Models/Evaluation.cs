using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jujialaorenfuwuxitong.Models
{
    public class Evaluation
    {
        public string evaluationId;
        public DateTime time;
        public char grade;
        public string message;
        public Evaluation(dynamic obj)
        {
            evaluationId = obj.id;
            time = obj.time;
            grade = obj.grade;
            message = obj.message;
        }
    }
}