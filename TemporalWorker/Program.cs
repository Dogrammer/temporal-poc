using Temporalio.Client;
using Temporalio.Worker;

var temporalAddress = Environment.GetEnvironmentVariable("TEMPORAL__ADDRESS") ?? "localhost:7233";
var taskQueue = Environment.GetEnvironmentVariable("TASK_QUEUE") ?? "housing-loans";

Console.WriteLine($"[Worker] Connecting to Temporal at {temporalAddress}");
var client = await TemporalClient.ConnectAsync(new TemporalClientConnectOptions(temporalAddress));

var activities = new Activities.LoanActivities();

using var worker = new TemporalWorker(
    client,
    new TemporalWorkerOptions(taskQueue)
        .AddWorkflow<Workflows.HousingLoanWorkflow>()
        .AddActivity(activities.SayHelloAsync)
        .AddActivity(activities.CheckDocumentsAsync)
        .AddActivity(activities.RiskScoreAsync)
        .AddActivity(activities.NotifyAsync));

Console.WriteLine($"[Worker] Listening on task queue '{taskQueue}'");
await worker.ExecuteAsync(CancellationToken.None);