using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace InsuranceProcessor.AnalyzingProcess;

public class Orchestrator
{
    [Function(nameof(Orchestrator))]
    public static async Task<object> RunOrchestrator(
        [OrchestrationTrigger] TaskOrchestrationContext context)
    {
        var logger = context.CreateReplaySafeLogger(nameof(Orchestrator));
        logger.LogWarning("Starting orchestration...");
        
        var input = context.GetInput<Parameter>();
        
        #region Function chaining pattern
        
        await context.CallActivityAsync(nameof(StoreImage), input);
        
        #endregion
        
        #region Fan-out/fan-in pattern
        
        var searchEngineTask = context.CallActivityAsync(nameof(SearchEngineCreation), input);
        var imageAnalyzationTask = context.CallActivityAsync<string>(nameof(ImageAnalyzation), input);
        await Task.WhenAll(searchEngineTask, imageAnalyzationTask);
        
        var imageAnalyzationResult = await imageAnalyzationTask;
        var result = JObject.Parse(imageAnalyzationResult);

        var manualProcessing = result["manualProcessing"]?.Value<bool>();
        var description = result["description"]?.Value<string>();
        var decision = result["decision"]?.Value<bool>();
        logger.LogWarning("Description: {Description}", description);

        #endregion

        #region Human-interaction pattern

        if (manualProcessing == true)
        {
            // todo - send email to employee with description
            logger.LogWarning("Manual processing required, waiting for human interaction");
            var humanInteractionResult = await context.WaitForExternalEvent<bool>("ProcessApproval");
            if (humanInteractionResult)
            {
                logger.LogWarning("Human interaction approved, continuing process");
                decision = humanInteractionResult;
            }
            else
            {
                logger.LogWarning("Human interaction denied, stopping process");
                decision = false;
            }
        }

        #endregion
        
        #region Function chaining pattern
        
        await context.CallActivityAsync(nameof(SendMail), decision);
        
        #endregion

        return new
        {
            decision = decision,
            description = description,
            manualProcessing = manualProcessing
        };
    }
}