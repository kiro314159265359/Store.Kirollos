using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.OrderModels
{
    public class OrderResultDto
    {
        // Id
        public Guid Id { get; set; }
        // User Email
        public string UserEmail { get; set; }
        // Shipping address
        public AddressDto ShippingAddress { get; set; }

        // Order Item
        public ICollection<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>(); // navigational property

        // DeliveryMethod
        public string DeliveryMethod { get; set; }

        // Payment Status
        public string paymentStatus { get; set; }

        // SubTotal
        public decimal SubTotal { get; set; }

        // Order date
        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.Now;

        // payment
        public string PaymentIntentId { get; set; } = string.Empty;

        public decimal TotalValue { get; set; }
    }
}
