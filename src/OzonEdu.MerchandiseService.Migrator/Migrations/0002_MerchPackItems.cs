using FluentMigrator;

namespace OzonEdu.MerchandiseService.Migrator.Migrations
{
    [Migration(2)]
    public class MerchPackItems : Migration
    {
        public override void Up()
        {
            Execute.Sql(@"
                create table if not exists merch_pack_items (
                    id      bigserial   primary key,
                    name    text        not null,
                    sku     bigint      not null                                
                );"
            );
        }

        public override void Down()
        {
            Execute.Sql("drop table if exists merch_pack_items;");
        }
    }
}