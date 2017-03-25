namespace Synthesize.FileParsing
{
    public class Operation
    {
        public override string ToString()
        {
            return $"{Name} {Op} {Bits} {Left.Name} {Right.Name} {Output.Name}";
        }
        public RegisterBase Left { get; set; }
        public RegisterWithParentBase Output { get; set; }
        public RegisterBase Right { get; set; }
        public int Bits { get; set; }
        public string Name { get; set; }
        public string Op { get; set; }
    }
}
