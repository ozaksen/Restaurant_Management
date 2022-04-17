using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace QrMenuAgain.Models
{
    public class Tables
    {

        public long id { get; set; }
        public long AssignedEmployeeId { get; set; }
        public long RestaurantId { get; set; }
        public int TableNumber { get; set; }
        public bool IsOccupied { get; set; }
        public string TableSid { get; set; }
        [NotMapped]
        public static List<Orders> TotalOrders { get; set; }
        public double TotalPrice { get; set; }
        public bool IsSomeonePaying {get; set;}
        public void addOrder(Orders order)
        {
            if (TotalOrders == null) {
                TotalOrders = new List<Orders>();
            }
            TotalOrders.Add(order);
        }

        public void clearList()
        {
            TotalOrders.Clear();
        }
        public List<Orders> getList()
        {
            return TotalOrders;

        }
        public void initializeList() {
            TotalOrders = new List<Orders>();
        }
    }
}
