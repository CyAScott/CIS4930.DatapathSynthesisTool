using System.Linq;
using Synthesize.FileParsing;

namespace Synthesize.Allocation
{
    public class FunctionalUnit
    {
        public Operation[] Operations { get; set; }
        public int Bits => Operations == null || Operations.Length == 0 ? 0 : Operations.Max(op => op.Bits);
        public int Index { get; set; }
        public string Name => $"{Op}{Index:00}";
        public string VhdlCodeFile { get; set; }
        public override string ToString()
        {
            return $"{Name} -> {string.Join(", ", Operations.Select(op => op.Name))}";
        }
        public string Op { get; set; }
    }
}
