using FluentMigrator;

namespace OzonEdu.MerchandiseService.Migrator.Migrations
{
    [Migration(3)]
    public class MerchTypeToItemsRelations : Migration 
    {
        public override void Up()
        {
            Execute.Sql(@"
                create table if not exists merch_type_to_items_relations (
                    id                  bigserial   primary key,
                    merch_type          integer     not null,
                    merch_pack_item_id  bigint      not null,
                    constraint merch_type_to_items_unique
                        unique (merch_type, merch_pack_item_id)
                );"
            );
        }

        public override void Down()
        {
            Execute.Sql("drop table if exists merch_type_to_items_relations;");
        }
    }
}