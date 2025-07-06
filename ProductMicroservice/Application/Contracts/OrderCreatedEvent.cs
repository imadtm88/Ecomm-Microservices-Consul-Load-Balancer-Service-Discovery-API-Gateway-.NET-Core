using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contracts
{
    public class OrderCreatedEvent
    {
        public string Id { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public List<OrderItem> Items { get; set; } = new();
        public decimal Total { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
