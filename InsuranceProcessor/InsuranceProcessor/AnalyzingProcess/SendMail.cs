using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace InsuranceProcessor.AnalyzingProcess;

public class SendMail
{
    [Function(nameof(SendMail))]
    public static void Run(
        [ActivityTrigger] bool decision,
        FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger("SendMail");
        logger.LogWarning("send mail...");
        
        Thread.Sleep(new Random().Next(789, 2568));

        logger.LogWarning(decision
            ? "Mail sent successfully, decision is true."
            : "Mail sent successfully, decision is false.");
    }
}