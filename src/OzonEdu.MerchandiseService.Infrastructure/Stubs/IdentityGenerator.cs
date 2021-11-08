using System.Threading;

namespace OzonEdu.MerchandiseService.Infrastructure.Stubs
{
    public sealed class IdentityGenerator
    {
        private long _id;

        public long Get()
        {
            return Interlocked.Increment(ref _id);
        }
    }
}