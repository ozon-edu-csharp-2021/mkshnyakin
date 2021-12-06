using System;

namespace OzonEdu.MerchandiseService.Infrastructure.Repositories.Models
{
    public class MerchRequest
    {
        public long Id { get; set; }
        public long EmployeeId { get; set; }
        public int MerchType { get; set; }
        public int Status { get; set; }
        public int Mode { get; set; }
        public DateTime? GiveOutDate { get; set; }
        public bool IsEmailSended { get; set; }
    }
}