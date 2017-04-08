namespace Synthesize.FileParsing
{
    public abstract class RegisterBase
    {
        public override string ToString()
        {
            return $"{Name} {Bits}";
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public int Bits { get; set; }
        public abstract bool IsDependantOn(Operation op);
    }
}
