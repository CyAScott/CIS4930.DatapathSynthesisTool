namespace Synthesize.FileParsing
{
    public class InputRegister : RegisterBase
    {
        public override bool IsDependantOn(Operation op)
        {
            return false;
        }
    }
}
