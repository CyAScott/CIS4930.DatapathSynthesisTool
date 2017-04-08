using System;
using System.Linq;
using NUnit.Framework;
using Synthesize.FileParsing;
using Synthesize.Scheduler;

namespace Synthesize.Tests
{
    [TestFixture]
    public class IlpScheduleTest
    {
        private void test(Func<AifFile> getFile)
        {
            TestLogger.Setup();

            var schedule = new IlpScheduler(getFile());
            Assert.IsNotNull(schedule);

            schedule.BuildSchedule();

            //make sure all the operations are included in the schedule
            var operations = schedule.Cycles.SelectMany(cycle => cycle).ToArray();
            Assert.AreEqual(operations.Length, schedule.AifFile.Operations.Count);
            Assert.IsTrue(operations.All(operation => schedule.AifFile.Operations.ContainsValue(operation)));

            Assert.AreEqual(schedule.Cycles.Length, schedule.AifFile.MinCycles.Values.Max(), "The schedule does not have the min number of cycles.");

            //check the order of operations
            for (var index = 1; index < schedule.Cycles.Length; index++)
            {
                var cycle1 = schedule.Cycles[index - 1];
                var cycle2 = schedule.Cycles[index];

                Assert.IsTrue(cycle1.All(op1 => cycle2.All(op2 => !op1.IsDependantOn(op2))), "The order of operations are invalid.");
            }
        }

        [Test]
        public void TestBookExample()
        {
            test(() => AifFileTests.BookExample);
        }
        [Test]
        public void TestEllip()
        {
            test(() => AifFileTests.Ellip);
        }
        [Test]
        public void TestFir()
        {
            test(() => AifFileTests.Fir);
        }
        [Test]
        public void TestIir()
        {
            test(() => AifFileTests.Iir);
        }
        [Test]
        public void TestInputExample()
        {
            test(() => AifFileTests.InputExample);
        }
        [Test]
        public void TestLattice()
        {
            test(() => AifFileTests.Lattice);
        }
        [Test]
        public void TestToyExample()
        {
            test(() => AifFileTests.ToyExample);
        }
    }
}
