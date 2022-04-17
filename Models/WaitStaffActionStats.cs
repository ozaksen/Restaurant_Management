using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QrMenuAgain.Models
{
    public class WaitStaffActionStats
    {
        public long Id { get; set; }
        public int tableNumber { get; set; }
        public DateTime CompleteTime { get; set; }
        public string EmployeeName { get; set; }
        public string request { get; set; }
    }
}
