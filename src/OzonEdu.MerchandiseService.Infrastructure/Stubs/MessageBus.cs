using OzonEdu.MerchandiseService.Infrastructure.Contracts.MessageBus;

namespace OzonEdu.MerchandiseService.Infrastructure.Stubs
{
    public class MessageBus : IMessageBus
    {
        public void Notify(IMessageBusMessage message)
        {
        }
    }
}