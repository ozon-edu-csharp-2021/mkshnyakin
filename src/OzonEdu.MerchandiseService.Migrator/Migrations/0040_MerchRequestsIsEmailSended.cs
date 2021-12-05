using FluentMigrator;

namespace OzonEdu.MerchandiseService.Migrator.Migrations
{
    [Migration(40)]
    public class MerchRequestsIsEmailSended : Migration
    {
        public override void Up()
        {
            Execute.Sql(@"
                alter table merch_requests
                    add is_email_sended boolean default FALSE not null;
                "
            );
        }

        public override void Down()
        {
            Execute.Sql("alter table merch_requests drop column is_email_sended;");
        }
    }
}