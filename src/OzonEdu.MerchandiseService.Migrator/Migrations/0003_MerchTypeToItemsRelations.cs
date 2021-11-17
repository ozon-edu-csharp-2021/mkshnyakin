using FluentMigrator;

namespace OzonEdu.MerchandiseService.Migrator.Migrations
{
    [Migration(3)]
    public class MerchTypeToItemsRelations : Migration 
    {
        public override void Up()
        {
            Create.Table(TableNames.MerchTypeToItemsRelations)
                .WithColumn("id").AsInt64().Identity().PrimaryKey()
                .WithColumn("merch_type").AsInt32().NotNullable()
                .WithColumn("merch_pack_item_id").AsInt64().NotNullable()
                ;
        }

        public override void Down()
        {
            Delete.Table(TableNames.MerchTypeToItemsRelations);
        }
    }
}