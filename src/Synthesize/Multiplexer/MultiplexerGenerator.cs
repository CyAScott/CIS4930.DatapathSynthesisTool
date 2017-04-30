using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using Synthesize.Allocation;

namespace Synthesize.Multiplexer
{
    public class MultiplexerGenerator
    {
        protected readonly ILogger Log = LogManager.GetLogger(nameof(AllocatorBase));

        public MultiplexerGenerator(RegisterAllocator allocator)
        {
            RegisterAllocator = allocator;

            FunctionalUnitMultiplexers = allocator.Functional.Units
                .Where(unit => unit.Operations.Length > 1)
                .Select((unit, index) => new FunctionalUnitMultiplexer
                {
                    SelectionBitSize = Convert.ToInt32(Math.Ceiling(Math.Log(unit.Operations.Length, 2))),
                    Op = unit.Operations,
                    Unit = unit
                })
                .ToArray();

            foreach (var multiplexor in FunctionalUnitMultiplexers.OrderBy(mx => mx.Name))
            {
                Log.Info(multiplexor);
            }

            RegisterUnitMultiplexers = allocator.Units
                .Where(unit => unit.Registers.Length > 1)
                .Select((unit, index) => new RegisterUnitMultiplexer
                {
                    SelectionBitSize = Convert.ToInt32(Math.Ceiling(Math.Log(unit.Registers.Length, 2))),
                    Registers = unit.Registers,
                    Unit = unit
                })
                .ToArray();

            foreach (var multiplexor in RegisterUnitMultiplexers.OrderBy(mx => mx.Name))
            {
                Log.Info(multiplexor);
            }
        }

        public FunctionalUnitMultiplexer[] FunctionalUnitMultiplexers { get; }
        public IEnumerable<Multiplexer> Multiplexers =>
            FunctionalUnitMultiplexers?.Cast<Multiplexer>().Concat(RegisterUnitMultiplexers) ??
            RegisterUnitMultiplexers ??
            Enumerable.Empty<Multiplexer>();
        public RegisterUnitMultiplexer[] RegisterUnitMultiplexers { get; }
        public RegisterAllocator RegisterAllocator { get; }
        public int BusBitWidth => Multiplexers.Sum(unit => unit.SelectionBitSize);
        public int StateCount => RegisterAllocator.Functional.Scheduler.Cycles.Length;
    }
}
