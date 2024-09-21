using Microsoft.AspNetCore.Mvc;
using OrderService.Models;
using OrderService.Services.Interface;

namespace OrderService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController(IEventPublisher eventPublisher) : ControllerBase
    {
        private readonly IEventPublisher _eventPublisher = eventPublisher;

        [HttpPost]
        public IActionResult CreateOrder([FromBody] Order order)
        {
            order.Id = Guid.NewGuid();
            order.CreatedAt = DateTime.Now;

            //Publishing Event
            _eventPublisher.Publish(order);

            return Ok(order);
        }
    }
}
