using System;
using System.Linq;
using NUnit.Framework;
using Synthesize.Allocation;
using Synthesize.FileParsing;
using Synthesize.Scheduler;

namespace Synthesize.Tests
{
    [TestFixture]
    public class RegisterAllocationTests
    {
        private void test(Func<AifFile> getFile)
        {
            TestLogger.Setup();

            var file = getFile();

            var schedule = new IlpScheduler(file, file.MinCycles.Values.Max());
            schedule.BuildSchedule();

            var registerAllocator = new RegisterAllocator(new FunctionalUnitAllocator(schedule));
            Assert.IsNotNull(registerAllocator.Units);

            var registers = registerAllocator.Units.SelectMany(unit => unit.Registers).ToArray();
            Assert.AreEqual(registers.Length, schedule.AifFile.Registers.Count);
            Assert.IsTrue(registers.All(reg => schedule.AifFile.Registers.ContainsValue(reg)));

            Assert.IsTrue(registerAllocator.Units
                .All(unit => unit.Registers
                    .All(reg1 => unit.Registers
                        .Where(reg2 => reg1 != reg2)
                        .All(reg1.IsCompatible))));
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
