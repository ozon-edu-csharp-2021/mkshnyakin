using System;

namespace OzonEdu.MerchandiseService.Infrastructure.Contracts.MessageBus
{
    public class EmailMessage : IMessageBusMessage
    {
        public string ToName { get; init; }
        public string ToEmail { get; init; }
        public string Subject { get; init; }
        public string Body { get; init; }
        public void Send()
        {
            Console.WriteLine($"To: <{ToName}> {ToEmail}");
            Console.WriteLine($"Subject: {Subject}");
            Console.WriteLine($"Body: {Body}");
        }
    }
}