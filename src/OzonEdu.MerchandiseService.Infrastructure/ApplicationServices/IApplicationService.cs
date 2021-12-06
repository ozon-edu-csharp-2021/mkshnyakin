using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OzonEdu.MerchandiseService.Domain.AggregationModels.EmployeeAggregate;
using OzonEdu.MerchandiseService.Domain.AggregationModels.MerchPackItemAggregate;
using OzonEdu.MerchandiseService.Domain.AggregationModels.MerchRequestAggregate;

namespace OzonEdu.MerchandiseService.Infrastructure.ApplicationServices
{
    public interface IApplicationService
    {
        Task<Employee> GetEmployee(long id, string email, CancellationToken cancellationToken = default);

        Task<IEnumerable<MerchRequest>> GetEmployeeMerchRequests(
            long id,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<MerchPackItem>> GetMerchPackItemsByType(
            RequestMerchType requestMerchType,
            CancellationToken cancellationToken = default);

        Task<bool> IsAvailable(MerchRequest request, CancellationToken cancellationToken = default);

        Task<bool> CanCheckAndReserve(
            MerchRequest request,
            Employee employee,
            CancellationToken cancellationToken = default);

        Task<MerchRequest> SaveRequest(MerchRequest merchRequest, CancellationToken cancellationToken = default);
    }
}