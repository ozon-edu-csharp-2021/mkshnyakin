using FluentMigrator;

namespace OzonEdu.MerchandiseService.Migrator.Migrations
{
    [Migration(30)]
    public class PopulateMerchTypeToItemsRelations : ForwardOnlyMigration
    {
        public override void Up()
        {
            Insert.IntoTable(TableNames.MerchTypeToItemsRelations)
                .Row(new
                {
                    merch_type = 10,
                    merch_pack_item_id = 3,
                })
                .Row(new
                {
                    merch_type = 20,
                    merch_pack_item_id = 1,
                })
                .Row(new
                {
                    merch_type = 20,
                    merch_pack_item_id = 2,
                })
                .Row(new
                {
                    merch_type = 30,
                    merch_pack_item_id = 1,
                })
                .Row(new
                {
                    merch_type = 30,
                    merch_pack_item_id = 2,
                })
                .Row(new
                {
                    merch_type = 40,
                    merch_pack_item_id = 4,
                })
                .Row(new
                {
                    merch_type = 40,
                    merch_pack_item_id = 5,
                })
                .Row(new
                {
                    merch_type = 50,
                    merch_pack_item_id = 6,
                })
                .Row(new
                {
                    merch_type = 50,
                    merch_pack_item_id = 7,
                })
                ;
        }
    }
}