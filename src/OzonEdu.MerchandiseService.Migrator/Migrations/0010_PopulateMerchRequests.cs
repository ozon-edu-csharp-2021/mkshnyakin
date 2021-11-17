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
            Insert.IntoTable(TableNames.MerchRequests)
                .Row(new
                {
                    employee_id = 1,
                    merch_type = 10,
                    status = 3,
                    mode = 1,
                    give_out_date = DateTime.Parse("11/23/2021 13:14:00", Culture, DateTimeStyles.None)
                })
                .Row(new
                {
                    employee_id = 1,
                    merch_type = 20,
                    status = 3,
                    mode = 2,
                    give_out_date = DateTime.Parse("10/15/2021 08:05:01", Culture, DateTimeStyles.None)
                })
                .Row(new
                {
                    employee_id = 2,
                    merch_type = 20,
                    status = 3,
                    mode = 2,
                    give_out_date = DateTime.Parse("10/15/2021 08:05:01", Culture, DateTimeStyles.None)
                })
                .Row(new
                {
                    employee_id = 2,
                    merch_type = 30,
                    status = 2,
                    mode = 2
                })
                .Row(new
                {
                    employee_id = 3,
                    merch_type = 10,
                    status = 3,
                    mode = 2,
                    give_out_date = DateTime.Parse("10/15/2019 08:05:01", Culture, DateTimeStyles.None)
                })
                .Row(new
                {
                    employee_id = 4,
                    merch_type = 10,
                    status = 3,
                    mode = 2,
                    give_out_date = DateTime.Parse("11/11/2021 08:05:01", Culture, DateTimeStyles.None)
                })
                .Row(new
                {
                    employee_id = 5,
                    merch_type = 10,
                    status = 2,
                    mode = 2
                })
                .Row(new
                {
                    employee_id = 5,
                    merch_type = 20,
                    status = 2,
                    mode = 1
                })
                ;
        }
    }
}
