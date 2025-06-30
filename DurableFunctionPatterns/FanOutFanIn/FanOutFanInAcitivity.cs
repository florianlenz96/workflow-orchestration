using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace DurableFunctionPatterns.FanOutFanIn;

public class FanOutFanInAcitivity
{
    [Function(nameof(FanOutFanInAcitivity))]
    public static string Run(
        [ActivityTrigger] string value,
        FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger("FanOutFanInAcitivity");
        logger.LogInformation($"Processing value: {value}");
        return $"Processed value: {value}";
    }
}