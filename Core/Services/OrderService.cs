using AutoMapper;
using Domain.Contracts;
using Domain.Exceptions;
using Domain.Models;
using Domain.Models.OrderModels;
using Services.Abstractions;
using Services.Specifications;
using Shared.OrderModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class OrderService(
        IMapper mapper,
        IBasketRepository basketRepository,
        IUnitOfWork unitOfWork
        ) : IOrderService
    {
        public async Task<OrderResultDto> CreateOrderAsync(OrderRequestDto orderRequest, string userEmail)
        {
            // 1. Address
            var address = mapper.Map<Address>(orderRequest.ShipToAddress);

            // 2. order items => Basket
            var basket = await basketRepository.GetBasketAsync(orderRequest.BasketId);
            if (basket is null)
                throw new BasketNotFoundExecption(orderRequest.BasketId);

            var orderItems = new List<OrderItem>();
            foreach (var item in basket.Items)
            {
                var product = await unitOfWork.GetRepository<Product, int>().GetAsync(item.Id);
                if (product is null)
                    throw new ProductNotFoundExecption(item.Id);

                var orderItem = new OrderItem(new ProductInOrderItem(product.Id, product.Name, product.PictureUrl), item.Quantity, product.Price);
                orderItems.Add(orderItem);
            }
            // 3. Get delivery mehtod
            var deliveryMethod = await unitOfWork.GetRepository<DeliveryMethod, int>().GetAsync(orderRequest.DeliveryMethodId);
            if (deliveryMethod is null)
                throw new DeliveryMethodNotFoundExecption(orderRequest.DeliveryMethodId);

            // 4. Subtotal
            var subTotal = orderItems.Sum(i => i.Price * i.Quantity);

            // 5. Payment Intent ID..
            // check order exists or not
            var spec = new OrderWithPaymentIntentSpecifications(basket.PaymentIntentId);
            var existOrder = await unitOfWork.GetRepository<Order, Guid>().GetAsync(spec);

            if (existOrder is not null)
                unitOfWork.GetRepository<Order, Guid>().Delete(existOrder);

            // Create Order
            var order = new Order(userEmail, address, orderItems, deliveryMethod, subTotal, basket.PaymentIntentId);
            await unitOfWork.GetRepository<Order, Guid>().AddAsync(order);
            var count = await unitOfWork.SaveChangeAsync();

            if (count == 0)
                throw new OrderCreateBadRequestException();

            var result = mapper.Map<OrderResultDto>(order);
            return result;
        }

        public async Task<IEnumerable<DeliveryMethodDto>> GetAllDeliveryMethods()
        {
            var deliveryMehtod = await unitOfWork.GetRepository<DeliveryMethod, int>().GetAllAsync();

            var result = mapper.Map<IEnumerable<DeliveryMethodDto>>(deliveryMehtod);
            return result;
        }

        public async Task<OrderResultDto> GetOrderByIdAsync(Guid id)
        {
            var spec = new OrderSpecifications(id);

            var order = await unitOfWork.GetRepository<Order, Guid>().GetAsync(spec);
            if (order is null)
                throw new OrderNotFoundExecption(id);

            var result = mapper.Map<OrderResultDto>(order);
            return result;
        }

        public async Task<IEnumerable<OrderResultDto>> GetOrdersByUserEmailAsync(string userEmail)
        {
            var spec = new OrderSpecifications(userEmail);

            var orders = await unitOfWork.GetRepository<Order, Guid>().GetAllAsync(spec);


            var result = mapper.Map<IEnumerable<OrderResultDto>>(orders);
            return result;
        }
    }
}
