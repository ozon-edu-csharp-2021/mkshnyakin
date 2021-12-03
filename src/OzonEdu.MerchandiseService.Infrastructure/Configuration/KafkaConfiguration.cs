namespace OzonEdu.MerchandiseService.Infrastructure.Configuration
{
    public class KafkaConfiguration
    {
        public string BootstrapServers { get; set; }
        public string EmployeeNotificationEventTopic { get; set; }
        public string EmployeeNotificationEventGroupId { get; set; }
        public string StockReplenishedEventTopic { get; set; }
        public string StockReplenishedEventGroupId { get; set; }
    }
}