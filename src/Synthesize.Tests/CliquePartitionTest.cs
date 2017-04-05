using NUnit.Framework;
using Synthesize.CliquePartition;

namespace Synthesize.Tests
{
    [TestFixture]
    public class CliquePartitionTest
    {
        [Test]
        public void Test()
        {
            TestLogger.Setup();
            
            CliqueHelper.CliquePartition(new[]
            {
                new [] { 1, 0, 0, 1, 0, 1, 0, 1, 0 },
                new [] { 0, 1, 0, 1, 0, 1, 0, 1, 0 },
                new [] { 0, 0, 1, 0, 0, 0, 0, 0, 0 },
                new [] { 1, 1, 0, 1, 0, 0, 0, 1, 0 },
                new [] { 0, 0, 0, 0, 1, 0, 0, 0, 0 },
                new [] { 1, 1, 0, 0, 0, 1, 0, 1, 0 },
                new [] { 0, 0, 0, 0, 0, 0, 1, 0, 0 },
                new [] { 1, 1, 0, 1, 0, 1, 0, 1, 0 },
                new [] { 0, 0, 0, 0, 0, 0, 0, 0, 1 }
            });
        }
    }
}
