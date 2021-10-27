using System.Threading;
using System.Threading.Tasks;
using OzonEdu.MerchandiseService.HttpModels;

namespace OzonEdu.MerchandiseService.HttpClient
{
    public interface IMerchandiseServiceHttpClient
    {
        Task<EmployeeMerchGetResponse> V1GetHistoryForEmployee(int employeeId, CancellationToken token);
        Task<EmployeeMerchPostResponse> V1RequestMerchForEmployee(int employeeId, CancellationToken token);
    }
}