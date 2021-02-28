# Anomalies detector
## Summary
This Widget365 anomalies detector is inspired to Hadoop MapReduce to handle heavy ETL batch processing.

Log files are decomposed and processed in parallel following the Fan-Out pattern. You can replay every log file drop.

I created 2 implementations for this process. A _(1)_ Serverless option based on a durable orchestration process. This enables fault tolerance and replay (results are checkpointed on a storage account).

A second implementation _(2)_ is simply based on multithreading.

Solution comes with 3 projects:
- `Common`
- `MapReduceOrchestrator`
- `Integration`


### Common

Exposes common entities, Windsor installers and some logic to parse log files.

- `ETLBaseMapReduce` is the main abstract class performing map/reduce operations on the primary dropped log file
- `SynchronizedMapReduce` is `ETLBaseMapReduce` default implementation. Runs async.
- `LogFile` exposes a generic log file structure (as dropped). `SingleSensorReadingsLog` is a map on `LogFile`)
- `ISensor` exposes some signatures to handle existing/new sensors
- `IRankingReducer` exposes some signatures to validate a sensor measures. Every sensor subscribe to a RankingReducer
- `IRankingReducerFactory` identifies and applies the right reducer to `SingleSensorReadingsLog` instances.

### MapReduceOrchestrator

Runs on Azure Durable Functions. When a log file is droppen on a storage account, orchestrator function is triggered.
Multiple checkpoints are created so that the process is resolved in 6 different functions:

- `LogFileStoredHandler` triggered when a log file is dropped in a storage account. Invokes `RunOrchestrator`
- `RunOrchestrator` download the dropped file. Identifies the devices and invokes a sub-orchestrator `ProcessBatchOrchestrator` which handles a single log partition
- `ProcessBatchOrchestrator` is a second-level orchestrator. Stores a device log (`StoreDeviceLog`) and processes it (`ProcessDeviceLog`). Results are waited by the 1st-level orchestrator
- `StoreResult` stores the final result (the json file)

### Integration

Contains the integration tests I wrote while writing the code. 

> You can find an integration test for everything I wrote that was not _right the first time_.

## Help

### Setup

To setup your local storage, apply this settings to `local.settings.json`:
```
{
  "IsEncrypted": false,
  "Values": {
    "LogStorageConnectionString": "DefaultEndpointsProtocol=https;AccountName=365widgetslogstoragedev;...",
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "LogStoragePath": "log-lake",
    "DeviceLogPath": "map-reduce-lake"
  }
}
```

### Naming conventions

- input files are expect to be dropped in: `log-lake` container;
- output file are uploaded in `map-reduce-lake` container (a _processing_ and _complete_ folder are created for each file)

### To do

- better error handling
- edge-case scenario tests
- sas token for storage account
