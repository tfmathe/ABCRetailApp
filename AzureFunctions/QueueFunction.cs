using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AzureFunctions
{
    public class QueueFunction
    {
        private readonly ILogger _logger;
        private readonly QueueClient _queueClient;

        public QueueFunction(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<QueueFunction>();
            var conn = Environment.GetEnvironmentVariable(
                "AzureStorage__ConnectionString");
            _queueClient = new QueueClient(conn, "orders",
                new QueueClientOptions
                {
                    MessageEncoding = QueueMessageEncoding.Base64
                });
            _queueClient.CreateIfNotExists();
        }

        [Function("SendOrderMessage")]
        public async Task<IActionResult> SendOrderMessage(
            [HttpTrigger(AuthorizationLevel.Function,
                "post", Route = "queue/send")] HttpRequest req)
        {
            _logger.LogInformation("SendOrderMessage triggered.");
            using var reader = new StreamReader(req.Body);
            var message = await reader.ReadToEndAsync();
            if (string.IsNullOrWhiteSpace(message))
                message = "New order received";
            await _queueClient.SendMessageAsync(message);
            return new OkObjectResult(
                $"Order message '{message}' sent to queue.");
        }

        [Function("ReadOrderMessages")]
        public async Task<IActionResult> ReadOrderMessages(
            [HttpTrigger(AuthorizationLevel.Function,
                "get", Route = "queue/read")] HttpRequest req)
        {
            _logger.LogInformation("ReadOrderMessages triggered.");
            var messages = await _queueClient.PeekMessagesAsync(
                maxMessages: 10);
            var list = messages.Value.Select(m => new
            {
                MessageId = m.MessageId,
                MessageText = m.MessageText,
                DequeueCount = m.DequeueCount
            }).ToList();
            return new OkObjectResult(list);
        }

        [Function("ProcessOrderMessage")]
        public void ProcessOrderMessage(
            [QueueTrigger("orders",
                Connection = "AzureStorage__ConnectionString")]
            string message)
        {
            _logger.LogInformation(
                $"Processing order from queue: {message}");
        }

        [Function("ClearOrderQueue")]
        public async Task<IActionResult> ClearOrderQueue(
            [HttpTrigger(AuthorizationLevel.Function,
                "delete", Route = "queue/clear")] HttpRequest req)
        {
            _logger.LogInformation("ClearOrderQueue triggered.");
            await _queueClient.ClearMessagesAsync();
            return new OkObjectResult("Order queue cleared successfully.");
        }
    }
}