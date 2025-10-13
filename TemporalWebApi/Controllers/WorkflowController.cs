using Microsoft.AspNetCore.Mvc;
using Temporalio.Client;
using TemporalPOC.Contracts.Workflows;
using TemporalPOC.Contracts.Models;

namespace TemporalWebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkflowController : ControllerBase
{
    private readonly ITemporalClient _temporalClient;
    private readonly ILogger<WorkflowController> _logger;

    public WorkflowController(ITemporalClient temporalClient, ILogger<WorkflowController> logger)
    {
        _temporalClient = temporalClient;
        _logger = logger;
    }

    [HttpPost("start-loan")]
    public async Task<IActionResult> StartLoanWorkflow([FromBody] StartLoanRequest request)
    {
        try
        {
            var loanId = string.IsNullOrEmpty(request.LoanId) 
                ? Guid.NewGuid().ToString("N").Substring(0, 8) 
                : request.LoanId;
            
            var workflowId = $"kredit-{loanId}";
            var taskQueue = request.TaskQueue ?? "housing-loans";

            _logger.LogInformation("Starting workflow for loan {LoanId}", loanId);

            var handle = await _temporalClient.StartWorkflowAsync(
                (HousingLoanWorkflow wf) => wf.RunAsync(loanId),
                new(id: workflowId, taskQueue: taskQueue));

            _logger.LogInformation("Started workflow {WorkflowId}", handle.Id);

            return Ok(new
            {
                workflowId = handle.Id,
                loanId = loanId,
                status = "started"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting workflow");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("status/{workflowId}")]
    public async Task<IActionResult> GetWorkflowStatus(string workflowId)
    {
        try
        {
            var handle = _temporalClient.GetWorkflowHandle(workflowId);
            var description = await handle.DescribeAsync();

            return Ok(new
            {
                workflowId = workflowId,
                status = description.Status.ToString(),
                startTime = description.StartTime,
                executionTime = description.ExecutionTime
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting workflow status");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("result/{workflowId}")]
    public async Task<IActionResult> GetWorkflowResult(string workflowId)
    {
        try
        {
            var handle = _temporalClient.GetWorkflowHandle(workflowId);
            var result = await handle.GetResultAsync<LoanWorkflowResult>();

            return Ok(new
            {
                workflowId = workflowId,
                loanId = result.LoanId,
                message = result.Message,
                riskScore = result.RiskScore,
                riskRating = result.RiskRating,
                provider = result.Provider,
                apiCallDurationMs = result.ApiCallDurationMs
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting workflow result");
            return StatusCode(500, new { error = ex.Message });
        }
    }
}


public class StartLoanRequest
{
    public string? LoanId { get; set; }
    public string? TaskQueue { get; set; }
}

