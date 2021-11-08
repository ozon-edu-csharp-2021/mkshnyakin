using OzonEdu.MerchandiseService.Domain.Models;

namespace OzonEdu.MerchandiseService.Domain.AggregationModels.MerchRequestAggregate
{
    public sealed class CreationMode : Enumeration
    {
        public static CreationMode Auto = new(1, nameof(Auto));
        public static CreationMode Manual = new(2, nameof(Manual));

        public CreationMode(int id, string name) : base(id, name)
        {
        }
    }
}