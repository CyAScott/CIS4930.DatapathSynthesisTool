using System.Linq;
using Synthesize.FileParsing;

namespace Synthesize.Allocation
{
    public class FunctionalUnit
    {
        public Operation[] Operations { get; set; }
        public int Index { get; set; }
        public string Name => $"{Op}{Index:00}";
        public override string ToString()
        {
            return $"{Name} -> {string.Join(", ", Operations.Select(op => op.Name))}";
        }
        public string Op { get; set; }
    }
}
