using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Microsoft.Extensions.Options;
using OzonEdu.MerchandiseService.Infrastructure.Configuration;
using OzonEdu.MerchandiseService.Infrastructure.Contracts;
using OzonEdu.StockApi.Grpc;

namespace OzonEdu.MerchandiseService.Infrastructure.Clients.Implementation
{
    public class OzonEduStockApiGrpcClient : IOzonEduStockApiClient, IDisposable
    {
        private readonly GrpcChannel _channel;
        private readonly StockApiGrpc.StockApiGrpcClient _client;
        private readonly OzonEduStockApiGrpcOptions _options;

        public OzonEduStockApiGrpcClient(IOptions<OzonEduStockApiGrpcOptions> options)
        {
            _options = options.Value;
            _channel = GrpcChannel.ForAddress(_options.Address);
            _client = new StockApiGrpc.StockApiGrpcClient(_channel);
        }

        public void Dispose()
        {
            _channel?.Dispose();
        }

        public async Task<bool> IsAvailable(IEnumerable<long> skus, CancellationToken cancellationToken = default)
        {
            if (skus is null)
            {
                return false;
            }

            var isSkuAvailable = skus.ToDictionary(k => k, _ => false);
            var response = await _client.GetStockItemsAvailabilityAsync(
                new SkusRequest {Skus = {skus}},
                cancellationToken: cancellationToken);

            foreach (var item in response.Items)
            {
                if (isSkuAvailable.ContainsKey(item.Sku) && item.Quantity > 0)
                {
                    isSkuAvailable[item.Sku] = true;
                }
            }

            var isAvailable = isSkuAvailable.Values.All(x => x);
            return isAvailable;
        }

        public async Task<bool> Reserve(IEnumerable<long> skus, CancellationToken cancellationToken = default)
        {
            if (skus is null)
            {
                return false;
            }

            var response = await _client.GiveOutItemsAsync(
                new GiveOutItemsRequest
                {
                    Items = {skus.Select(x => new SkuQuantityItem {Sku = x, Quantity = 1})}
                },
                cancellationToken: cancellationToken);

            var isReserved = response.Result.Equals(GiveOutItemsResponse.Types.Result.Successful);
            return isReserved;
        }
    }
}