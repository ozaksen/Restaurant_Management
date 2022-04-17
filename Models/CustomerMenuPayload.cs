using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QrMenuAgain.Models
{
    public class CustomerMenuPayload
    {
        public string name { get; set; }
        public string currencySymbol { get; set; }
        public List<FoodItem> items {get; set;}
    }
}
