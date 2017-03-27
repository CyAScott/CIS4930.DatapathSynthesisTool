using NUnit.Framework;
using Synthesize.Scheduler;

namespace Synthesize.Tests
{
    [TestFixture]
    public class IlpScheduleTest
    {
        [Test]
        public void TestEllip()
        {
            TestLogger.Setup();

            var schedule = new IlpScheduler(AifFileTests.Ellip);
            Assert.IsNotNull(schedule);

            schedule.BuildSchedule();
        }
        [Test]
        public void TestFir()
        {
            TestLogger.Setup();

            var schedule = new IlpScheduler(AifFileTests.Fir);
            Assert.IsNotNull(schedule);

            schedule.BuildSchedule();
        }
        [Test]
        public void TestIir()
        {
            TestLogger.Setup();

            var schedule = new IlpScheduler(AifFileTests.Iir);
            Assert.IsNotNull(schedule);

            schedule.BuildSchedule();
        }
        [Test]
        public void TestLattice()
        {
            TestLogger.Setup();

            var schedule = new IlpScheduler(AifFileTests.Lattice);
            Assert.IsNotNull(schedule);

            schedule.BuildSchedule();
        }
        [Test]
        public void TestToyExample()
        {
            TestLogger.Setup();

            var schedule = new IlpScheduler(AifFileTests.ToyExample);
            Assert.IsNotNull(schedule);

            schedule.BuildSchedule();
        }
    }
}
