using OzonEdu.MerchandiseService.Domain.Exceptions;
using OzonEdu.MerchandiseService.Domain.Models;

namespace OzonEdu.MerchandiseService.Domain.AggregationModels.MerchRequestAggregate
{
    public sealed class CreationMode : Enumeration
    {
        public static CreationMode System = new(1, nameof(System));
        public static CreationMode User = new(2, nameof(User));

        public CreationMode(int id, string name) : base(id, name)
        {
        }
    }
}