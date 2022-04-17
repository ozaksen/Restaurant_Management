using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QrMenuAgain.Models
{
    public class StaffHelpRequests
    {
        public long id { get; set; }
        public long StaffName { get; set; }
        public string Position { get; set; }
        public string RequestType { get; set; }
        public string Description { get; set; }
    }
}
