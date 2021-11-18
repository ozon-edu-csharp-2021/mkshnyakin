using OzonEdu.MerchandiseService.Domain.Exceptions;
using OzonEdu.MerchandiseService.Domain.Models;

namespace OzonEdu.MerchandiseService.Domain.AggregationModels.MerchRequestAggregate
{
    public sealed class RequestMerchType : Enumeration
    {
        public static RequestMerchType WelcomePack = new(10, nameof(WelcomePack));
        public static RequestMerchType ConferenceListenerPack = new(20, nameof(ConferenceListenerPack));
        public static RequestMerchType ConferenceSpeakerPack = new(30, nameof(ConferenceSpeakerPack));
        public static RequestMerchType ProbationPeriodEndingPack = new(40, nameof(ProbationPeriodEndingPack));
        public static RequestMerchType VeteranPack = new(50, nameof(VeteranPack));

        public RequestMerchType(int id, string name) : base(id, name)
        {
        }
        
        public static RequestMerchType Create(int id)
        {
            return id switch
            {
                10 => WelcomePack,
                20 => ConferenceListenerPack,
                30 => ConferenceSpeakerPack,
                40 => ProbationPeriodEndingPack,
                50 => VeteranPack,
                _ => throw new CorruptedValueObjectException($"{nameof(id)} is invalid. Id: {id}")
            };
        }
    }
}