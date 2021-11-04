using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CSharpCourse.Core.Lib.Enums;
using OzonEdu.MerchandiseService.Domain.Contracts;

namespace OzonEdu.MerchandiseService.Domain.AggregationModels.MerchPackItemAggregate
{
    public interface IMerchPackItemRepository : IRepository<MerchPackItem>
    {
        Task<IReadOnlyList<MerchPackItem>> FindByMerchTypeAsync(MerchType merchType,
            CancellationToken cancellationToken = default);
    }
}