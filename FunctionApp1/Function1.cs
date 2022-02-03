using Microsoft.Azure.Storage.Queue;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Text.Json;
using FunctionApp1.Model;

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

            var product = JsonSerializer.Deserialize<Product>(queueItem.AsString);
            log.LogInformation($"Product Name: {product.Name}");

            var str = Environment.GetEnvironmentVariable("sqlDBConnection");

            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();
                //var text = $"INSERT INTO dbo.message (message_id, Company, Name, Price) VALUES" +
                //" ({queueItem.Id}, {product.Company}, {product.Name}, {product.Price});";

                //var sqlcommand = "INSERT INTO message (message_id, Company, Name) VALUES (@message_id, @Company, @Name)";
                var sqlcommand = "INSERT INTO message (message_id, Company, Name, Price) VALUES (@message_id, @Company, @Name, @Price)";

                using (SqlCommand cmd = new SqlCommand(sqlcommand, conn))
                {
                    // Execute the command and log the # rows affected.
                    cmd.Parameters.AddWithValue("@message_id", queueItem.Id);
                    cmd.Parameters.AddWithValue("@Company", product.Company);
                    cmd.Parameters.AddWithValue("@Name", product.Name);
                    cmd.Parameters.AddWithValue("@Price", product.Price);
                    //cmd.Parameters.AddWithValue("@message_id", queueItem.Id);
                    try
                    {
                        var rows = await cmd.ExecuteNonQueryAsync();
                        log.LogInformation($"{rows} rows were updated");
                    }
                    catch (Exception ex)
                    {
                        log.LogError($"Something went wrong {ex}");
                    }
                }
            }
        }
    }
}
