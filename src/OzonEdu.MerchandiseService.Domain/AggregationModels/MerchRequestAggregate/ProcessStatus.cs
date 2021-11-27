using OzonEdu.MerchandiseService.Domain.Exceptions;
using OzonEdu.MerchandiseService.Domain.Models;

namespace OzonEdu.MerchandiseService.Domain.AggregationModels.MerchRequestAggregate
{
    public sealed class ProcessStatus : Enumeration
    {
        public static ProcessStatus Draft = new(1, nameof(Draft));
        public static ProcessStatus OutOfStock = new(2, nameof(OutOfStock));
        public static ProcessStatus Complete = new(3, nameof(Complete));

        public ProcessStatus(int id, string name) : base(id, name)
        {
        }
    }
}