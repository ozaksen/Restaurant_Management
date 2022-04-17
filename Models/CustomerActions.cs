using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QrMenuAgain.Models
{
    public class CustomerActions
    {
        public string id { get; set; }
        public string name { get; set; }
        public string icon { get; set; }

        public CustomerActions(string id, string name, string icon) {
            this.id = id;
            this.name = name;
            this.icon = icon;
        }
    }
}
