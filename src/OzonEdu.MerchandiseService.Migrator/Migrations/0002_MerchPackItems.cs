using FluentMigrator;

namespace OzonEdu.MerchandiseService.Migrator.Migrations
{
    [Migration(2)]
    public class MerchPackItems : Migration
    {
        public override void Up()
        {
            Create.Table(TableNames.MerchPackItems)
                .WithColumn("id").AsInt64().Identity().PrimaryKey()
                .WithColumn("name").AsString().NotNullable()
                .WithColumn("sku").AsInt64().NotNullable()
                ;
        }

        public override void Down()
        {
            Delete.Table(TableNames.MerchPackItems);
        }
    }
}