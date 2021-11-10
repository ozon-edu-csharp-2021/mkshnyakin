using System.Collections.Generic;
using MediatR;

namespace OzonEdu.MerchandiseService.Infrastructure.Commands.MerchRequestAggregate
{
    public class GetMerchRequestHistoryForEmployeeIdCommand : IRequest<IEnumerable<MerchRequestHistoryItem>>
    {
        public long EmployeeId { get; init; }
    }
}