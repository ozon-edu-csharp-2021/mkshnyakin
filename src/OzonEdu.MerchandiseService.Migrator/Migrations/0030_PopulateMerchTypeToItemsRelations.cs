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
                values
                    (10, 1),
                    (10, 2),
                    (10, 3),
                    (10, 4),
                       
                    (20, 5),
                    (20, 6),
                    (20, 7),
                    
                    (30, 8),
                    (30, 9),
                    (30, 10),
                    
                    (40, 11),
                    (40, 12),
                    
                    (50, 13),
                    (50, 14),
                    (50, 15),
                    (50, 16),
                    (50, 17)
                on conflict do nothing;"
            );
        }
    }
}