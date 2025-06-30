using DurableFunctionPatterns.HumanInteraction;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask.Client;

namespace DurableFunctionPatterns.AsyncHttp;

public class StartAsyncHttp
{
    [Function("StartAsyncHttp")]
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
        
        var response = await client
            .CreateCheckStatusResponseAsync(req, instanceId);

        return response;
    }
}