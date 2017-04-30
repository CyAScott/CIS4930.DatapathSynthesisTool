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
    }
}
