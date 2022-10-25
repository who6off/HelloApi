﻿using HelloApi.Models;
using HelloApi.Models.Requests;
using HelloApi.Repositories.Interfaces;
using HelloApi.Services.Interfaces;

namespace HelloApi.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }


        public async Task<Order?> GetById(int id)
        {
            return await _orderRepository.GetById(id);
        }


        public async Task<Order?> Add(Order order)
        {
            var existingOrder = await _orderRepository.FindUnrequestedForDeliveryOrder(order.BuyerId.Value);
            if (existingOrder != null)
                return null;

            var newOrder = await _orderRepository.Add(order);
            return newOrder;
        }


        public async Task<bool> Delete(int id)
        {
            var order = await _orderRepository.GetById(id);

            if (order is null)
                return false;

            if (order.IsRequestedForDelivery)
                return false;

            var isDeleted = await _orderRepository.Delete(id);
            return isDeleted;
        }


        public async Task<Order?> RequestDelivery(int id)
        {
            var order = await _orderRepository.GetById(id);
            order.IsRequestedForDelivery = true;
            order.Date = DateTime.Now;

            order = await _orderRepository.Update(order);

            return order;
        }


        public async Task<OrderItem?> GetOrderItemById(int id)
        {
            var orderItem = await _orderRepository.GetOrderItemById(id);
            return orderItem;
        }


        public async Task<OrderItem?> AddProductToOrder(OrderProductRequest request, int buyerId)
        {
            var orderId =
                request.OrderId ??
                (await _orderRepository.FindUnrequestedForDeliveryOrder(buyerId))?.Id;

            if (orderId is not null)
            {
                var orderItem = await _orderRepository.FindOrderItem(orderId.Value, request.ProductId);

                if (orderItem is not null)
                {
                    orderItem.Amount += request.Amount;
                    return await _orderRepository.UpdateOrderItem(orderItem);
                }
            }

            if (orderId is null)
                orderId = (await _orderRepository.Add(new Order()
                {
                    BuyerId = buyerId,
                    Date = DateTime.Now
                }))?.Id;

            var result = (orderId is null)
                ? null
                : await _orderRepository.AddProductToOrder(new OrderItem()
                {
                    ProductId = request.ProductId,
                    Amount = request.Amount,
                    OrderId = orderId.Value,
                });

            return result;
        }


        public async Task<OrderItem?> UpdateProductInOrder(OrderProductUpdateRequest request, OrderItem orderItem)
        {
            var updatedOrderItem = await _orderRepository.UpdateOrderItem(new OrderItem()
            {
                Id = request.Id,
                Amount = request.Amount,
                ProductId = orderItem.ProductId,
                OrderId = orderItem.OrderId
            });

            return updatedOrderItem;
        }

        public async Task<bool> DeleteProductInOrder(OrderItem orderItem)
        {
            var isDeleted = await _orderRepository.DeleteOrderItem(orderItem);

            return isDeleted;
        }

    }
}
