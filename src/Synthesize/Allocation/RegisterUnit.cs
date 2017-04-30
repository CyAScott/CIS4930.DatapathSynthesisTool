using System.Linq;
using Synthesize.FileParsing;

namespace Synthesize.Allocation
{
    public class RegisterUnit
    {
        public RegisterBase[] Registers { get; set; }
        public int Bits => Registers == null || Registers.Length == 0 ? 0 : Registers.Max(reg => reg.Bits);
        public int Index { get; set; }
        public string Name => $"REG{Index:00}";
        public override string ToString()
        {
            return $"{Name} -> {string.Join(", ", Registers.Select(reg => reg.Name))}";
        }
    }
}
