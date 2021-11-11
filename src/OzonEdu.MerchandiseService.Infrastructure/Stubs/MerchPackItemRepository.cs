using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CSharpCourse.Core.Lib.Enums;
using OzonEdu.MerchandiseService.Domain.AggregationModels.MerchPackItemAggregate;
using OzonEdu.MerchandiseService.Domain.AggregationModels.MerchRequestAggregate;
using OzonEdu.MerchandiseService.Infrastructure.Extensions;

namespace OzonEdu.MerchandiseService.Infrastructure.Stubs
{
    public class MerchPackItemRepository : IMerchPackItemRepository
    {
        private static readonly IdentityGenerator IdGen = new();

        private static readonly MerchPackItem Pen = new(IdGen.Get(), ItemName.Create("Ручка с логотипом Ozon"),
            Sku.Create(1));

        private static readonly MerchPackItem Notepad = new(IdGen.Get(), ItemName.Create("Блокнот с логотипом Ozon"),
            Sku.Create(2));

        private static readonly ImmutableDictionary<MerchType, ImmutableArray<MerchPackItem>> MerchPackStubs =
            new Dictionary<MerchType, ImmutableArray<MerchPackItem>>
                {
                    [MerchType.WelcomePack] = ImmutableArray.Create(new MerchPackItem[]
                    {
                        new(IdGen.Get(), ItemName.Create("Футболка синяя"), Sku.Create(3))
                    }),
                    [MerchType.ConferenceListenerPack] = ImmutableArray.Create(new[]
                    {
                        Pen,
                        Notepad
                    }),
                    [MerchType.ConferenceSpeakerPack] = ImmutableArray.Create(new[]
                    {
                        Pen,
                        Notepad
                    }),
                    [MerchType.ProbationPeriodEndingPack] = ImmutableArray.Create(new MerchPackItem[]
                    {
                        new(IdGen.Get(), ItemName.Create("Футболка с логотипом Ozon"), Sku.Create(4)),
                        new(IdGen.Get(), ItemName.Create("Носки с логотипом Ozon "), Sku.Create(5))
                    }),
                    [MerchType.VeteranPack] = ImmutableArray.Create(new MerchPackItem[]
                    {
                        new(IdGen.Get(), ItemName.Create("Рюкзак для ноутбука"), Sku.Create(6)),
                        new(IdGen.Get(), ItemName.Create("Толстовка с логотипом Ozon"), Sku.Create(7))
                    })
                }
                .ToImmutableDictionary();

        public Task<MerchPackItem> CreateAsync(MerchPackItem merchRequest,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<MerchPackItem> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<MerchPackItem> UpdateAsync(MerchPackItem itemToUpdate,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<MerchPackItem> DeleteAsync(MerchPackItem itemToUpdate,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<MerchPackItem>> FindByMerchTypeAsync(
            RequestMerchType domainRequestMerchType,
            CancellationToken cancellationToken = default)
        {
            var merchType = (MerchType) domainRequestMerchType.Id;
            MerchPackStubs.TryGetValue(merchType, out var value);
            IReadOnlyList<MerchPackItem> items = value;
            return Task.FromResult(items);
        }

        public Task<IReadOnlyList<MerchPackItem>> FindByMerchTypeAsync(
            MerchType merchType,
            CancellationToken cancellationToken = default)
        {
            MerchPackStubs.TryGetValue(merchType, out var value);
            IReadOnlyList<MerchPackItem> items = value;
            return Task.FromResult(items);
        }

        public Task<IEnumerable<RequestMerchType>> FindMerchTypesBySkuAsync(
            IEnumerable<long> skuIds,
            CancellationToken cancellationToken = default)
        {
            var merchTypes = new Dictionary<MerchType, bool>();
            foreach (var skuId in skuIds)
            {
                foreach (var kvp in MerchPackStubs)
                {
                    var merchType = kvp.Key;
                    if (kvp.Value.Any(x => x.Sku.Id == skuId))
                    {
                        merchTypes[merchType] = true;
                    }
                }
            }

            var result = merchTypes.Select(x => x.Key.ToRequestMerchType());
            return Task.FromResult(result);
        }
    }
}