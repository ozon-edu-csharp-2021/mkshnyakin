using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using OpenTracing;
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
        private readonly SemaphoreSlim _semaphore = new(1);
        private readonly IMerchRequestRepository _merchRequestRepository;
        private readonly IApplicationService _applicationService;
        private readonly CacheKeysProvider _cacheKeys;
        private readonly IDistributedCache _distributedCache;
        private readonly JsonSerializerOptions _serializerOptions = new()
        {
            IncludeFields = true
        };
        private readonly ITracer _tracer;

        public GetMerchRequestHistoryForEmployeeIdCommandCommandHandler(
            IMerchRequestRepository merchRequestRepository,
            IApplicationService applicationService,
            CacheKeysProvider cacheKeys,
            IDistributedCache distributedCache,
            ITracer tracer)
        {
            _merchRequestRepository = merchRequestRepository;
            _applicationService = applicationService;
            _cacheKeys = cacheKeys;
            _distributedCache = distributedCache;
            _tracer = tracer;
        }

        public async Task<IEnumerable<MerchRequestHistoryItem>> Handle(
            GetMerchRequestHistoryForEmployeeIdCommand request,
            CancellationToken cancellationToken)
        {
            using var span = _tracer
                .BuildSpan(nameof(GetMerchRequestHistoryForEmployeeIdCommandCommandHandler))
                .StartActive();

            var key = _cacheKeys.GetMerchRequestHistoryKey(request.EmployeeId);

            {
                var cacheValue = await _distributedCache.GetStringAsync(key, cancellationToken);
                if (!string.IsNullOrEmpty(cacheValue))
                {
                    span.Span.SetTag("cached", true);
                    return JsonSerializer.Deserialize<List<MerchRequestHistoryItem>>(cacheValue, _serializerOptions);
                }
            }

            await _semaphore.WaitAsync(cancellationToken);
            try
            {
                // double check locking
                var cacheValue = await _distributedCache.GetStringAsync(key, cancellationToken);
                if (!string.IsNullOrEmpty(cacheValue))
                {
                    span.Span.SetTag("cached", true);
                    return JsonSerializer.Deserialize<List<MerchRequestHistoryItem>>(cacheValue, _serializerOptions);
                }

                span.Span.SetTag("cached", false);
                var result = await GetHistoryForEmployee(request.EmployeeId, cancellationToken);
                var value = JsonSerializer.Serialize(result, _serializerOptions);
                var options = new DistributedCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromMinutes(1)
                };
                await _distributedCache.SetStringAsync(key, value, options, cancellationToken);

                return result;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<List<MerchRequestHistoryItem>> GetHistoryForEmployee(
            long employeeId,
            CancellationToken cancellationToken = default)
        {
            using var span = _tracer.BuildSpan(nameof(GetHistoryForEmployee)).StartActive();
            
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

            const int maxDegreeOfParallelism = 1;
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
                            Item = new MerchPackHistoryItem
                            {
                                Name = x.ItemName.Value,
                                Sku = x.Sku.Id
                            },
                            GiveOutDate = merchRequest.GiveOutDate.Value
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

            var result = allResults
                .SelectMany(x => x)
                .OrderBy(x => x.GiveOutDate)
                .ToList();

            return result;
        }
    }
}