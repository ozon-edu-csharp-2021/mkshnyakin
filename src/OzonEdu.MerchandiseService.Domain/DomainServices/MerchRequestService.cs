using System;
using System.Collections.Generic;
using OzonEdu.MerchandiseService.Domain.AggregationModels.EmployeeAggregate;
using OzonEdu.MerchandiseService.Domain.AggregationModels.MerchPackItemAggregate;
using OzonEdu.MerchandiseService.Domain.AggregationModels.MerchRequestAggregate;

namespace OzonEdu.MerchandiseService.Domain.DomainServices
{
    public class MerchRequestService
    {
        public static MerchRequest CreateMerchandizeRequest(Employee employee, IEnumerable<MerchPackItem> merchItems)
        {
            //var request = new MerchRequest();
            /*
            var request = new MerchandizeRequest(employee.Id, employee.PhoneNumber);

            if (!managers.Any(m => m.CanHandleNewTask))
            {
                throw new Exception("No vacant managers");
            }

            var responsibleManager = managers.OrderBy(m => m.AssignedTasks).First();

            request.AssignTo(responsibleManager.Id);
            responsibleManager.AssignTask();
            */

            //return request;
            return null;
        }
    }
}