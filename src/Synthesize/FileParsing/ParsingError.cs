using System;

namespace Synthesize.FileParsing
{
    public class ParsingError : ArgumentException
    {
        public ParsingError(string sectionName, string line, int lineNumber, int lineIndex, string message)
            : base($"Error parsing {sectionName} ({lineNumber}, {lineIndex}): {message}")
        {
            Line = line;
            LineIndex = lineIndex;
            LineNumber = lineNumber;
            SectionName = sectionName;
        }

        public readonly int LineIndex, LineNumber;
        public readonly string Line, SectionName;
    }
}
