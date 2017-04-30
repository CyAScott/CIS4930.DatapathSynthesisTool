namespace Synthesize.FileParsing
{
    /// <summary>
    /// An interface for units (i.e. operations, registers, etc.)
    /// </summary>
    public interface IAmAUnit
    {
        int Bits { get; }
        string Name { get; }
    }
}
