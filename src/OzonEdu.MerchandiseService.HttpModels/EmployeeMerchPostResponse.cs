using System.Collections.Generic;

namespace OzonEdu.MerchandiseService.HttpModels
{
    public class EmployeeMerchPostResponse
    {
        public IEnumerable<EmployeeMerchItem> Items { get; set; }
    }
}