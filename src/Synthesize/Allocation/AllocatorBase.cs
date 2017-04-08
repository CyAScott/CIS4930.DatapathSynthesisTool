using NLog;

namespace Synthesize.Allocation
{
    public abstract class AllocatorBase
    {
        protected readonly ILogger Log = LogManager.GetLogger(nameof(AllocatorBase));

        public void Allocate()
        {
        }
    }
}
