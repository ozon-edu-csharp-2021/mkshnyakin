using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OzonEdu.MerchandiseService.Infrastructure.Contracts;

namespace OzonEdu.MerchandiseService.Infrastructure.Stubs
{
    public class OzonEduStockApiClient : IOzonEduStockApiClient
    {
        public Task<int> GetAvailableQuantityAsync(long sku, CancellationToken cancellationToken = default)
        {
            var quantity = sku % 3 == 0 ? 0 : 100;
            return Task.FromResult(quantity);
        }

        public Task<bool> IsAvailable(IEnumerable<long> skus, CancellationToken cancellationToken = default)
        {
            var result = !skus.Any(x => x is >= 4 and <= 5); // ProbationPeriodEndingPack всегда отсутствует
            return Task.FromResult(result);
        }

        public Task<bool> Reserve(IEnumerable<long> skus, CancellationToken cancellationToken = default)
        {
            var result = !skus.Any(x => x is >= 5 and <= 6); // VeteranPack всегда не резервируется
            return Task.FromResult(result);
        }
    }
}