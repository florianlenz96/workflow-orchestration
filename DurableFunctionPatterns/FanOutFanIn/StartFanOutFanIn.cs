using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask.Client;

namespace DurableFunctionPatterns.FanOutFanIn;

public class StartFanOutFanIn
{
    [Function("StartFanOutFanIn")]
    public static async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")]
        HttpRequestData req,
        [DurableClient] DurableTaskClient client,
        FunctionContext executionContext)
    {
        var input = new FanOutFanInParameter
        {
            Name = "John Doe",
            Description = "Sample description",
            Company = "Sample company",
        };
        
        await client.ScheduleNewOrchestrationInstanceAsync(
            nameof(FanOutFanInOrchestrator), input);
        
        return req.CreateResponse(HttpStatusCode.OK);
    }
}