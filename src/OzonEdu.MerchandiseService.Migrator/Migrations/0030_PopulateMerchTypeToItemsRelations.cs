using FluentMigrator;

namespace OzonEdu.MerchandiseService.Migrator.Migrations
{
    [Migration(30)]
    public class PopulateMerchTypeToItemsRelations : ForwardOnlyMigration
    {
        public override void Up()
        {
            Execute.Sql(@"
                insert into merch_type_to_items_relations (merch_type, merch_pack_item_id)
                values  (10, 3),
                        (20, 1),
                        (20, 2),
                        (30, 1),
                        (30, 2),
                        (40, 4),
                        (40, 5),
                        (50, 6),
                        (50, 7)
                on conflict do nothing;"
            );
        }
    }
}