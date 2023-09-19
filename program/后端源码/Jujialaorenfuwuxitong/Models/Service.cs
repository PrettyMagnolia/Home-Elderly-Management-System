using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jujialaorenfuwuxitong.Models
{
	public class Service
	{
		public string serviceID;
		public string name;
		public int piece_price;
		public string unit;
		public string type;
		public string content;
		public Service(dynamic obj)
        {
			serviceID = obj.serviceID;
			name = obj.name;
			piece_price = obj.piece_price;
			unit = obj.unit;
			type = obj.type;
			content = obj.content;
        }
	}
}