namespace Synthesize.FileParsing
{
    public abstract class RegisterBase
    {
        public override string ToString()
        {
            return $"{Name} {Bits}";
        }
        public bool IsCompatible(RegisterBase reg)
        {
            return StartCycle > reg.StopCycle || StopCycle < reg.StartCycle;
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public int Bits { get; set; }
        public int StartCycle { get; set; } = -1;
        public int StopCycle { get; set; } = -1;
        public abstract bool IsDependantOn(Operation op);
    }
}
