using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QrMenuAgain.Models
{
    public class Menus
    {
        public long Id { get; set; }
        public long RestaurantId { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public string CurrencySymbol { get; set; }

        public Menus(string Name, bool IsActive, string CurrencySymbol, long RestaurantId) {
            this.Name = Name;
            this.IsActive = IsActive;
            this.CurrencySymbol = CurrencySymbol;
            this.RestaurantId = RestaurantId;
        }
    
    }
}
