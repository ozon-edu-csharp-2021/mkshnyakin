using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OzonEdu.MerchandiseService.HttpModels;

namespace OzonEdu.MerchandiseService.HttpClient.Test
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            using var cts = new CancellationTokenSource();
            var http = new System.Net.Http.HttpClient
            {
                BaseAddress = new Uri("http://localhost:5000/")
            };
            var merchHttpClient = new MerchandiseServiceHttpClient(http);

            {
                var response = await merchHttpClient.V1GetHistoryForEmployee(33, cts.Token);
                var items = response?.Items ?? Enumerable.Empty<EmployeeMerchGetResponseItem>();
                foreach (var item in items)
                {
                    Console.WriteLine($"Name: {item.Item.Name}. Sku: {item.Item.SkuId}. Date: {item.Date}.");
                }
            }

            {
                var response = await merchHttpClient.V1RequestMerchForEmployee(5, cts.Token);
                var items = response?.Items ?? Enumerable.Empty<EmployeeMerchItem>();
                foreach (var item in items)
                {
                    Console.WriteLine($"Name: {item.Name}. Sku: {item.SkuId}.");
                }
            }

            {
                var response = await merchHttpClient.V1GetHistoryForEmployee(2, cts.Token);
                var items = response?.Items ?? Enumerable.Empty<EmployeeMerchGetResponseItem>();
                foreach (var item in items)
                {
                    Console.WriteLine($"Name: {item.Item.Name}. Sku: {item.Item.SkuId}. Date: {item.Date}.");
                }
            }
        }
    }
}