using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;

namespace DurableFunctionPatterns.HumanInteraction;

public class HumanInteractionOrchestrator
{
    [Function(nameof(HumanInteractionOrchestrator))]
    public static async Task<object> RunOrchestrator(
        [OrchestrationTrigger] TaskOrchestrationContext context)
    {
        var intput = context.GetInput<HumanInteractionParameter>();
        var log = context.CreateReplaySafeLogger("HumanInteractionOrchestrator");
        
        await context.CallActivityAsync(nameof(HumanInteractionAcitivity), intput);
        var humanInteractionResult = await context.WaitForExternalEvent<bool>("HumanInteractionEvent");
        if (humanInteractionResult)
        {
            log.LogInformation("Human interaction was successful.");
        }
        else
        {
            log.LogInformation("Human interaction was not successful.");
        }

        return "Human Interaction completed successfully";
    }
}