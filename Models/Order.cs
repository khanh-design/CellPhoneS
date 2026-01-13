using System;
using System.Collections.Generic;

namespace CPhoneS.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public string PaymentMethod { get; set; }
        public string Status { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
