using System.Linq;
using Synthesize.FileParsing;

namespace Synthesize.Allocation
{
    public class FunctionalUnit
    {
        public Operation[] Operations { get; set; }
        public int Index { get; set; }
        public override string ToString()
        {
            return $"{Op}{Index:00} -> {string.Join(", ", Operations.Select(op => op.Name))}";
        }
        public string Op { get; set; }
    }
}
