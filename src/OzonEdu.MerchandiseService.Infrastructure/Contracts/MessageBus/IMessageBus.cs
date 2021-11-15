namespace OzonEdu.MerchandiseService.Infrastructure.Contracts.MessageBus
{
    public interface IMessageBus
    {
        void Notify(IMessageBusMessage message);
    }
}