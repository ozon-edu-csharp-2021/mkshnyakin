namespace OzonEdu.MerchandiseService.Infrastructure.Configuration
{
    public class KafkaConfiguration
    {
        public string BootstrapServers { get; set; }
        public string GroupId { get; set; }
        public string EmailingServiceTopic { get; set; }
    }
}