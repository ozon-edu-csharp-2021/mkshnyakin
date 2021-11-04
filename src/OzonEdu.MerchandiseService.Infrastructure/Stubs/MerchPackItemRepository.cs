using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using CSharpCourse.Core.Lib.Enums;
using OzonEdu.MerchandiseService.Domain.AggregationModels.MerchPackItemAggregate;

namespace OzonEdu.MerchandiseService.Infrastructure.Stubs
{
    public class MerchPackItemRepository : IMerchPackItemRepository
    {
        private static readonly MerchPackItem Pen = new(new ItemName("Ручка с логотипом Ozon"), new Sku(2));

        private static readonly MerchPackItem Notepad = new(new ItemName("Блокнот с логотипом Ozon"), new Sku(3));

        private static readonly ImmutableDictionary<MerchType, ImmutableArray<MerchPackItem>> MerchPackStubs =
            new Dictionary<MerchType, ImmutableArray<MerchPackItem>>
                {
                    [MerchType.WelcomePack] = ImmutableArray.Create(new MerchPackItem[]
                    {
                        new(new ItemName("Футболка синяя"), new Sku(1)),
                    }),
                    [MerchType.ConferenceListenerPack] = ImmutableArray.Create(new MerchPackItem[]
                    {
                        Pen,
                        Notepad
                    }),
                    [MerchType.ConferenceSpeakerPack] = ImmutableArray.Create(new MerchPackItem[]
                    {
                        Pen,
                        Notepad
                    }),
                    [MerchType.ProbationPeriodEndingPack] = ImmutableArray.Create(new MerchPackItem[]
                    {
                        new(new ItemName("Футболка с логотипом Ozon"), new Sku(4)),
                        new(new ItemName("Носки с логотипом Ozon "), new Sku(5)),
                    }),
                    [MerchType.VeteranPack] = ImmutableArray.Create(new MerchPackItem[]
                    {
                        new(new ItemName("Рюкзак для ноутбука"), new Sku(6)),
                        new(new ItemName("Толстовка с логотипом Ozon"), new Sku(7)),
                    })
                }
                .ToImmutableDictionary();

        public Task<MerchPackItem> CreateAsync(MerchPackItem itemToCreate,
            CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task<MerchPackItem> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task<MerchPackItem> UpdateAsync(MerchPackItem itemToUpdate,
            CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task<MerchPackItem> DeleteAsync(MerchPackItem itemToUpdate,
            CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task<IReadOnlyList<MerchPackItem>> FindByMerchTypeAsync(MerchType merchType,
            CancellationToken cancellationToken = default)
        {
            MerchPackStubs.TryGetValue(merchType, out var value);
            IReadOnlyList<MerchPackItem> items = value;
            return Task.FromResult(items);
        }
    }
}