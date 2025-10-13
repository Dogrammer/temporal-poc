using Temporalio.Activities;
using System.Text.Json;
using System.Diagnostics;

namespace Activities;

public interface ILoanActivities
{
    Task<string> SayHelloAsync(string loanId);
    Task CheckDocumentsAsync(string loanId);
    Task<RiskScoreResponse> RiskScoreAsync(string loanId);
    Task NotifyAsync(string loanId, string message);
}

public record RiskScoreResponse(int Score, string Rating, string Provider, long ApiCallDurationMs);

public class LoanActivities : ILoanActivities
{
    [Activity]
    public Task<string> SayHelloAsync(string loanId)
        => Task.FromResult($"Hello, Kredit {loanId}!");

    [Activity]
    public async Task CheckDocumentsAsync(string loanId)
    {
        Console.WriteLine($"[Activity] Checking documents for {loanId}...");
        // await Task.Delay(300); // simulate work
    }

    [Activity]
    public async Task<RiskScoreResponse> RiskScoreAsync(string loanId)
    {
        Console.WriteLine($"[Activity] Calculating risk score for {loanId}...");
        
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            // Call a public API to get random data (simulating credit score check)
            using var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(10);
            
            // Using JSONPlaceholder API as an example (you can replace with any API)
            var userId = Math.Abs(loanId.GetHashCode() % 10) + 1; // Generate user ID from loanId
            
            var apiCallStart = Stopwatch.StartNew();
            var response = await httpClient.GetAsync($"https://jsonplaceholder.typicode.com/users/{userId}");
            response.EnsureSuccessStatusCode();
            
            var userData = await response.Content.ReadAsStringAsync();
            apiCallStart.Stop();
            
            var user = JsonSerializer.Deserialize<JsonElement>(userData);
            
            // Simulate credit score calculation based on API data
            var score = 300 + (userId * 70); // Score between 370-1000
            var rating = score switch
            {
                >= 800 => "Excellent",
                >= 700 => "Good",
                >= 600 => "Fair",
                _ => "Poor"
            };
            
            stopwatch.Stop();
            var duration = apiCallStart.ElapsedMilliseconds;
            
            var result = new RiskScoreResponse(
                Score: score,
                Rating: rating,
                Provider: $"CreditAPI (User: {user.GetProperty("name").GetString()})",
                ApiCallDurationMs: duration
            );
            
            Console.WriteLine($"[Activity] Risk score for {loanId}: {score} ({rating}) - API call took {duration}ms");
            
            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            Console.WriteLine($"[Activity] Error calling API: {ex.Message} (took {stopwatch.ElapsedMilliseconds}ms)");
            // Return a default score if API fails
            return new RiskScoreResponse(650, "Fair", "Default (API unavailable)", stopwatch.ElapsedMilliseconds);
        }
    }

    [Activity]
    public Task NotifyAsync(string loanId, string message)
    {
        Console.WriteLine($"[Activity] Notifying for {loanId}: {message}");
        return Task.CompletedTask;
    }
}
