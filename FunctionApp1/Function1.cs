using FunctionApp1.Model;
using Microsoft.Azure.Storage.Queue;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace FunctionApp1
{
    public class Function1
    {
        [FunctionName("Function1")]
        [StorageAccount("StorageConnectionString")]
        public static async Task Run([QueueTrigger("myqueue-items")] CloudQueueMessage queueItem, ILogger log)
        {
            log.LogInformation($"message id: {queueItem.Id}");
            log.LogInformation($"message inserted at: {queueItem.InsertionTime}");
            log.LogInformation($"C# Queue trigger function processed: {queueItem.AsString}");
            await Utils<Product>.SaveMessage(log, queueItem.AsString, queueItem.Id);
        }
    }
}
