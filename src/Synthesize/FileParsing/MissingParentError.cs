using System;

namespace Synthesize.FileParsing
{
    public class MissingParentError : ArgumentException
    {
        public MissingParentError(string registerName)
            : base($"Register \"{registerName}\" is missing a parent.")
        {
        }
    }
}
