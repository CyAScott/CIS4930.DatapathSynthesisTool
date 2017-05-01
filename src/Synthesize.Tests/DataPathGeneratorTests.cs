using System;
using System.Collections.Generic;
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
        private void test(Func<AifFile> getFile, Dictionary<string, string> testCase = null)
        {
            TestLogger.Setup();
            
            var log = LogManager.GetLogger(nameof(DataPathGeneratorTests));
            var file = getFile();

            var schedule = new IlpScheduler(file, file.MinCycles.Values.Max());
            schedule.BuildSchedule();

            var dataPathGenerator = new DataPathGenerator(new MultiplexerGenerator(new RegisterAllocator(new FunctionalUnitAllocator(schedule))));


            using (var stream = new MemoryStream())
            using (var writeStream = new StreamWriter(stream))
            {
                if (testCase != null)
                {
                    Debug.WriteLine(nameof(dataPathGenerator.SaveTestBench));
                    dataPathGenerator.SaveTestBench(writeStream, new[]
                    {
                        testCase
                    });
                }

                Debug.WriteLine(nameof(dataPathGenerator.SaveController));
                dataPathGenerator.SaveController(writeStream);

                Debug.WriteLine(nameof(dataPathGenerator.SaveDataPath));
                dataPathGenerator.SaveDataPath(writeStream, true);

                Debug.WriteLine(nameof(dataPathGenerator.SaveDesign));
                dataPathGenerator.SaveDesign(writeStream);

                writeStream.Flush();
                log.Info(Encoding.UTF8.GetString(stream.ToArray()));
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
            test(() => AifFileTests.Fir, new Dictionary<string, string>
            {
                {"c1", Convert.ToString(1, 2).PadLeft(8, '0')},
                {"x0", Convert.ToString(2, 2).PadLeft(8, '0')},
                {"c2", Convert.ToString(3, 2).PadLeft(8, '0')},
                {"x1", Convert.ToString(4, 2).PadLeft(8, '0')},
                {"c3", Convert.ToString(5, 2).PadLeft(8, '0')},
                {"x2", Convert.ToString(6, 2).PadLeft(8, '0')},
                {"c4", Convert.ToString(7, 2).PadLeft(8, '0')},
                {"x3", Convert.ToString(8, 2).PadLeft(8, '0')},
                {"c5", Convert.ToString(9, 2).PadLeft(8, '0')},
                {"x4", Convert.ToString(10, 2).PadLeft(8, '0')},

                {"yout", Convert.ToString(10, 2).PadLeft(190, '0')}
            });
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
