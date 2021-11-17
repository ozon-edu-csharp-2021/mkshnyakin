using FluentMigrator;

namespace OzonEdu.MerchandiseService.Migrator.Migrations
{
    [Migration(1)]
    public class MerchRequests : Migration
    {
        public override void Up()
        {
            Create.Table(TableNames.MerchRequests)
                .WithColumn("id").AsInt64().Identity().PrimaryKey()
                .WithColumn("employee_id").AsInt64().NotNullable()
                .WithColumn("merch_type").AsInt32().NotNullable()
                .WithColumn("status").AsInt32().NotNullable()
                .WithColumn("mode").AsInt32().NotNullable()
                .WithColumn("give_out_date").AsDateTime().Nullable()
                ;
        }

        public override void Down()
        {
            Delete.Table(TableNames.MerchRequests);
        }
    }
}