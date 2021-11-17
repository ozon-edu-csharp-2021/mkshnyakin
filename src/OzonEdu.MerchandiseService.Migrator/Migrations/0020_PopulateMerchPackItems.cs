using FluentMigrator;

namespace OzonEdu.MerchandiseService.Migrator.Migrations
{
    [Migration(20)]
    public class PopulateMerchPackItems : ForwardOnlyMigration
    {
        public override void Up()
        {
            Insert.IntoTable(TableNames.MerchPackItems)
                .Row(new
                {
                    name = "Ручка с логотипом Ozon",
                    sku = 1,
                })
                .Row(new
                {
                    name = "Блокнот с логотипом Ozon",
                    sku = 2,
                })
                .Row(new
                {
                    name = "Футболка синяя",
                    sku = 3,
                })
                .Row(new
                {
                    name = "Футболка с логотипом Ozon",
                    sku = 4,
                })
                .Row(new
                {
                    name = "Носки с логотипом Ozon",
                    sku = 5,
                })
                .Row(new
                {
                    name = "Рюкзак для ноутбука",
                    sku = 6,
                })
                .Row(new
                {
                    name = "Толстовка с логотипом Ozon",
                    sku = 7,
                })
                ;
        }
    }
}