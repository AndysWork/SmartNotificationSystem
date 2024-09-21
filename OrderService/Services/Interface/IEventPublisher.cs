namespace OrderService.Services.Interface
{
    public interface IEventPublisher
    {
        void Publish<T>(T @event);
    }
}
