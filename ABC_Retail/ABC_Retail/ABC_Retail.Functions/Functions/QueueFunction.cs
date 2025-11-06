using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ABC_Retail.Functions
{
    public static class QueueProcessor
    {
        [Function("QueueProcessor")]
        public static void Run(

           [QueueTrigger("orders", Connection = "AzureWebJobsStorage")] string myQueueItem,
            FunctionContext context)
        {
            var logger = context.GetLogger("QueueProcessor");

            // This log proves the function read the message successfully
            logger.LogInformation($"C# Queue trigger function processed message: {myQueueItem}");

            // The function ends here, indicating the message was successfully processed and will be deleted from the queue.
        }
    }
}