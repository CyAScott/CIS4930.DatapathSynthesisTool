namespace Synthesize.FileParsing
{
    public class Operation
    {
        public override string ToString()
        {
            return $"{Name, -10} {Op, -5} {Bits:00} {Left.Name, -10} {Right.Name, -10} {Output.Name, -10}";
        }
        public RegisterBase Left { get; set; }
        public RegisterWithParentBase Output { get; set; }
        public RegisterBase Right { get; set; }
        public bool IsDependantOn(Operation op)
        {
            return (Left?.IsDependantOn(op) ?? false) || (Right?.IsDependantOn(op) ?? false);
        }
        public int Bits { get; set; }
        public int CycleIndex { get; set; } = -1;
        public string Id { get; set; }
        public string Name { get; set; }
        public string Op { get; set; }
    }
}
