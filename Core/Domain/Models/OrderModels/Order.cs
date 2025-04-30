using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.OrderModels
{
    public class Order : BaseEntity<Guid>
    {
        public Order()
        {
            
        }
        public Order(string userEmail, Address shippingAddress, ICollection<OrderItem> orderItems, DeliveryMethod deliveryMethod, decimal subTotal, string paymentIntentId)
        {
            Id = Guid.NewGuid();
            UserEmail = userEmail;
            ShippingAddress = shippingAddress;
            OrderItems = orderItems;
            DeliveryMethod = deliveryMethod;
            SubTotal = subTotal;
            PaymentIntentId = paymentIntentId;
        }
        // Id
        // User Email
        public string UserEmail { get; set; }
        // Shipping address
        public Address ShippingAddress { get; set; }

        // Order Item
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>(); // navigational property

        // DeliveryMethod
        public DeliveryMethod DeliveryMethod { get; set; } // navigational property
        public int? DeliveryMethodId { get; set; } // FK

        // Payment Status
        public OrderPaymentStatus paymentStatus { get; set; } = OrderPaymentStatus.Pending;

        // SubTotal
        public decimal SubTotal { get; set; }

        // Order date
        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.Now;

        // payment
        public string PaymentIntentId { get; set; }
    }
}
