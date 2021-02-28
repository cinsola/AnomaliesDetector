using Common.Exceptions;
using Common.Providers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Common.MapReduce
{
    public abstract class ETLBaseMapReduce
    {
        private readonly ILogRowProvider _logRowProvider;
        private LogFile SharedQueue { get; set; }
        private List<Task<DeviceDetails>> BackgroundProcessingBatches { get; set; }
        public ETLBaseMapReduce(ILogRowProvider fileFormatProvider)
        {
            _logRowProvider = fileFormatProvider;
            BackgroundProcessingBatches = new List<Task<DeviceDetails>>();
        }

        public async Task<IEnumerable<DeviceDetails>> LoadAndTriggerProcessFileAsync(Stream stream)
        {
            stream.Position = 0;
            using (var reader = new StreamReader(stream))
            {
                string line = reader.ReadLine();
                var environmentContext = _logRowProvider.LoadEnvironmentReference(line);
                while ((line = reader.ReadLine()) != null)
                {
                    var logRow = _logRowProvider.Load(line);
                    if (logRow.Type == RowType.Header)
                    {
                        if (SharedQueue != null)
                        {
                            ProcessCurrentDelayed(BackgroundProcessingBatches, environmentContext, SharedQueue);
                        }
                        SharedQueue = new LogFile();
                        SharedQueue.Queue.Enqueue(logRow);
                    }
                    else
                    {
                        try
                        {
                            SharedQueue.Queue.Enqueue(logRow);
                        }
                        catch (Exception e)
                        {
                            throw new InvalidLogException("Header is missing", e);
                        }
                    }
                }

                ProcessCurrentDelayed(BackgroundProcessingBatches, environmentContext, SharedQueue);
            }

            var results = await Task.WhenAll(BackgroundProcessingBatches);
            return results;
        }

        public virtual void ProcessCurrentDelayed(List<Task<DeviceDetails>> taskQueue, EnvironmentReference environmentContext, LogFile file)
        {
            taskQueue.Add(ProcessBatch(new LogFile(environmentContext, file.Queue)));
        }

        public virtual Task<DeviceDetails> ProcessBatch(LogFile measures)
        {
            throw new NotImplementedException();
        }
    }
}
