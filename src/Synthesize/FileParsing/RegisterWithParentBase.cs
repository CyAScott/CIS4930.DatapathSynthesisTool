namespace Synthesize.FileParsing
{
    public abstract class RegisterWithParentBase : RegisterBase
    {
        public Operation Parent { get; set; }
    }
}
