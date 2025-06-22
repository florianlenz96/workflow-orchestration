using HttpMultipartParser;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;

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
        
        var parser = await MultipartFormDataParser.ParseAsync(req.Body);
        var description = parser.GetParameterValue("description");
        var imageFile = parser.Files.FirstOrDefault(f => f.Name == "image");
        var ms = new MemoryStream();
        await imageFile.Data.CopyToAsync(ms);
        var base64Image = Convert.ToBase64String(ms.ToArray());
        
        var processId = Guid.NewGuid().ToString();
        var orchestrationInput = new Parameter
        {
            Name = processId,
            Description = description,
            Image = base64Image
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