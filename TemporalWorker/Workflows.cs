using Temporalio.Workflows;

namespace Workflows;

[Workflow]
public class HousingLoanWorkflow
{
    [WorkflowRun]
    public async Task<LoanWorkflowResult> RunAsync(string loanId)
    {
        var activityOptions = new ActivityOptions
        {
            StartToCloseTimeout = TimeSpan.FromSeconds(30)
        };

        var hello = await Workflow.ExecuteActivityAsync(
            (Activities.LoanActivities activities) => activities.SayHelloAsync(loanId),
            activityOptions);
        
        await Workflow.ExecuteActivityAsync(
            (Activities.LoanActivities activities) => activities.CheckDocumentsAsync(loanId),
            activityOptions);
        
        var riskScore = await Workflow.ExecuteActivityAsync(
            (Activities.LoanActivities activities) => activities.RiskScoreAsync(loanId),
            activityOptions);
        
        await Workflow.ExecuteActivityAsync(
            (Activities.LoanActivities activities) => activities.NotifyAsync(loanId, $"Loan {loanId} processed with score {riskScore.Score}"),
            activityOptions);
        
        return new LoanWorkflowResult(
            LoanId: loanId,
            Message: hello,
            RiskScore: riskScore.Score,
            RiskRating: riskScore.Rating,
            Provider: riskScore.Provider,
            ApiCallDurationMs: riskScore.ApiCallDurationMs
        );
    }
}

public record LoanWorkflowResult(
    string LoanId,
    string Message,
    int RiskScore,
    string RiskRating,
    string Provider,
    long ApiCallDurationMs
);
