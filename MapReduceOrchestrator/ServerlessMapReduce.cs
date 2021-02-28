using Common;
using Common.MapReduce;
using Common.Providers;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MapReduceOrchestrator
{
    public class ServerlessMapReduce : ETLBaseMapReduce
    {
        public static IDurableOrchestrationContext Context { get; set; }
        public static string OriginalFileDrop { get; set; }
        public ServerlessMapReduce(IDurableOrchestrationContext context,
            ILogRowProvider logRowProvider, string originalFileDrop) : base(logRowProvider) {
            Context = context;
            OriginalFileDrop = originalFileDrop;
        }

        [FunctionName(nameof(ProcessBatchOrchestrator))]
        public static async Task<DeviceDetails> ProcessBatchOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            ILogger log)
        {
            log.LogInformation("Starting sub-orchestration...");
            LogFile measures = context.GetInput<LogFile>();

            //1. store device file (as SingleReading)
            var chunkFile = await context.CallActivityAsync<string>(nameof(StoreDeviceLog), measures);

            //2. process device file (from SingleReading)
            var result = await context.CallActivityAsync<DeviceDetails>(nameof(ProcessDeviceLog), chunkFile); ;
            
            return result;
        }

        [FunctionName(nameof(StoreDeviceLog))]
        public static string StoreDeviceLog([ActivityTrigger] IDurableActivityContext context, 
            [Blob("%DeviceLogPath%", FileAccess.Write, Connection = "LogStorageConnectionString" )] CloudBlobContainer targetBlob,
            ILogger log)
        {
            var inputFile = context.GetInput<LogFile>();
            log.LogInformation($"Storing log. {inputFile.Queue.Count} rows to store.");

            var heading = inputFile.Queue.Peek();
            var fileName = $"{DateTime.UtcNow.ToString("yy-MM-dd")}/{OriginalFileDrop}/processing/{heading.Name}.txt";
            var blobBlock = targetBlob.GetBlockBlobReference(fileName);
            using (var stream = inputFile.ToSingleLogReadingsStream())
            {
                blobBlock.UploadFromStream(stream);

            }
            return fileName;
        }
        
        
        [FunctionName(nameof(ProcessDeviceLog))]
        public static DeviceDetails ProcessDeviceLog([ActivityTrigger] string fileName,
            [Blob("%DeviceLogPath%", FileAccess.Read, Connection = "LogStorageConnectionString")] CloudBlobContainer sourceBlob,
            ILogger log)
        {
            log.LogInformation($"Analyze measures log: {fileName} ");

            var file = sourceBlob.GetBlockBlobReference(fileName);
            var stream = new MemoryStream();
            file.DownloadToStream(stream);

            var rankingManager = AnomaliesDetectorContainer.Instance.Resolve<IRankingManager>();
            var deviceReadings = rankingManager.FromSingleSensorReadings(stream);
            var ranking = rankingManager.GetRanking(deviceReadings);
            return ranking;
        }

        public override void ProcessCurrentDelayed(List<Task<DeviceDetails>> taskQueue, EnvironmentReference environmentContext, LogFile file)
        {
            var task = Context.CallSubOrchestratorAsync<DeviceDetails>(nameof(ProcessBatchOrchestrator), new LogFile(environmentContext, file.Queue));
            taskQueue.Add(task);
        }

    }
}
