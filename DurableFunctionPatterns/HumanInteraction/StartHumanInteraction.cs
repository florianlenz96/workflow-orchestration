using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask.Client;

namespace DurableFunctionPatterns.HumanInteraction;

public class StartHumanInteraction
{
    [Function("StartHumanInteraction")]
    public static async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")]
        HttpRequestData req,
        [DurableClient] DurableTaskClient client,
        FunctionContext executionContext)
    {
        var input = new HumanInteractionParameter
        {
            Name = "John Doe",
            Description = "Sample description",
            Company = "Sample company",
        };
        
        var instanceId = await client.ScheduleNewOrchestrationInstanceAsync(
            nameof(HumanInteractionOrchestrator), input);
        
        var response = req.CreateResponse(HttpStatusCode.Accepted);
        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
        await response.WriteStringAsync(instanceId);

        return response;
    }
}