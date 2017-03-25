using System;
namespace Synthesize.FileParsing
{
    public class CircularReferenceError : ArgumentException
    {
        public CircularReferenceError(string registerName)
            : base($"The register \"{registerName}\" is referenced multiple times in the same data path.")
        {
        }
    }
}
