using System.Threading.Tasks;
using OzonEdu.MerchandiseService.Infrastructure.Contracts;

namespace OzonEdu.MerchandiseService.Infrastructure.Stubs
{
    public class OzonEduStockApiClient : IOzonEduStockApiClient
    {
        public Task<int> GetAvailableQuantityAsync(long sku)
        {
            var quantity = sku % 3 == 0 ? 0 : 100;
            return Task.FromResult(quantity);
        }
    }
}