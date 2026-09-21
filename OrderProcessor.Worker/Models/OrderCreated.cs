using System;
using System.Collections.Generic;
using System.Text;

namespace OrderProcessor.Worker.Models
{
    public class OrderCreated
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public decimal Amount { get; set; }
    }
}
