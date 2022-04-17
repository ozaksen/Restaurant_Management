using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QrMenuAgain.Models
{

    public class TableStatistics
    {
        public long Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? FinishTime { get; set; }
        public int TableNumber { get; set; }
        public string timeSpent { get; set; }
        public double TotalAmount { get; set; }
        public int CustomerSize { get; set; }
        public string Server { get; set; }
        public long RestaurantId { get; set; }

    }
}
