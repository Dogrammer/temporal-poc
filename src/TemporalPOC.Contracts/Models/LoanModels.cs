namespace TemporalPOC.Contracts.Models;

public class LoanWorkflowResult
{
    public string LoanId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public int RiskScore { get; set; }
    public string RiskRating { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    public long ApiCallDurationMs { get; set; }
}

public class LoanApplicationRequest
{
    public string LoanId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string ApplicantName { get; set; } = string.Empty;
}

public class RiskAssessmentRequest
{
    public string LoanId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string ApplicantName { get; set; } = string.Empty;
}

public class RiskAssessmentResponse
{
    public int RiskScore { get; set; }
    public string RiskRating { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    public long ApiCallDurationMs { get; set; }
}

public record RiskScoreResponse(int Score, string Rating, string Provider, long ApiCallDurationMs);
