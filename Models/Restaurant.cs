using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QrMenuAgain.Models
{
    public class Restaurant
    {
        public long Id { get; set; }
        public string RestaurantName { get; set; }
        public string Description { get; set; }
        public string Cuisine { get; set; }
        public string Address { get; set; }
        public string Price { get; set; }
        public int TableCount { get; set; }
        
        //public Level Price { get; set; }
    }

    public enum Level
    {
        Low,
        Medium,
        High
    }

}
