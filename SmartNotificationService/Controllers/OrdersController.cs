using Microsoft.AspNetCore.Mvc;
using SmartNotificationService.Models;
using SmartNotificationService.Services.Interface;

namespace SmartNotificationService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IEventPublisher _eventPublisher;

        public OrdersController(IEventPublisher eventPublisher) => _eventPublisher = eventPublisher;

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
