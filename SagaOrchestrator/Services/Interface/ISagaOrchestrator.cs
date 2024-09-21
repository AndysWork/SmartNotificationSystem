using SagaOrchestrator.Models;

namespace SagaOrchestrator.Services.Interface
{
    public interface ISagaOrchestrator
    {
        Task StartSagaAsync(Order order);
    }
}
