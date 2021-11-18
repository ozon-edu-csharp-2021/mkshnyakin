using System;
using System.Globalization;
using FluentMigrator;

namespace OzonEdu.MerchandiseService.Migrator.Migrations
{
    [Migration(10)]
    public class PopulateMerchRequests : ForwardOnlyMigration
    {
        private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;
        
        public override void Up()
        {
            Execute.Sql(@"
                insert into merch_requests (employee_id, merch_type, status, mode, give_out_date)
                values  (1, 10, 3, 1, '2021-11-23 13:14:00.000000'),
                        (1, 20, 3, 2, '2021-10-15 08:05:01.000000'),
                        (2, 20, 3, 2, '2021-10-15 08:05:01.000000'),
                        (2, 30, 2, 2, null),
                        (3, 10, 3, 2, '2019-10-15 08:05:01.000000'),
                        (4, 10, 3, 2, '2021-11-11 08:05:01.000000'),
                        (5, 10, 2, 2, null),
                        (5, 20, 2, 1, null)
                on conflict do nothing;"
            );
        }
    }
}
