namespace OzonEdu.MerchandiseService.Infrastructure.Cache
{
    public class CacheKeysProvider
    {
        const string GetMerchRequestHistory = "GetMerchRequestHistory";

        public string GetMerchRequestHistoryKey(long employeeId) => $"{GetMerchRequestHistory}{employeeId}";
    }
}