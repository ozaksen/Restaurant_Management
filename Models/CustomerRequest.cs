using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using QrMenuAgain;
using QrMenuAgain.Base;

namespace QrMenuAgain.Models
{
    public class CustomerRequest
    {
        public long id { get; set; }
        public string sessionId { get; set; }
        public int tableId { get; set; }
        public string reason { get; set; }
        public bool isSnoozed { get; set; }
        public string nameWaiter { get; set; }
        public bool isSentToBusboy { get; set; }

    }
}