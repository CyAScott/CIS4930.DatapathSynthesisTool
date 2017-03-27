using System.Collections.Generic;
using System.Linq;
using Synthesize.FileParsing;

namespace Synthesize.Scheduler
{
    public class ListScheduler : SchedulerBase
    {
        public ListScheduler(AifFile file, Dictionary<string, int> resources)
            : base(file)
        {
            Resources = resources;
        }
        public override void BuildSchedule()
        {
            var operations = AifFile.Operations.Values.ToList();
            var readyRegisters = AifFile.Registers.Values.OfType<InputRegister>().Cast<RegisterBase>().ToList();

            var cycles = new List<Operation[]>();
            while (operations.Count > 0)
            {
                var cycle = operations
                    .Where(op => readyRegisters.Contains(op.Left) && readyRegisters.Contains(op.Right))
                    .GroupBy(op => op.Op)
                    .SelectMany(group => group.Take(Resources[group.Key]))
                    .ToArray();

                operations.RemoveAll(op => cycle.Contains(op));

                readyRegisters.AddRange(cycle.Select(op => op.Output));

                cycles.Add(cycle);
            }
            Cycles = cycles.ToArray();

            PrintSchedule();
        }
    }
}
