using Shared.OrderModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Abstractions
{
    public interface IOrderService
    {
        // Get Order By Id Async()
        Task<OrderResultDto> GetOrderByIdAsync(Guid id);
        // Get user orders by email
        Task<IEnumerable<OrderResultDto>> GetOrdersByUserEmailAsync(string userEmail);
        // Create Order
        Task<OrderResultDto>CreateOrderAsync(OrderRequestDto orderRequest, string userEmail);

        // Get All deliverymethods
        Task<IEnumerable<DeliveryMethodDto>> GetAllDeliveryMethods();
    }
}
