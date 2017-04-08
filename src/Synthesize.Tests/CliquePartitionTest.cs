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
                new [] { 1, 0, 0, 1, 0, 1, 0, 1, 0 },//0
                new [] { 0, 1, 0, 1, 0, 1, 0, 1, 0 },//1
                new [] { 0, 0, 1, 0, 0, 0, 0, 0, 0 },//2
                new [] { 1, 1, 0, 1, 0, 0, 0, 1, 0 },//3
                new [] { 0, 0, 0, 0, 1, 0, 0, 0, 0 },//4
                new [] { 1, 1, 0, 0, 0, 1, 0, 1, 0 },//5
                new [] { 0, 0, 0, 0, 0, 0, 1, 0, 0 },//6
                new [] { 1, 1, 0, 1, 0, 1, 0, 1, 0 },//7
                new [] { 0, 0, 0, 0, 0, 0, 0, 0, 1 } //8
            });

            CliqueHelper.CliquePartition(new[]
            {
                new [] { 1, 0, 1, 0, 1, 0 },//0
                new [] { 0, 1, 1, 0, 1, 0 },//1
                new [] { 1, 1, 1, 0, 0, 0 },//2
                new [] { 0, 0, 0, 1, 1, 0 },//3
                new [] { 1, 1, 0, 1, 1, 0 },//4
                new [] { 0, 0, 0, 0, 0, 1 } //5
            });

            CliqueHelper.CliquePartition(new[]
            {
                new [] { 1, 0, 1, 1, 1, 1 },//0
                new [] { 0, 1, 1, 1, 1, 1 },//1
                new [] { 1, 1, 1, 0, 1, 1 },//2
                new [] { 1, 1, 0, 1, 1, 1 },//3
                new [] { 1, 1, 1, 1, 1, 0 },//4
                new [] { 1, 1, 1, 1, 0, 1 } //5
            });
        }
    }
}
