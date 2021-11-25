using FluentMigrator;

namespace OzonEdu.MerchandiseService.Migrator.Migrations
{
    [Migration(20)]
    public class PopulateMerchPackItems : ForwardOnlyMigration
    {
        public override void Up()
        {
            Execute.Sql(@"
                insert into merch_pack_items (name, sku)
                values
                    ('TShirtStarter L', 4),
                    ('NotepadStarter', 41),
                    ('PenStarter', 42),
                    ('SocksStarter', 43),

                    ('TShirtСonferenceListener L', 28),
                    ('NotepadСonferenceListener', 46),
                    ('PenСonferenceListener', 47),
                       
                    ('SweatshirtСonferenceSpeaker L', 22),
                    ('NotepadСonferenceSpeaker', 44),
                    ('PenСonferenceSpeaker', 45),
                       
                    ('TShirtAfterProbation L', 10),
                    ('SweatshirtAfterProbation L', 16),
                       
                    ('TShirtVeteran L', 33),
                    ('SweatshirtVeteran XL', 39),
                    ('NotepadVeteran', 48),
                    ('PenVeteran', 49),
                    ('CardHolderVeteran', 50)
                on conflict do nothing;"
            );
        }
    }
}