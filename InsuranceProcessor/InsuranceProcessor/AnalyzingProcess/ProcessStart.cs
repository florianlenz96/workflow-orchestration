using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace InsuranceProcessor.AnalyzingProcess;

public class ProcessStart
{
    [Function("ProcessStart")]
    public static async Task<HttpResponseData> HttpStart(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")]
        HttpRequestData req,
        [DurableClient] DurableTaskClient client,
        FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger("ProcessStart");
        
        // Read JSON request body
        using var reader = new StreamReader(req.Body);
        var requestBody = await reader.ReadToEndAsync();
        
        // Deserialize JSON to get the parameters
        var requestData = JsonSerializer.Deserialize<RequestData>(requestBody);
        
        if (requestData == null)
        {
            var errorResponse = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
            await errorResponse.WriteStringAsync("Invalid request data");
            return errorResponse;
        }
        
        var orchestrationInput = new Parameter
        {
            Name = requestData.Name,
            Description = requestData.Description,
            Image = requestData.Image
        };
        
        var instanceId = await client.ScheduleNewOrchestrationInstanceAsync(nameof(Orchestrator), orchestrationInput);

        logger.LogWarning("Started orchestration with ID = '{instanceId}'.", instanceId);

        #region Async HTTP Api pattern
        
        var response = await client
            .CreateCheckStatusResponseAsync(req, instanceId);

        #endregion

        return response;
    }
}

// Request data model to match the frontend JSON structure
public class RequestData
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
}