using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace InsuranceProcessor.AnalyzingProcess;

public class ImageAnalyzation
{
    [Function(nameof(ImageAnalyzation))]
    public static string Run(
        [ActivityTrigger] Parameter parameter,
        FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger("ImageAnalyzation");
        logger.LogWarning("Analyze image...");
        
        Thread.Sleep(new Random().Next(789, 2568));
        
        if (parameter.Description.Contains("blau", StringComparison.OrdinalIgnoreCase))
        {
            return "{\n   \"decision\":false,\n   \"manualProcessing\":true,\n   \"description\":\"Das Fahrzeug im Bild ist deutlich silber lackiert, während der Kunde angibt, sein Auto sei blau. Die Farbabweichung spricht gegen die Übereinstimmung.\"\n}";
        }
        
        return "{\n   \"decision\":true,\n   \"manualProcessing\":false,\n   \"description\":\"Das Fahrzeug im Bild ist silber lackiert und der Kunde gibt dies an. Es spricht nichts gegen die Beschreibung und das eingesendete Bild.\"\n}";
    }
}