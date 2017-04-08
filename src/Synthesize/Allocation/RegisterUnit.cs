using System.Linq;
using Synthesize.FileParsing;

namespace Synthesize.Allocation
{
    public class RegisterUnit
    {
        public RegisterBase[] Registers { get; set; }
        public int Index { get; set; }
        public override string ToString()
        {
            return $"REG{Index:00} -> {string.Join(", ", Registers.Select(reg => reg.Name))}";
        }
    }
}
