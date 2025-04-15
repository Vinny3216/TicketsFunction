using System.IO;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Threading.Tasks;

namespace TicketsFunction
{
    public class HttpToQueueFunction
    {
        private readonly ILogger _logger;

        public HttpToQueueFunction(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<HttpToQueueFunction>();
        }

        [Function("HttpToQueueFunction")]
        [QueueOutput("tickethub", Connection = "AzureWebJobsStorage")]
        public async Task<string> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequestData req)
        {
            _logger.LogInformation("HTTP trigger received a request.");

            using var reader = new StreamReader(req.Body);
            string requestBody = await reader.ReadToEndAsync();

            _logger.LogInformation($"Message to queue: {requestBody}");

            return requestBody;
        }
    }
}
