using System.Threading.Tasks;

namespace OzonEdu.MerchandiseService.Infrastructure.Contracts
{
    public interface IOzonEduStockApiClient
    {
        Task<int> GetAvailableQuantityAsync(long sku);
    }
}