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

        public static ProcessStatus Create(int id)
        {
            return id switch
            {
                1 => Draft,
                2 => OutOfStock,
                3 => Complete,
                _ => throw new CorruptedValueObjectException($"{nameof(id)} is invalid. Id: {id}")
            };
        }
    }
}