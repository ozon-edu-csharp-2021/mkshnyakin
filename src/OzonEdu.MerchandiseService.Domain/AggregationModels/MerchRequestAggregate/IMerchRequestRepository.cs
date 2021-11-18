using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OzonEdu.MerchandiseService.Domain.Contracts;

namespace OzonEdu.MerchandiseService.Domain.AggregationModels.MerchRequestAggregate
{
    public interface IMerchRequestRepository : IRepository<MerchRequest>
    {
        Task<IReadOnlyCollection<MerchRequest>> FindByEmployeeIdAsync(
            long employeeId,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyCollection<MerchRequest>> FindByRequestMerchTypeAsync(
            RequestMerchType requestMerchType,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyCollection<MerchRequest>> FindCompletedByEmployeeIdAsync(
            long employeeId,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyCollection<MerchRequest>> FindOutOfStockByRequestMerchTypesAsync(
            IEnumerable<RequestMerchType> requestMerchTypes,
            CancellationToken cancellationToken = default);
    }
}