using System;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Microsoft.Data.SqlClient;

namespace TicketFunction
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;

        public Function1(ILogger<Function1> logger)
        {
            _logger = logger;
        }

        [Function(nameof(Function1))]
        public async Task Run([QueueTrigger("tickethub", Connection = "AzureWebJobsStorage")] QueueMessage message)
        {
            _logger.LogInformation("✅ Function triggered, starting process.");

            string messageJson = message.MessageText;
            _logger.LogInformation($"📨 Received message: {messageJson}");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            Tickets? ticket = null;
            try
            {
                ticket = JsonSerializer.Deserialize<Tickets>(messageJson, options);
                _logger.LogInformation("✅ Deserialization complete.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error during deserialization: {ex.Message}");
                _logger.LogError($"🔍 Stack trace: {ex.StackTrace}");
                return;
            }

            if (ticket == null)
            {
                _logger.LogError("❌ Ticket is null after deserialization.");
                return;
            }

            _logger.LogInformation("✅ Starting DB insert process.");

            string? connectionString = Environment.GetEnvironmentVariable("SqlConnectionString");
            if (string.IsNullOrEmpty(connectionString))
            {
                _logger.LogError("❌ SQL connection string is missing.");
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    _logger.LogInformation("✅ Database connection opened.");

                    var query = "INSERT INTO dbo.Tickets (ConcertId, Email, Name, Phone, Quantity, CreditCard, Expiration, SecurityCode, Address, City, Province, PostalCode, Country) VALUES (@ConcertId, @Email, @Name, @Phone, @Quantity, @CreditCard, @Expiration, @SecurityCode, @Address, @City, @Province, @PostalCode, @Country)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ConcertId", ticket.ConcertId);
                        cmd.Parameters.AddWithValue("@Email", ticket.Email);
                        cmd.Parameters.AddWithValue("@Name", ticket.Name);
                        cmd.Parameters.AddWithValue("@Phone", ticket.Phone);
                        cmd.Parameters.AddWithValue("@Quantity", ticket.Quantity);
                        cmd.Parameters.AddWithValue("@CreditCard", ticket.CreditCard);
                        cmd.Parameters.AddWithValue("@Expiration", ticket.Expiration);
                        cmd.Parameters.AddWithValue("@SecurityCode", ticket.SecurityCode);
                        cmd.Parameters.AddWithValue("@Address", ticket.Address);
                        cmd.Parameters.AddWithValue("@City", ticket.City);
                        cmd.Parameters.AddWithValue("@Province", ticket.Province);
                        cmd.Parameters.AddWithValue("@PostalCode", ticket.PostalCode);
                        cmd.Parameters.AddWithValue("@Country", ticket.Country);

                        await cmd.ExecuteNonQueryAsync();
                        _logger.LogInformation("✅ Data inserted into database.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error inserting into database: {ex.Message}");
                _logger.LogError($"🔍 Stack trace: {ex.StackTrace}");
            }
        }
    }
}
