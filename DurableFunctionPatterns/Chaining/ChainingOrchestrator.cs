using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;

namespace DurableFunctionPatterns.Chaining;

public class ChainingOrchestrator
{
    [Function(nameof(ChainingOrchestrator))]
    public static async Task<object> RunOrchestrator(
        [OrchestrationTrigger] TaskOrchestrationContext context)
    {
        var intput = context.GetInput<ChainingParameter>();
        
        await context.CallActivityAsync(nameof(ChainingAcitivity), intput.Name);
        await context.CallActivityAsync(nameof(ChainingAcitivity), intput.Company);
        await context.CallActivityAsync(nameof(ChainingAcitivity), intput.Description);
        
        return "Chaining completed successfully";
    }
}