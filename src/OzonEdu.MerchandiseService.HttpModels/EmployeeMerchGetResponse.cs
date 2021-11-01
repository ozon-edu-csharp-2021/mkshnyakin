using System.Collections.Generic;

namespace OzonEdu.MerchandiseService.HttpModels
{
    public class EmployeeMerchGetResponse
    {
        public IEnumerable<EmployeeMerchGetResponseItem> Items { get; set; }
    }
}