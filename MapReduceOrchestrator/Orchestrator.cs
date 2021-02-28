using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Providers;
using MapReduceOrchestrator.Entities;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace MapReduceOrchestrator
{
    public static class Orchestrator
    {
        [FunctionName(nameof(LogFileStoredHandler))]
        public static async Task LogFileStoredHandler(
            [BlobTrigger("%LogStoragePath%/{name}", Connection = "LogStorageConnectionString")] CloudBlockBlob blob,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            await starter.StartNewAsync<string>(nameof(RunOrchestrator), blob.Name);

            log.LogInformation($"Started MapReduce orchestration.");
        }

        [FunctionName(nameof(RunOrchestrator))]
        public static async Task<IEnumerable<DeviceDetails>> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            [Blob("%LogStoragePath%", Connection = "LogStorageConnectionString")] CloudBlobContainer blobContainer,
            ILogger log)
        {

            //1. get the file
            var blobName = context.GetInput<string>();
            var blobReference = blobContainer.GetBlockBlobReference(blobName);
            var dataStream = new MemoryStream();

            log.LogInformation($"Downloading of the log file '{blobName}'...");
            blobReference.DownloadToStream(dataStream);

            //2. process the file
            var serverlessFileMapper = new ServerlessMapReduce(context, 
                AnomaliesDetectorContainer.Instance.Resolve<ILogRowProvider>(),
                blobName);

            var results = await serverlessFileMapper.LoadAndTriggerProcessFileAsync(dataStream);

            //3. store the process result
            await context.CallActivityAsync<bool>(nameof(StoreResult), new FunctionDTO<IEnumerable<DeviceDetails>> { Input = results, OriginalFileDrop = blobName, OrchestrationId = context.InstanceId });

            return results;
        }

        [FunctionName(nameof(StoreResult))]
        public static bool StoreResult([ActivityTrigger] FunctionDTO<IEnumerable<DeviceDetails>> input,
            [Blob("%DeviceLogPath%", FileAccess.Write, Connection = "LogStorageConnectionString")] CloudBlobContainer targetBlob,
            ILogger log)
        {
            log.LogInformation("Storing process results...");

            var fileName = $"{DateTime.UtcNow.ToString("yy-MM-dd")}/{input.OriginalFileDrop}/complete/result.json";
            var blobBlock = targetBlob.GetBlockBlobReference(fileName);
            blobBlock.UploadText(Newtonsoft.Json.JsonConvert.SerializeObject(input.Input));

            return true;
        }
    }
}