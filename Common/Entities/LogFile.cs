using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Common
{
    public class LogFile
    {
        public Queue<LogFileRow> Queue { get; private set; }
        public EnvironmentReference EnvironmentReference { get; private set; }

        [JsonConstructor()]
        public LogFile(EnvironmentReference environmentReference, IEnumerable<LogFileRow> queue)
        {
            Queue = new Queue<LogFileRow>(queue);
            EnvironmentReference = environmentReference;
        }

        public LogFile() : base()
        {
            Queue = new Queue<LogFileRow>();
        }

        public Stream ToSingleLogReadingsStream()
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.WriteLine(EnvironmentReference.Serialize());
            while(Queue.Any())
            {
                var row = Queue.Dequeue();
                writer.WriteLine(row.OriginalRow);
            }
            writer.Flush();
            stream.Position = 0;
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }
    }
}
