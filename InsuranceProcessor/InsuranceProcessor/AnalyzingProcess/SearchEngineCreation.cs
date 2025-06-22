using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace InsuranceProcessor.AnalyzingProcess;

public class SearchEngineCreation
{
    [Function(nameof(SearchEngineCreation))]
    public static void Run(
        [ActivityTrigger] Parameter parameter,
        FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger("SearchEngineCreation");
        logger.LogWarning("Use search engine...");
        
        Thread.Sleep(new Random().Next(789, 2568));
    }
}