using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using NLog;
using NUnit.Framework;
using Synthesize.Allocation;
using Synthesize.DataPath;
using Synthesize.FileParsing;
using Synthesize.Multiplexer;
using Synthesize.Scheduler;

namespace Synthesize.Tests
{
    [TestFixture]
    public class DataPathGeneratorTests
    {
        private void printSave(ILogger log, Action<StreamWriter, bool> writer)
        {
            using (var stream = new MemoryStream())
            using (var writeStream = new StreamWriter(stream))
            {
                writer(writeStream, false);
                writeStream.Flush();
                log.Info(Encoding.UTF8.GetString(stream.ToArray()));
            }
        }
        private void test(Func<AifFile> getFile)
        {
            TestLogger.Setup();
            
            var log = LogManager.GetLogger(nameof(DataPathGeneratorTests));
            var file = getFile();

            var schedule = new IlpScheduler(file, file.MinCycles.Values.Max());
            schedule.BuildSchedule();

            var dataPathGenerator = new DataPathGenerator(new MultiplexerGenerator(new RegisterAllocator(new FunctionalUnitAllocator(schedule))));
            
            Debug.WriteLine(nameof(dataPathGenerator.SaveController));
            printSave(log, dataPathGenerator.SaveController);

            Debug.WriteLine(nameof(dataPathGenerator.SaveDesign));
            printSave(log, dataPathGenerator.SaveDesign);

            Debug.WriteLine(nameof(dataPathGenerator.SaveDataPath));
            printSave(log, dataPathGenerator.SaveDataPath);

            //Debug.WriteLine(nameof(dataPathGenerator.SaveTestBench));
            //printSave(log, dataPathGenerator.SaveTestBench);
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
