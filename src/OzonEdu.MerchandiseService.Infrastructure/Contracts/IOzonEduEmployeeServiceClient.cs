using System.Threading;
using System.Threading.Tasks;
using OzonEdu.MerchandiseService.Infrastructure.Stubs;

namespace OzonEdu.MerchandiseService.Infrastructure.Contracts
{
    public interface IOzonEduEmployeeServiceClient
    {
        Task<OzonEduEmployeeServiceClient.EmployeeViewModel> GetByIdAsync(
            long employeeId,
            CancellationToken cancellationToken = default);

        Task<OzonEduEmployeeServiceClient.EmployeeViewModel> FindByEmailAsync(
            string employeeEmail,
            CancellationToken cancellationToken = default);
    }
}