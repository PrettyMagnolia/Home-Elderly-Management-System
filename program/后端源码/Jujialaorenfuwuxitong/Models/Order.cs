using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jujialaorenfuwuxitong.Models
{
    public class Order
    {
        public string orderID;
        public string orderStatus;
        public string method;
        public int money;
        public Order(dynamic obj)
        {
            orderID = obj.id;
            orderStatus = obj.status;
            method = obj.method;
            money = obj.money;
        }
    }
   
}