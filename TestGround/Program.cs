using System;
using System.Data.SqlClient;
using System.Text.Json;
using System.Threading.Tasks;

namespace TestGround
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var product = new Product();
            var jsonMsg = JsonSerializer.Serialize(new Product());
            Console.WriteLine($"message in Json: {jsonMsg}");

            var msg = JsonSerializer.Deserialize<Product>(jsonMsg);
            Console.WriteLine(msg.Name);

            var str = "Server=tcp:jtn874t9r4.database.windows.net,1433;Initial Catalog=esiteDB;Persist Security Info=False;User ID=pwang009;Password=Kostland2277;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();
                var sqlcommand = "INSERT INTO message (Company, Name) VALUES (@Company, @Name)";
                using (SqlCommand cmd = new SqlCommand(sqlcommand, conn))
                {
                    // Execute the command and log the # rows affected.
                    cmd.Parameters.AddWithValue("@Company", msg.Company);
                    cmd.Parameters.AddWithValue("@Name", msg.Name);
                    //{ queueItem.Id}, { product.Company}, { product.Name}, { product.Price}
                    try
                    {
                        var rows = await cmd.ExecuteScalarAsync();
                        //var word = rows > 1 ? "have" : "has";
                        //Console.WriteLine($"{rows} row {word} been inserted.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Something went wrong {ex}");
                    }

                }
            }
        }
    }

    public class Product
    {
        public Product()
        {
            Company = "Lotus, Inc";
            Name = "ATS 909l (White)";
            Price = 799.99M;
        }
        public string Company { get; private set; }
        public string Name { get; private set; }
        public decimal Price { get; private set; }

    }
}
