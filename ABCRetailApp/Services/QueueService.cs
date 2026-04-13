using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;

namespace ABCRetailApp.Services
{
    public class QueueService
    {
        private readonly QueueClient _client;

        public QueueService(IConfiguration config)
        {
            _client = new QueueClient(
                config["AzureStorage:ConnectionString"], "orders",
                new QueueClientOptions
                {
                    MessageEncoding = QueueMessageEncoding.Base64
                });
            _client.CreateIfNotExists();
        }

        public async Task SendMessageAsync(string message) =>
            await _client.SendMessageAsync(message);

        public async Task<List<PeekedMessage>> GetMessagesAsync()
        {
            var result = await _client.PeekMessagesAsync(maxMessages: 10);
            return result.Value.ToList();  // ← returns List<PeekedMessage>
        }

        public async Task ClearQueueAsync() =>
            await _client.ClearMessagesAsync();
    }
}