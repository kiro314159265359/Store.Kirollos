using AutoMapper;
using Domain.Contracts;
using Domain.Exceptions;
using Domain.Models;
using Domain.Models.OrderModels;
using Microsoft.Extensions.Configuration;
using Services.Abstractions;
using Shared;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderProduct = Domain.Models.Product;

namespace Services
{
    public class PaymentService(
        IBasketRepository basketRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IConfiguration configuration) : IPaymentService
    {
        public async Task<BasketDto> CreateOrUpdatePaymentIntentAsync(string basketId)
        {
            var basket = await basketRepository.GetBasketAsync(basketId);
            if (basket is null)
            {
                throw new BasketNotFoundExecption(basketId);
            }

            foreach (var item in basket.Items)
            {
                var product = await unitOfWork.GetRepository<OrderProduct, int>().GetAsync(item.Id);
                if (product is null)
                    throw new ProductNotFoundExecption(item.Id);

                item.Price = product.Price;
            }

            if (!basket.DeliveryMethodId.HasValue)
                throw new Exception("Invalid Delivey Method !");

            var DeliveryMethod = await unitOfWork.GetRepository<DeliveryMethod, int>().GetAsync(basket.DeliveryMethodId.Value);
            if (DeliveryMethod is null)
                throw new DeliveryMethodNotFoundExecption(basket.DeliveryMethodId.Value);

            basket.ShippingPrice = DeliveryMethod.Cost;

            var amount = (long)(basket.Items.Sum(i => i.Price * i.Quantity) + basket.ShippingPrice) * 100;

            StripeConfiguration.ApiKey = configuration["StripeSettings:SecretKey"];

            var service = new PaymentIntentService();

            if (string.IsNullOrEmpty(basket.PaymentIntentId))
            {
                // Create new paymentIntent
                var createOptions = new PaymentIntentCreateOptions()
                {
                    Amount = amount,
                    Currency = "USD",
                    PaymentMethodTypes = new List<string>() { "card" }
                };

                var paymentIntent = await service.CreateAsync(createOptions);
                basket.PaymentIntentId = paymentIntent.Id;
                basket.ClientSecret = paymentIntent.ClientSecret;

            }
            else
            {
                var updateOptions = new PaymentIntentUpdateOptions()
                {
                    Amount = amount
                };

                await service.UpdateAsync(basket.PaymentIntentId, updateOptions);

            }

            await basketRepository.UpdateBasketAsync(basket);

            var result = mapper.Map<BasketDto>(basket);

            return result;
        }
    }
}
