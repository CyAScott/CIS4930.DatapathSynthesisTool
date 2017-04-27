using System;
using System.Linq;
using Synthesize.Allocation;
using Synthesize.FileParsing;

namespace Synthesize.Multiplexor
{
    public class RegisterUnitMultiplexor
    {
        public RegisterBase[] Registers { get; set; }
        public RegisterUnit Unit { get; set; }
        public int BitSize { get; set; }
        public string Name => $"MX_{Unit.Name}";
        public override string ToString()
        {
            return $"{Name} = {{{string.Join(", ", Registers.Select((reg, index) => $"{Convert.ToString(index, 2).PadLeft(BitSize, '0')}: {reg.Name}"))}}}";
        }
    }
}
