using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace QrMenuAgain.Models
{
    public class Employees : IdentityUser
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Hours { get; set; }
        public double Salary { get; set; }
        override
        public string Email { get; set; }
        public string Password { get; set; }
        public long RestaurantId { get; set; }

        public List<Tables> TableIds { get; set; }
        public string Token { get; set; }
        public string Role { get; set; }

        public string getFullName() {
            return FirstName + " " + LastName;
        }


    }
}
