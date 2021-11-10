using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OzonEdu.MerchandiseService.Domain.AggregationModels.MerchPackItemAggregate;
using OzonEdu.MerchandiseService.Domain.AggregationModels.MerchRequestAggregate;
using OzonEdu.MerchandiseService.Infrastructure.Commands.MerchRequestAggregate;
using OzonEdu.MerchandiseService.Infrastructure.Exceptions;

namespace OzonEdu.MerchandiseService.Infrastructure.Handlers.MerchRequestAggregate
{
    public class GetMerchRequestHistoryForEmployeeIdCommandCommandHandler
        : IRequestHandler<GetMerchRequestHistoryForEmployeeIdCommand, IEnumerable<MerchRequestHistoryItem>>
    {
        private readonly IMerchPackItemRepository _merchPackItemRepository;
        private readonly IMerchRequestRepository _merchRequestRepository;

        public GetMerchRequestHistoryForEmployeeIdCommandCommandHandler(
            IMerchRequestRepository merchRequestRepository,
            IMerchPackItemRepository merchPackItemRepository)
        {
            _merchRequestRepository = merchRequestRepository;
            _merchPackItemRepository = merchPackItemRepository;
        }

        public async Task<IEnumerable<MerchRequestHistoryItem>> Handle(
            GetMerchRequestHistoryForEmployeeIdCommand request,
            CancellationToken cancellationToken)
        {
            var result = new List<MerchRequestHistoryItem>();
            var employeeId = request.EmployeeId;
            IEnumerable<MerchRequest> history;

            try
            {
                history = await _merchRequestRepository.FindCompletedByEmployeeIdAsync(employeeId, cancellationToken);
            }
            catch (Exception e)
            {
                throw new ItemNotFoundException(
                    $"MerchRequestHistoryItems are not found for employeeId: {employeeId}",
                    e);
            }

            foreach (var merchRequest in history)
            {
                IEnumerable<MerchPackItem> merchPackItems;
                try
                {
                    merchPackItems = await _merchPackItemRepository.FindByMerchTypeAsync(
                        merchRequest.MerchType,
                        cancellationToken);
                }
                catch (Exception e)
                {
                    throw new ItemNotFoundException(
                        $"MerchPackItems are not found for MerchType: {merchRequest.MerchType}",
                        e);
                }

                var merchRequestHistoryItems = merchPackItems.Select(x => new MerchRequestHistoryItem
                {
                    Item = x,
                    GiveOutDate = merchRequest.GiveOutDate
                });

                result.AddRange(merchRequestHistoryItems);
            }

            return result;
        }
    }
}