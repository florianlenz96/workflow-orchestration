using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace DurableFunctionPatterns.Chaining;

public class ChainingAcitivity
{
    [Function(nameof(ChainingAcitivity))]
    public static string Run(
        [ActivityTrigger] string value,
        FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger("ChainingAcitivity");
        logger.LogInformation($"Processing value: {value}");
        return $"Processed value: {value}";
    }
}