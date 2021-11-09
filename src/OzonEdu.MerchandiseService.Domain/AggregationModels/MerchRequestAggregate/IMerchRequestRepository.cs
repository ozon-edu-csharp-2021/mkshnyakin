using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OzonEdu.MerchandiseService.Domain.Contracts;

namespace OzonEdu.MerchandiseService.Domain.AggregationModels.MerchRequestAggregate
{
    public interface IMerchRequestRepository : IRepository<MerchRequest>
    {
        public Task<IEnumerable<MerchRequest>> FindByRequestMerchTypeAsync(
            RequestMerchType requestMerchType,
            CancellationToken cancellationToken = default);
    }
}