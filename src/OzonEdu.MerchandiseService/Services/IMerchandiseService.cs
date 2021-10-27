using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OzonEdu.MerchandiseService.Models;

namespace OzonEdu.MerchandiseService.Services
{
    public interface IMerchandiseService
    {
        Task<IEnumerable<MerchHistoryItem>> GetHistoryForEmployee(int employeeId, CancellationToken token);
        Task<IEnumerable<MerchItem>> RequestMerchForEmployee(int employeeId, CancellationToken token);
    }
}