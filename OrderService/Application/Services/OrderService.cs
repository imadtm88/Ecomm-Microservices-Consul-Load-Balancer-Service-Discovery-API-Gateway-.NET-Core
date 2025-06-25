using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class OrderService
    {
        private readonly IOrderRepository _repo;

        public OrderService(IOrderRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<Order>> GetAllAsync() => await _repo.GetAllAsync();

        public async Task<Order?> GetByIdAsync(string id) => await _repo.GetByIdAsync(id);

        public async Task CreateAsync(OrderDto dto)
        {
            var items = dto.Items.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList();

            var order = new Order
            {
                ClientId = dto.ClientId,
                Items = items,
                Total = items.Sum(x => x.Quantity * x.UnitPrice),
                CreatedAt = DateTime.UtcNow
            };

            await _repo.CreateAsync(order);
        }

        public async Task UpdateAsync(string id, OrderDto dto)
        {
            var order = await _repo.GetByIdAsync(id);
            if (order == null) throw new Exception("Order not found");

            var items = dto.Items.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                OrderId = id
            }).ToList();

            order.Items = items;
            order.Total = items.Sum(x => x.Quantity * x.UnitPrice);
            await _repo.UpdateAsync(order);
        }

        public async Task DeleteAsync(string id) => await _repo.DeleteAsync(id);
    }
}
