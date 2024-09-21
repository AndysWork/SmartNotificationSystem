﻿namespace SagaOrchestrator.Models
{
    public class Order
    {
        public Guid Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; } = 0;
        public DateTime CreatedAt { get; set; }
    }
}
