using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OzonEdu.MerchandiseService.Infrastructure.Contracts
{
    public interface IOzonEduStockApiClient
    {
        Task<bool> IsAvailable(IEnumerable<long> skus, CancellationToken cancellationToken = default);
        Task<bool> Reserve(IEnumerable<long> skus, CancellationToken cancellationToken = default);
    }
}