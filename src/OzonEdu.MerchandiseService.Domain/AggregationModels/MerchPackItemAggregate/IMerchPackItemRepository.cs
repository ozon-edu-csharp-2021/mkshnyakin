using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OzonEdu.MerchandiseService.Domain.AggregationModels.MerchRequestAggregate;
using OzonEdu.MerchandiseService.Domain.Contracts;

namespace OzonEdu.MerchandiseService.Domain.AggregationModels.MerchPackItemAggregate
{
    public interface IMerchPackItemRepository : IRepository<MerchPackItem>
    {
        Task<IReadOnlyCollection<MerchPackItem>> FindByMerchTypeAsync(
            RequestMerchType requestMerchType,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyCollection<RequestMerchType>> FindMerchTypesBySkuAsync(
            IEnumerable<long> skuIds,
            CancellationToken cancellationToken = default);

        Task AddToPackAsync(
            RequestMerchType requestMerchType,
            IEnumerable<MerchPackItem> merchPackItems,
            CancellationToken cancellationToken = default);
    }
}