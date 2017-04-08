namespace Synthesize.FileParsing
{
    public abstract class RegisterWithParentBase : RegisterBase
    {
        public Operation Parent { get; set; }
        public override bool IsDependantOn(Operation op)
        {
            return Parent != null && (Parent == op || Parent.IsDependantOn(op));
        }
    }
}
