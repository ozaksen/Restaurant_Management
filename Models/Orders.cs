using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QrMenuAgain.Models
{
    [Keyless]
    public class Orders
    {
        public long FoodId { get; set; }
        public int Quantity { get; set; }
        public string CustomerId { get; set; }
        public bool isSubmitted { get; set; }

    }
}
