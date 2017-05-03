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
        private void test(Func<AifFile> getFile, params Dictionary<string, string>[] testCases)
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
                if (testCases.Length > 0)
                {
                    Debug.WriteLine(nameof(dataPathGenerator.SaveTestBench));
                    dataPathGenerator.SaveTestBench(writeStream, testCases);
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
            test(() => AifFileTests.Ellip, new Dictionary<string, string>
            {
                {"inp", "0000"},
                {"TWO", "0001"},
                {"svin2", "0000"},
                {"svin13", "0001"},
                {"svin18", "0000"},
                {"svin26", "0001"},
                {"svin33", "0000"},
                {"svin38", "0000"},
                {"svin39", "0000"},
                {"svout2", "1000"},
                {"svout13", "1110"},
                {"svout18", "0111"},
                {"svout26", "0111"},
                {"svout33", "1000"},
                {"svout38", "0100"},
                {"svout39", "0100"}
            },
            new Dictionary<string, string>
            {
                {"inp", "0000"},
                {"TWO", "0001"},
                {"svin2", "0000"},
                {"svin13", "0001"},
                {"svin18", "0000"},
                {"svin26", "0001"},
                {"svin33", "0000"},
                {"svin38", "0001"},
                {"svin39", "0000"},
                {"svout2", "1000"},
                {"svout13", "1110"},
                {"svout18", "0111"},
                {"svout26", "0111"},
                {"svout33", "1011"},
                {"svout38", "0110"},
                {"svout39", "0100"}
            });
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
                {"c4", Convert.ToString(3, 2).PadLeft(8, '0')},
                {"x3", Convert.ToString(1, 2).PadLeft(8, '0')},
                {"c5", Convert.ToString(4, 2).PadLeft(8, '0')},
                {"x4", Convert.ToString(2, 2).PadLeft(8, '0')},

                {"yout", Convert.ToString(55, 2).PadLeft(8, '0')}
            },
            new Dictionary<string, string>
            {
                {"c1", Convert.ToString(3, 2).PadLeft(8, '0')},
                {"x0", Convert.ToString(2, 2).PadLeft(8, '0')},
                {"c2", Convert.ToString(3, 2).PadLeft(8, '0')},
                {"x1", Convert.ToString(1, 2).PadLeft(8, '0')},
                {"c3", Convert.ToString(5, 2).PadLeft(8, '0')},
                {"x2", Convert.ToString(7, 2).PadLeft(8, '0')},
                {"c4", Convert.ToString(3, 2).PadLeft(8, '0')},
                {"x3", Convert.ToString(1, 2).PadLeft(8, '0')},
                {"c5", Convert.ToString(2, 2).PadLeft(8, '0')},
                {"x4", Convert.ToString(2, 2).PadLeft(8, '0')},

                {"yout", Convert.ToString(51, 2).PadLeft(8, '0')}
            });
        }
        [Test]
        public void TestIir()
        {
            test(() => AifFileTests.Iir, new Dictionary<string, string>
            {
                {"a1", Convert.ToString(1, 2).PadLeft(8, '0')},
                {"y1", Convert.ToString(2, 2).PadLeft(8, '0')},
                {"a2", Convert.ToString(3, 2).PadLeft(8, '0')},
                {"y2", Convert.ToString(4, 2).PadLeft(8, '0')},
                {"b0", Convert.ToString(5, 2).PadLeft(8, '0')},
                {"x0", Convert.ToString(6, 2).PadLeft(8, '0')},
                {"b1", Convert.ToString(3, 2).PadLeft(8, '0')},
                {"x1", Convert.ToString(1, 2).PadLeft(8, '0')},
                {"b2", Convert.ToString(4, 2).PadLeft(8, '0')},
                {"x2", Convert.ToString(2, 2).PadLeft(8, '0')},

                {"yout", Convert.ToString(55, 2).PadLeft(8, '0')}
            },
            new Dictionary<string, string>
            {
                {"a1", Convert.ToString(3, 2).PadLeft(8, '0')},
                {"y1", Convert.ToString(2, 2).PadLeft(8, '0')},
                {"a2", Convert.ToString(3, 2).PadLeft(8, '0')},
                {"y2", Convert.ToString(1, 2).PadLeft(8, '0')},
                {"b0", Convert.ToString(5, 2).PadLeft(8, '0')},
                {"x0", Convert.ToString(7, 2).PadLeft(8, '0')},
                {"b1", Convert.ToString(3, 2).PadLeft(8, '0')},
                {"x1", Convert.ToString(1, 2).PadLeft(8, '0')},
                {"b2", Convert.ToString(2, 2).PadLeft(8, '0')},
                {"x2", Convert.ToString(2, 2).PadLeft(8, '0')},

                {"yout", Convert.ToString(51, 2).PadLeft(8, '0')}
            });
        }
        [Test]
        public void TestInputExample()
        {
            test(() => AifFileTests.InputExample, new Dictionary<string, string>
            {
                {"a", "0001"},
                {"b", "0010"},
                {"c", "0010"},
                {"d", "0010"},
                {"h", "0001"},
                {"g", "0001"},
                {"e", "0010"},
                {"f", "0010"},
                {"i", "0011"}
            },
            new Dictionary<string, string>
            {
                {"a", "0010"},
                {"b", "0001"},
                {"c", "0010"},
                {"d", "0010"},
                {"h", "0010"},
                {"g", "0010"},
                {"e", "0010"},
                {"f", "0001"},
                {"i", "0010"}
            });
        }
        [Test]
        public void TestLattice()
        {
            test(() => AifFileTests.Lattice, new Dictionary<string, string>
            {
                {"x", "00000001"},
                {"px0", "00000010"},
                {"px1", "00000011"},
                {"c1", "00000001"},
                {"c2", "00000101"},
                {"c3", "00000001"},
                {"c4", "00000001"},
                {"c5", "00000001"},
                {"y", "10111110"},
                {"x0", "01001010"},
                {"x1", "00100111"}
            },
            new Dictionary<string, string>
            {
                {"x", "00000001"},
                {"px0", "00000000"},
                {"px1", "00000011"},
                {"c1", "00000001"},
                {"c2", "00000000"},
                {"c3", "00000001"},
                {"c4", "00001100"},
                {"c5", "00000011"},
                {"y", "00100001"},
                {"x0", "00000010"},
                {"x1", "00000010"}
            });
        }
        [Test]
        public void TestToyExample()
        {
            test(() => AifFileTests.ToyExample, new Dictionary<string, string>
            {
                {"a", "0001"},
                {"b", "0010"},
                {"c", "0010"},
                {"d", "0010"},
                {"h", "0001"},
                {"g", "0001"},
                {"e", "0010"},
                {"f", "0010"},
                {"i", "0011"}
            },
            new Dictionary<string, string>
            {
                {"a", "0010"},
                {"b", "0001"},
                {"c", "0010"},
                {"d", "0010"},
                {"h", "0010"},
                {"g", "0010"},
                {"e", "0010"},
                {"f", "0001"},
                {"i", "0010"}
            });
        }
    }
}
