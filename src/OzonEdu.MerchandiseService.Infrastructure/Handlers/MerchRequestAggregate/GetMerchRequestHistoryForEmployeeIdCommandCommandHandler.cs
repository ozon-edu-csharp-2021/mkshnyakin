using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LazyCache;
using MediatR;
using OzonEdu.MerchandiseService.Domain.AggregationModels.MerchRequestAggregate;
using OzonEdu.MerchandiseService.Infrastructure.ApplicationServices;
using OzonEdu.MerchandiseService.Infrastructure.Cache;
using OzonEdu.MerchandiseService.Infrastructure.Commands.MerchRequestAggregate;
using OzonEdu.MerchandiseService.Infrastructure.Exceptions;

namespace OzonEdu.MerchandiseService.Infrastructure.Handlers.MerchRequestAggregate
{
    public class GetMerchRequestHistoryForEmployeeIdCommandCommandHandler
        : IRequestHandler<GetMerchRequestHistoryForEmployeeIdCommand, IEnumerable<MerchRequestHistoryItem>>
    {
        private readonly IMerchRequestRepository _merchRequestRepository;
        private readonly IApplicationService _applicationService;
        private readonly IAppCache _cache;
        private readonly CacheKeysProvider _cacheKeys;

        public GetMerchRequestHistoryForEmployeeIdCommandCommandHandler(
            IMerchRequestRepository merchRequestRepository,
            IApplicationService applicationService,
            IAppCache cache,
            CacheKeysProvider cacheKeys)
        {
            _merchRequestRepository = merchRequestRepository;
            _applicationService = applicationService;
            _cache = cache;
            _cacheKeys = cacheKeys;
        }

        public async Task<IEnumerable<MerchRequestHistoryItem>> Handle(
            GetMerchRequestHistoryForEmployeeIdCommand request,
            CancellationToken cancellationToken)
        {
            async Task<IEnumerable<MerchRequestHistoryItem>> PopulateCache()
            {
                var result = new List<MerchRequestHistoryItem>();
                var employeeId = request.EmployeeId;
                IEnumerable<MerchRequest> history;

                try
                {
                    history = await _merchRequestRepository.FindCompletedByEmployeeIdAsync(
                        employeeId,
                        cancellationToken);
                }
                catch (Exception e)
                {
                    throw new ItemNotFoundException(
                        $"MerchRequestHistoryItems are not found for employeeId({employeeId}) because service crashed",
                        e);
                }

                var maxDegreeOfParallelism = 1;
                var semaphore = new SemaphoreSlim(maxDegreeOfParallelism);
                var tasks = history.Select(async merchRequest =>
                    {
                        await semaphore.WaitAsync(cancellationToken);
                        try
                        {
                            var merchPackItems =
                                await _applicationService.GetMerchPackItemsByType(
                                    merchRequest.MerchType,
                                    cancellationToken);
                            var merchRequestHistoryItems = merchPackItems.Select(x => new MerchRequestHistoryItem
                            {
                                Item = x,
                                GiveOutDate = merchRequest.GiveOutDate
                            });
                            return merchRequestHistoryItems;
                        }
                        finally
                        {
                            semaphore.Release();
                        }
                    }
                );
                var allResults = await Task.WhenAll(tasks);
                
                result = allResults
                    .SelectMany(x => x)
                    .OrderBy(x => x.GiveOutDate.Value)
                    .ToList();
                
                return result;
            }

            var key = _cacheKeys.GetMerchRequestHistoryKey(request.EmployeeId);
            return await _cache.GetOrAddAsync(key, PopulateCache, DateTimeOffset.Now.AddSeconds(60));
        }
    }
}