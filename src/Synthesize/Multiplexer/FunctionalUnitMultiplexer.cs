using System;
using System.Collections.Generic;
using System.Linq;
using Synthesize.Allocation;
using Synthesize.FileParsing;

namespace Synthesize.Multiplexer
{
    /// <summary>
    /// A multiplexor that selects the left and right side inputs for a functional unit.
    /// </summary>
    public class FunctionalUnitMultiplexer : Multiplexer
    {
        /// <summary>
        /// The shared functional unit for all the logical registers.
        /// </summary>
        public FunctionalUnit Unit { get; set; }

        /// <summary>
        /// The logical operations the multiplexer selects from.
        /// </summary>
        public Operation[] Op { get; set; }

        public override IEnumerable<IAmAUnit> Items => Op;
        public override IEnumerable<string> ItemNames => Op?.Select(op => op.Name) ?? Enumerable.Empty<string>();
        public override Tuple<string, string> GetSelectValueAndNameFrom(int cycle)
        {
            return Op
                .Select((op, index) => new
                {
                    val = index,
                    operation = op
                })
                .Where(op => op.operation.CycleIndex == cycle)
                .Select(op => new Tuple<string, string>(Convert.ToString(op.val, 2).PadLeft(SelectionBitSize, '0'), op.operation.Name))
                .FirstOrDefault() ?? new Tuple<string, string>(new string('X', SelectionBitSize), "");
        }
        public override int OutputBitSize => Op.Max(op => op.Left.Bits + op.Right.Bits);
        public override string Name => $"MX_{Unit.Name}";
        public override string ToString()
        {
            return $"{Name} = {{{string.Join(", ", Op.Select((inputs, index) => $"{Convert.ToString(index, 2).PadLeft(SelectionBitSize, '0')}: [{inputs.Left.Name}, {inputs.Right.Name}]"))}}}";
        }
    }
}
