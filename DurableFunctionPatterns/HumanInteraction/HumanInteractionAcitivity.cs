using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace DurableFunctionPatterns.HumanInteraction;

public class HumanInteractionAcitivity
{
    [Function(nameof(HumanInteractionAcitivity))]
    public static string Run(
        [ActivityTrigger] HumanInteractionParameter value,
        FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger("HumanInteractionAcitivity");
        logger.LogInformation($"Processing value: {value}");
        return $"Processed value: {value}";
    }
}