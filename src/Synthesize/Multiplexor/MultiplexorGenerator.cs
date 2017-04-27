using System;
using System.Linq;
using NLog;
using Synthesize.Allocation;
using Synthesize.FileParsing;

namespace Synthesize.Multiplexor
{
    public class MultiplexorGenerator
    {
        protected readonly ILogger Log = LogManager.GetLogger(nameof(AllocatorBase));

        public MultiplexorGenerator(RegisterAllocator allocator)
        {
            RegisterAllocator = allocator;

            FunctionalUnitMultiplexors = allocator.Functional.Units
                .Where(unit => unit.Operations.Length > 1)
                .Select((unit, index) => new FunctionalUnitMultiplexor
                {
                    BitSize = Convert.ToInt32(Math.Ceiling(Math.Log(unit.Operations.Length, 2))),
                    Inputs = unit.Operations.Select(op => new Tuple<RegisterBase, RegisterBase>(op.Left, op.Right)).ToArray(),
                    Unit = unit,
                })
                .ToArray();

            foreach (var multiplexor in FunctionalUnitMultiplexors.OrderBy(mx => mx.Name))
            {
                Log.Info(multiplexor);
            }

            RegisterUnitMultiplexors = allocator.Units
                .Where(unit => unit.Registers.Length > 1)
                .Select((unit, index) => new RegisterUnitMultiplexor
                {
                    BitSize = Convert.ToInt32(Math.Ceiling(Math.Log(unit.Registers.Length, 2))),
                    Registers = unit.Registers,
                    Unit = unit
                })
                .ToArray();

            foreach (var multiplexor in RegisterUnitMultiplexors.OrderBy(mx => mx.Name))
            {
                Log.Info(multiplexor);
            }
        }

        public FunctionalUnitMultiplexor[] FunctionalUnitMultiplexors { get; }
        public RegisterUnitMultiplexor[] RegisterUnitMultiplexors { get; }
        public RegisterAllocator RegisterAllocator { get; }
    }
}
