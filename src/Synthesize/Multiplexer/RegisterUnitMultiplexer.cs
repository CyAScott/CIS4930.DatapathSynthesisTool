using System;
using System.Collections.Generic;
using System.Linq;
using Synthesize.Allocation;
using Synthesize.FileParsing;

namespace Synthesize.Multiplexer
{
    public class RegisterUnitMultiplexer : Multiplexer
    {
        /// <summary>
        /// The logical registers the multiplexer selects from.
        /// </summary>
        public RegisterBase[] Registers { get; set; }

        /// <summary>
        /// The shared register unit for all the logical registers.
        /// </summary>
        public RegisterUnit Unit { get; set; }

        public override IEnumerable<IAmAUnit> Items => Registers;
        public override IEnumerable<string> ItemNames => Registers?.Select(reg => reg.Name) ?? Enumerable.Empty<string>();
        public override Tuple<string, string> GetSelectValueAndNameFrom(int cycle)
        {
            return Registers
                .Select((item, index) => new
                {
                    val = index,
                    reg = item
                })
                .Where(item => item.reg.StartCycle <= cycle && cycle <= item.reg.StopCycle)
                .Select(item => new Tuple<string, string>(Convert.ToString(item.val, 2).PadLeft(SelectionBitSize, '0'), item.reg.Name))
                .FirstOrDefault() ?? new Tuple<string, string>(new string('X', SelectionBitSize), "");
        }
        public override int OutputBitSize => Unit.Bits;
        public override string Name => $"MX_{Unit.Name}";
        public override string ToString()
        {
            return $"{Name} = {{{string.Join(", ", Registers.Select((reg, index) => $"{Convert.ToString(index, 2).PadLeft(SelectionBitSize, '0')}: {reg.Name}"))}}}";
        }
    }
}
