using System.Collections.Generic;
using System.Linq;
using NLog;
using Synthesize.FileParsing;

namespace Synthesize.Scheduler
{
    public abstract class SchedulerBase
    {
        protected readonly ILogger Log = LogManager.GetLogger(nameof(SchedulerBase));
        protected SchedulerBase(AifFile file)
        {
            AifFile = file;
        }

        public AifFile AifFile { get; }
        public Dictionary<string, int> Resources { get; protected set; }
        public Operation[][] Cycles { get; protected set; }
        public abstract void BuildSchedule();
        public void PrintSchedule()
        {
            Log.Info("Resources: " + string.Join(", ", Resources.Select(pair => $"{pair.Key} = {pair.Value}")));

            for (var cycle = 0; cycle < Cycles.Length; cycle++)
            {
                Log.Info($"Cycle {cycle:00}: " + string.Join(", ", Cycles[cycle]
                    .GroupBy(op => op.Op)
                    .Select(group => $"{group.Key} -> {{{string.Join(", ", group.Select(op => op.Name))}}}")));
            }
        }
    }
}
