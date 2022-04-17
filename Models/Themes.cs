using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QrMenuAgain.Models
{
    public class Themes
    {
        public long Id { get; set; }
        public string AccentColor { get; set; }
        public string AccentColorDark { get; set; }
        public string Name { get; set; }
        public bool isDefault { get; set; }
    }
}
