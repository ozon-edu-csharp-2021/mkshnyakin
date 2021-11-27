using System.Threading;
using System.Threading.Tasks;
using CSharpCourse.Core.Lib.Enums;
using OzonEdu.MerchandiseService.HttpModels;

namespace OzonEdu.MerchandiseService.HttpClient
{
    public interface IMerchandiseServiceHttpClient
    {
        Task<EmployeeMerchGetResponse> V1GetHistoryForEmployee(long employeeId, CancellationToken token);
        Task<EmployeeMerchPostResponse> V1RequestMerchForEmployee(
            long employeeId,
            MerchType merchType,
            CancellationToken token);
    }
}