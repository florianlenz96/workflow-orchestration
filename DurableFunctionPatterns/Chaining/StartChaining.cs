using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask.Client;

namespace DurableFunctionPatterns.Chaining;

public class StartChaining
{
    [Function("StartChaining")]
    public static async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")]
        HttpRequestData req,
        [DurableClient] DurableTaskClient client,
        FunctionContext executionContext)
    {
        var input = new ChainingParameter
        {
            Name = "John Doe",
            Description = "Sample description",
            Company = "Sample company",
        };
        
        await client.ScheduleNewOrchestrationInstanceAsync(
            nameof(ChainingOrchestrator), input);

        return req.CreateResponse(HttpStatusCode.OK);
    }
}