using System;
using System.Linq;
using NUnit.Framework;
using Synthesize.Allocation;
using Synthesize.FileParsing;
using Synthesize.Multiplexer;
using Synthesize.Scheduler;

namespace Synthesize.Tests
{
    [TestFixture]
    public class MultiplexerGeneratorTests
    {
        private void test(Func<AifFile> getFile)
        {
            TestLogger.Setup();

            var file = getFile();

            var schedule = new IlpScheduler(file, file.MinCycles.Values.Max());
            schedule.BuildSchedule();

            var multiplexorGenerator = new MultiplexerGenerator(new RegisterAllocator(new FunctionalUnitAllocator(schedule)));
            Assert.IsNotNull(multiplexorGenerator.FunctionalUnitMultiplexers);
            Assert.IsNotNull(multiplexorGenerator.RegisterUnitMultiplexers);

            Assert.IsTrue(multiplexorGenerator.FunctionalUnitMultiplexers.Length == 0 ||
                multiplexorGenerator.FunctionalUnitMultiplexers.All(unit =>
                    unit?.SelectionBitSize > 0 &&
                    unit.Op?.Length > 1 &&
                    unit.Unit != null));

            Assert.IsTrue(multiplexorGenerator.RegisterUnitMultiplexers.Length == 0 ||
                multiplexorGenerator.RegisterUnitMultiplexers.All(unit =>
                    unit?.SelectionBitSize > 0 &&
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
