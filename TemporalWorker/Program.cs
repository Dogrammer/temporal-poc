using Temporalio.Client;
using Temporalio.Worker;
using TemporalPOC.Contracts.Workflows;
using TemporalPOC.Contracts.Activities;

var temporalAddress = Environment.GetEnvironmentVariable("TEMPORAL__ADDRESS") ?? "localhost:7233";
var taskQueue = Environment.GetEnvironmentVariable("TASK_QUEUE") ?? "housing-loans";

Console.WriteLine($"[Worker] Connecting to Temporal at {temporalAddress}");

// Retry connection with exponential backoff
TemporalClient client = null;
var maxRetries = 20;
var delay = TimeSpan.FromSeconds(5);

for (int attempt = 1; attempt <= maxRetries; attempt++)
{
    try
    {
        Console.WriteLine($"[Worker] Connection attempt {attempt}/{maxRetries}");
        client = await TemporalClient.ConnectAsync(new TemporalClientConnectOptions(temporalAddress));
        Console.WriteLine($"[Worker] Successfully connected to Temporal");
        break;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[Worker] Connection attempt {attempt} failed: {ex.Message}");
        
        if (attempt == maxRetries)
        {
            Console.WriteLine($"[Worker] Failed to connect after {maxRetries} attempts. Exiting.");
            throw;
        }
        
        Console.WriteLine($"[Worker] Waiting {delay.TotalSeconds} seconds before retry...");
        await Task.Delay(delay);
        delay = TimeSpan.FromSeconds(Math.Min(delay.TotalSeconds * 1.5, 30)); // Exponential backoff, max 30 seconds
    }
}

var activities = new LoanActivities();

using var worker = new TemporalWorker(
    client,
    new TemporalWorkerOptions(taskQueue)
        .AddWorkflow<HousingLoanWorkflow>()
        .AddActivity(activities.SayHelloAsync)
        .AddActivity(activities.CheckDocumentsAsync)
        .AddActivity(activities.RiskScoreAsync)
        .AddActivity(activities.NotifyAsync));

Console.WriteLine($"[Worker] Listening on task queue '{taskQueue}'");
await worker.ExecuteAsync(CancellationToken.None);