using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CSharpCourse.Core.Lib.Enums;
using OzonEdu.MerchandiseService.Models;

namespace OzonEdu.MerchandiseService.Services
{
    public class MerchandiseService : IMerchandiseService
    {
        private static readonly Dictionary<MerchType, IEnumerable<MerchItem>> MerchPackStubs =
            new()
            {
                [MerchType.WelcomePack] = new MerchItem[]
                {
                    new()
                    {
                        Name = "Рюкзак",
                        SkuId = 1,
                    },
                    new()
                    {
                        Name = "Кепка",
                        SkuId = 2,
                    },
                    new()
                    {
                        Name = "Футболка",
                        SkuId = 3,
                    },
                },
                [MerchType.ProbationPeriodEndingPack] = new MerchItem[]
                {
                    new()
                    {
                        Name = "Ручка",
                        SkuId = 4,
                    },
                    new()
                    {
                        Name = "Кружка",
                        SkuId = 5,
                    },
                    new()
                    {
                        Name = "Флешка",
                        SkuId = 6,
                    },
                }
            };

        private static readonly Dictionary<int, IEnumerable<MerchHistoryItem>> HistoryStubs =
            new()
            {
                [1] = MerchPackStubs[MerchType.WelcomePack].Select(x => new MerchHistoryItem
                {
                    Item = x,
                    Date = DateTime.Today
                }),
                [2] = MerchPackStubs[MerchType.ProbationPeriodEndingPack].Select(x => new MerchHistoryItem
                {
                    Item = x,
                    Date = DateTime.Today
                })
            };

        public Task<IEnumerable<MerchHistoryItem>> GetHistoryForEmployee(int employeeId, CancellationToken token)
        {
            HistoryStubs.TryGetValue(employeeId, out var history);
            return Task.FromResult(history);
        }

        public Task<IEnumerable<MerchItem>> RequestMerchForEmployee(int employeeId, CancellationToken token)
        {
            var items = MerchPackStubs[MerchType.WelcomePack];
            var historyItems = MerchPackStubs[MerchType.WelcomePack]
                .Select(x => new MerchHistoryItem
                {
                    Item = x,
                    Date = DateTime.Now
                });

            var result = HistoryStubs.TryAdd(employeeId, historyItems)
                ? items
                : null;
            
            return Task.FromResult(result);
        }
    }
}