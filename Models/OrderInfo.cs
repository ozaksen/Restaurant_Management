using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QrMenuAgain.Models
{
    public class OrderInfo
    {
        public long Id { get; set; }
        public string SessionId { get; set; }
        public List<Orders> OrderList { get; set; } = new List<Orders>();

    }
}
