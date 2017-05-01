using System.IO;

namespace Synthesize.Extensions
{
    public static class StreamExtensions
    {
        public static void WriteTextResource(this StreamWriter writer, string name, string indent = null)
        {
            using (var stream = typeof(StreamExtensions).Assembly.GetManifestResourceStream($"Synthesize.DataPath.Text.{name}.txt") ?? new MemoryStream())
            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    writer.WriteLine($"{indent}{reader.ReadLine()?.TrimEnd()}");
                }
            }
        }
        public static void WriteVhdlFile(this StreamWriter writer, string codeFile)
        {
            using (var stream = typeof(StreamExtensions).Assembly.GetManifestResourceStream($"Synthesize.DataPath.Vhdl.{codeFile}.vhd") ?? new MemoryStream())
            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    writer.WriteLine(reader.ReadLine()?.TrimEnd());
                }
            }
        }
    }
}
