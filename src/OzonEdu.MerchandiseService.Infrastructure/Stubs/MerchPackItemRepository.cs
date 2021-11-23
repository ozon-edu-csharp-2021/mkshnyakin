using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OzonEdu.MerchandiseService.Domain.AggregationModels.MerchPackItemAggregate;
using OzonEdu.MerchandiseService.Domain.AggregationModels.MerchRequestAggregate;

namespace OzonEdu.MerchandiseService.Infrastructure.Stubs
{
    public class MerchPackItemRepository : IMerchPackItemRepository
    {
        public sealed class MerchTypeToItemsRelation
        {
            public RequestMerchType RequestMerchType { get; init; }
            public long MerchPackItemId { get; init; }
            public override string ToString() => $"{RequestMerchType}: {MerchPackItemId}";
        }

        private static readonly IdentityGenerator ItemsIdGen = new();
        private static readonly IdentityGenerator RelationsIdGen = new();
        private static readonly ConcurrentDictionary<long, MerchPackItem> MerchPackItems = new();
        private static readonly ConcurrentDictionary<long, MerchTypeToItemsRelation> MerchTypeToItemsRelations = new();

        public MerchPackItemRepository()
        {
            var pen = Create(new MerchPackItem(ItemName.Create("Ручка с логотипом Ozon"), Sku.Create(1)));
            var notepad = Create(new MerchPackItem(ItemName.Create("Блокнот с логотипом Ozon"), Sku.Create(2)));
            var blueTshirt = Create(new MerchPackItem(ItemName.Create("Футболка синяя"), Sku.Create(3)));
            var ozonTshirt = Create(new MerchPackItem(ItemName.Create("Футболка с логотипом Ozon"), Sku.Create(4)));
            var socks = Create(new MerchPackItem(ItemName.Create("Носки с логотипом Ozon"), Sku.Create(5)));
            var backpack = Create(new MerchPackItem(ItemName.Create("Рюкзак для ноутбука"), Sku.Create(6)));
            var sweatshirt = Create(new MerchPackItem(ItemName.Create("Толстовка с логотипом Ozon"), Sku.Create(7)));

            AddToPack(RequestMerchType.WelcomePack, new[] {blueTshirt});
            AddToPack(RequestMerchType.ConferenceListenerPack, new[] {pen, notepad});
            AddToPack(RequestMerchType.ConferenceSpeakerPack, new[] {pen, notepad});
            AddToPack(RequestMerchType.ProbationPeriodEndingPack, new[] {ozonTshirt, socks});
            AddToPack(RequestMerchType.VeteranPack, new[] {backpack, sweatshirt});
        }

        public Task<MerchPackItem> CreateAsync(
            MerchPackItem merchPackItem,
            CancellationToken cancellationToken = default)
        {
            var id = ItemsIdGen.Get();
            var newMerchRequest = new MerchPackItem(
                id,
                merchPackItem.ItemName,
                merchPackItem.Sku
            );
            var result = MerchPackItems.TryAdd(id, newMerchRequest)
                ? newMerchRequest
                : null;
            return Task.FromResult(result);
        }

        public Task<MerchPackItem> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            MerchPackItems.TryGetValue(id, out var merchPackItem);
            return Task.FromResult(merchPackItem);
        }

        public Task<MerchPackItem> UpdateAsync(
            MerchPackItem merchPackItem,
            CancellationToken cancellationToken = default)
        {
            MerchPackItems[merchPackItem.Id] = merchPackItem;
            return Task.FromResult(MerchPackItems[merchPackItem.Id]);
        }

        public Task<MerchPackItem> DeleteAsync(
            MerchPackItem itemToDelete,
            CancellationToken cancellationToken = default)
        {
            if (MerchPackItems.TryRemove(itemToDelete.Id, out var deletedItem))
            {
                var relationIds = MerchTypeToItemsRelations
                    .Where(x => x.Value.MerchPackItemId == deletedItem.Id)
                    .Select(x => x.Key);

                foreach (var relationId in relationIds)
                {
                    MerchTypeToItemsRelations.TryRemove(relationId, out _);
                }
            }

            return Task.FromResult(deletedItem);
        }

        public Task<IReadOnlyCollection<MerchPackItem>> FindByMerchTypeAsync(
            RequestMerchType requestMerchType,
            CancellationToken cancellationToken = default)
        {
            var merchPackItems = new List<MerchPackItem>();
            foreach (var merchTypeToItemsRelation in MerchTypeToItemsRelations.Values)
            {
                if (merchTypeToItemsRelation.RequestMerchType.Equals(requestMerchType))
                {
                    if (MerchPackItems.TryGetValue(merchTypeToItemsRelation.MerchPackItemId, out var merchPackItem))
                    {
                        merchPackItems.Add(merchPackItem);
                    }
                }
            }

            var result = merchPackItems.AsReadOnly() as IReadOnlyCollection<MerchPackItem>;
            return Task.FromResult(result);
        }

        public Task<IReadOnlyCollection<RequestMerchType>> FindMerchTypesBySkuAsync(
            IEnumerable<long> skuIds,
            CancellationToken cancellationToken = default)
        {
            var merchTypes = new Dictionary<RequestMerchType, bool>();

            var groupByRequestMerchType = new Dictionary<RequestMerchType, List<long>>();
            foreach (var merchTypeToItemsRelation in MerchTypeToItemsRelations.Values)
            {
                var merchType = merchTypeToItemsRelation.RequestMerchType;
                if (!groupByRequestMerchType.ContainsKey(merchType))
                    groupByRequestMerchType[merchType] = new List<long>();

                if (MerchPackItems.TryGetValue(merchTypeToItemsRelation.MerchPackItemId, out var merchPackItem))
                    groupByRequestMerchType[merchType].Add(merchPackItem.Sku.Id);
            }

            foreach (var skuId in skuIds)
            {
                foreach (var (merchType, skusInGroup) in groupByRequestMerchType)
                {
                    if (skusInGroup.Contains(skuId))
                    {
                        merchTypes[merchType] = true;
                    }
                }
            }

            var result = merchTypes.Keys
                    .ToList()
                    .AsReadOnly()
                as IReadOnlyCollection<RequestMerchType>;
            return Task.FromResult(result);
        }

        public Task<int> AddToPackAsync(
            RequestMerchType requestMerchType,
            IEnumerable<MerchPackItem> merchPackItems,
            CancellationToken cancellationToken = default)
        {
            var affectedRows = 0;
            foreach (var merchPackItem in merchPackItems)
            {
                var newRelation = new MerchTypeToItemsRelation
                {
                    RequestMerchType = requestMerchType,
                    MerchPackItemId = merchPackItem.Id
                };
                if (MerchTypeToItemsRelations.TryAdd(RelationsIdGen.Get(), newRelation))
                {
                    affectedRows++;
                }
            }

            return Task.FromResult(affectedRows);
        }


        private MerchPackItem Create(MerchPackItem merchPackItem, CancellationToken cancellationToken = default)
        {
            return CreateAsync(merchPackItem, cancellationToken).GetAwaiter().GetResult();
        }

        private void AddToPack(
            RequestMerchType requestMerchType,
            IEnumerable<MerchPackItem> merchPackItems,
            CancellationToken cancellationToken = default)
        {
            AddToPackAsync(requestMerchType, merchPackItems, cancellationToken).GetAwaiter().GetResult();
        }
    }
}