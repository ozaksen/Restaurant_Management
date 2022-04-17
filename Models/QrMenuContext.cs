using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QrMenuAgain.Models;
using QRMenuBackEnd.Models;

namespace QrMenuAgain.Models
{
    public class QrMenuContext : DbContext
    {
        public QrMenuContext(DbContextOptions<QrMenuContext> options)
            : base(options)
        {
        }
    
        public DbSet<Restaurant> Restaurant { get; set; }

        public DbSet<CustomerRequest> CustomerRequest { get; set; }
        public DbSet<Shifts> Shifts { get; set; }
        public DbSet<Menus> Menus { get; set; }

        public DbSet<FoodItem> FoodItem { get; set; }

        public DbSet<Breaks> Breaks { get; set; }

        public DbSet<UserReviews> UserReviews { get; set; }

        public DbSet<Orders> Orders { get; set; }
        
        public DbSet<Tables> Table { get; set; }

        public DbSet<TableStatistics> TableStatistics { get; set; }

        public DbSet<Employees> Employees { get; set; }

        public DbSet<WaitStaffActionStats> WaitStaffActionStats { get; set; }



    }
}
