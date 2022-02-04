using FunctionApp1.Model;
using System;
using System.Data.SqlClient;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace FunctionApp1
{
    public class Utils<T> where T : class
    {
        public static async Task SaveMessage(ILogger log, string jsonString, string message_id)
        {
            var product = JsonSerializer.Deserialize<Product>(jsonString);
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
                    cmd.Parameters.AddWithValue("@message_id", message_id);
                    cmd.Parameters.AddWithValue("@Company", product.Company);
                    cmd.Parameters.AddWithValue("@Name", product.Name);
                    cmd.Parameters.AddWithValue("@Price", product.Price);
                    //cmd.Parameters.AddWithValue("@message_id", queueItem.Id);
                    try
                    {
                        var rows = await cmd.ExecuteNonQueryAsync();
                        var words = rows < 2 ? "row has" : "rows have";
                        log.LogInformation($"{rows} {words} been inserted to database!");
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

