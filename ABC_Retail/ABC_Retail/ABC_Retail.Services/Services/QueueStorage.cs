using Azure.Storage.Queues;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ABC_Retail.Services
{
    public class QueueStorage
    {
        private readonly QueueClient _queueClient;

        public QueueStorage(string connectionString, string queueName = "orders")
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Azure Storage connection string cannot be null or empty.", nameof(connectionString));

            if (string.IsNullOrWhiteSpace(queueName))
                throw new ArgumentException("Queue name cannot be null or empty.", nameof(queueName));

            queueName = queueName.ToLower();
            _queueClient = new QueueClient(connectionString, queueName);
            _queueClient.CreateIfNotExists();
        }

        // Send a message to the queue
        public async Task SendMessageAsync(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Message cannot be null or empty.", nameof(message));

            await _queueClient.SendMessageAsync(message);
        }

        // Get approximate message count
        public async Task<int> GetMessageCountAsync()
        {
            var properties = await _queueClient.GetPropertiesAsync();
            return properties.Value.ApproximateMessagesCount;
        }

        // Read messages from the queue without deleting
        public async Task<List<string>> PeekMessagesAsync(int maxMessages = 20)
        {
            var messages = new List<string>();
            var peekedMessages = await _queueClient.PeekMessagesAsync(maxMessages);
            foreach (var msg in peekedMessages.Value)
            {
                messages.Add(msg.MessageText);
            }
            return messages;
        }
    }
}
