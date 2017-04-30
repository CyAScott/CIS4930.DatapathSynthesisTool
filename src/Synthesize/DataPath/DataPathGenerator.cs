using System.IO;
using Synthesize.Allocation;
using Synthesize.FileParsing;
using Synthesize.Multiplexer;
using Synthesize.Scheduler;

namespace Synthesize.DataPath
{
    public partial class DataPathGenerator
    {
        protected string GetText(string name)
        {
            using (var stream = typeof(DataPathGenerator).Assembly.GetManifestResourceStream(name) ?? new MemoryStream())
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
        public DataPathGenerator(MultiplexerGenerator multiplexors)
        {
            Multiplexers = multiplexors;

            RegisterAllocator = Multiplexers.RegisterAllocator;

            Functional = RegisterAllocator.Functional;

            Scheduler = Functional.Scheduler;

            AifFile = Scheduler.AifFile;
        }

        public AifFile AifFile { get; }
        public MultiplexerGenerator Multiplexers { get; }
        public FunctionalUnitAllocator Functional { get; }
        public RegisterAllocator RegisterAllocator { get; }
        public SchedulerBase Scheduler { get; }
    }
}
