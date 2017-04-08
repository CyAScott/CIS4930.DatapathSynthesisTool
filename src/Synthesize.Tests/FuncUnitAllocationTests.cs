using System;
using System.Linq;
using NUnit.Framework;
using Synthesize.Allocation;
using Synthesize.FileParsing;
using Synthesize.Scheduler;

namespace Synthesize.Tests
{
    [TestFixture]
    public class FuncUnitAllocationTests
    {
        private void test(Func<AifFile> getFile)
        {
            TestLogger.Setup();

            var schedule = new IlpScheduler(getFile());
            schedule.BuildSchedule();

            var funcUnitAllocator = new FunctionalUnitAllocator(schedule);
            Assert.IsNotNull(funcUnitAllocator.Units);

            //The IP scheduler has determined the optimal resources needed.
            //Compare the results of functional allocator to the resources results from the IP scheduler.
            foreach (var resource in schedule.Resources)
            {
                Assert.AreEqual(funcUnitAllocator.Units.Count(unit => unit.Op == resource.Key), resource.Value, $"The unit count for {resource.Key} is wrong.");
            }

            foreach (var unit in funcUnitAllocator.Units)
            {
                //operations that share the same functional unit cannot share the same cycle index
                Assert.AreEqual(unit.Operations.Length, unit.Operations.Select(op => op.CycleIndex).Distinct().Count(),
                    "Two operations with the same cycle index are sharing the same function unit.");
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
