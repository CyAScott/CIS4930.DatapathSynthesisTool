using System;
using System.Linq;
using NUnit.Framework;
using Synthesize.Allocation;
using Synthesize.FileParsing;
using Synthesize.Multiplexor;
using Synthesize.Scheduler;

namespace Synthesize.Tests
{
    [TestFixture]
    public class MultiplexorGeneratorTests
    {
        private void test(Func<AifFile> getFile)
        {
            TestLogger.Setup();

            var file = getFile();

            var schedule = new IlpScheduler(file, file.MinCycles.Values.Max());
            schedule.BuildSchedule();

            var multiplexorGenerator = new MultiplexorGenerator(new RegisterAllocator(new FunctionalUnitAllocator(schedule)));
            Assert.IsNotNull(multiplexorGenerator.FunctionalUnitMultiplexors);
            Assert.IsNotNull(multiplexorGenerator.RegisterUnitMultiplexors);

            Assert.IsTrue(multiplexorGenerator.FunctionalUnitMultiplexors.Length == 0 ||
                multiplexorGenerator.FunctionalUnitMultiplexors.All(unit =>
                    unit?.BitSize > 0 &&
                    unit.Inputs?.Length > 1 &&
                    unit.Unit != null));

            Assert.IsTrue(multiplexorGenerator.RegisterUnitMultiplexors.Length == 0 ||
                multiplexorGenerator.RegisterUnitMultiplexors.All(unit =>
                    unit?.BitSize > 0 &&
                    unit.Registers?.Length > 1 &&
                    unit.Unit != null));
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
