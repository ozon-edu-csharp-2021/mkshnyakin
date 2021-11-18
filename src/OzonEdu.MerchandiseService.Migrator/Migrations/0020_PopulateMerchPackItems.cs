using FluentMigrator;

namespace OzonEdu.MerchandiseService.Migrator.Migrations
{
    [Migration(20)]
    public class PopulateMerchPackItems : ForwardOnlyMigration
    {
        public override void Up()
        {
            Execute.Sql(@"
                insert into merch_pack_items (name, sku)
                values  ('Ручка с логотипом Ozon', 1),
                        ('Блокнот с логотипом Ozon', 2),
                        ('Футболка синяя', 3),
                        ('Футболка с логотипом Ozon', 4),
                        ('Носки с логотипом Ozon', 5),
                        ('Рюкзак для ноутбука', 6),
                        ('Толстовка с логотипом Ozon', 7)
                on conflict do nothing;"
            );
        }
    }
}