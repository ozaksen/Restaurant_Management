using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QrMenuAgain.Models
{
    public class ResponseCustomerMenu
    {
        public string type = "interactive";
        public CustomerMenuPayload payload { get; set; }
    }
}
