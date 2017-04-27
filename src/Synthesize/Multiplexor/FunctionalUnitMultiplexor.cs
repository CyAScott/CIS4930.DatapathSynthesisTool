using System;
using System.Linq;
using Synthesize.Allocation;
using Synthesize.FileParsing;

namespace Synthesize.Multiplexor
{
    public class FunctionalUnitMultiplexor
    {
        public FunctionalUnit Unit { get; set; }
        public Tuple<RegisterBase, RegisterBase>[] Inputs { get; set; }
        public int BitSize { get; set; }
        public string Name => $"MX_{Unit.Name}";
        public override string ToString()
        {
            return $"{Name} = {{{string.Join(", ", Inputs.Select((inputs, index) => $"{Convert.ToString(index, 2).PadLeft(BitSize, '0')}: [{inputs.Item1.Name}, {inputs.Item2.Name}]"))}}}";
        }
    }
}
