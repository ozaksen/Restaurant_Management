using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QRMenuBackEnd.Models
{
    public class Breaks
    {
        public long Id { get; set; }
        public string Employee { get; set; }
        public string BreakStart { get; set; }
        public string BreakEnd { get; set; }
        public string BreakDuration { get; set; }
        public long RestaurantId { get; set; }

    }
}
