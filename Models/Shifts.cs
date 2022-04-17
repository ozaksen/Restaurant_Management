using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QRMenuBackEnd.Models
{
    public class Shifts
    {
        public long Id { get; set; }
        public string Employee { get; set; }
        public string ShiftStart { get; set; }
        public string ShiftEnd { get; set; }
        public string ShiftDuration { get; set; }
        public long RestaurantId { get; set; }

    }
}
