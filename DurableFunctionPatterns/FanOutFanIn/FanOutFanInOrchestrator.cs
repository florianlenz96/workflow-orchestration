using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;

namespace DurableFunctionPatterns.FanOutFanIn;

public class FanOutFanInOrchestrator
{
    [Function(nameof(FanOutFanInOrchestrator))]
    public static async Task<object> RunOrchestrator(
        [OrchestrationTrigger] TaskOrchestrationContext context)
    {
        var intput = context.GetInput<FanOutFanInParameter>();
        
        var task1 = context.CallActivityAsync(nameof(FanOutFanInAcitivity), intput.Name);
        var task2 = context.CallActivityAsync(nameof(FanOutFanInAcitivity), intput.Company);
        var task3 = context.CallActivityAsync(nameof(FanOutFanInAcitivity), intput.Description);
        
        await Task.WhenAll(task1, task2, task3);
        
        return "FanOut/FanIn completed successfully";
    }
}