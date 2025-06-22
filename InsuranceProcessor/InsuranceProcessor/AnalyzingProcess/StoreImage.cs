using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace InsuranceProcessor.AnalyzingProcess;

public class StoreImage
{
    [Function(nameof(StoreImage))]
    [BlobOutput("images/{Name}.jpg")]
    public static string Run(
        [ActivityTrigger] Parameter parameter,
        FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger("StoreImage");
        logger.LogWarning("Storing image...");
        
        return parameter.Image;
    }
}