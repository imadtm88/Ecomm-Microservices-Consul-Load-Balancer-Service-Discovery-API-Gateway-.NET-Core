using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class OrderDto
    {
        public string? Id { get; set; }
        public string ClientId { get; set; } = null!;
        public List<OrderItemDto> Items { get; set; } = new();
        public decimal Total { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
