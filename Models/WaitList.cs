using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QrMenuAgain.Models
{
    public class WaitList
    {
        public long Id { get; set; }
        public DateTime EnterDate { get; set; }
        public string Name { get; set; }
        public int NumberOfPeople { get; set; }
        public string Commnet { get; set; }
    }
}
