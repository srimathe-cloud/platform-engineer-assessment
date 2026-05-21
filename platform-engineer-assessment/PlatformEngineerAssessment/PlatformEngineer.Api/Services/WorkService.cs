using System.Text.Json;
using Azure.Data.Tables;
using Azure.Messaging.ServiceBus;
using PlatformEngineer.Api.Models;

namespace PlatformEngineer.Api.Services
{
    public class WorkService
    {
        private readonly ServiceBusClient? _sbClient;
        private readonly ServiceBusSender? _sbSender;
        private readonly TableClient _tableClient;
        private int _lastId = 0;

        public WorkService(IConfiguration configuration)
        {
            var serviceBusConnection = configuration["AzureServiceBus:ConnectionString"] ?? Environment.GetEnvironmentVariable("AZURE_SERVICEBUS_CONNECTION_STRING");
            var queueName = configuration["AzureServiceBus:QueueName"] ?? Environment.GetEnvironmentVariable("SERVICEBUS_QUEUE_NAME") ?? "workitems";
            var tableConnection = configuration["AzureStorage:ConnectionString"] ?? Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");
            var tableName = configuration["AzureStorage:TableName"] ?? Environment.GetEnvironmentVariable("TABLE_NAME") ?? "processeditems";

            if (!string.IsNullOrEmpty(serviceBusConnection))
            {
                _sbClient = new ServiceBusClient(serviceBusConnection);
                _sbSender = _sbClient.CreateSender(queueName);
            }

            if (!string.IsNullOrEmpty(tableConnection))
            {
                var tableService = new TableServiceClient(tableConnection);
                _tableClient = tableService.GetTableClient(tableName);
                _tableClient.CreateIfNotExists();
            }
            else
            {
                _tableClient = null!;
            }
        }

        public WorkItem Enqueue(WorkItem item)
        {
            item.Id = Interlocked.Increment(ref _lastId);
            item.CreatedAt = DateTime.UtcNow;
            item.Status = "Pending";

            if (_sbSender != null)
            {
                var text = JsonSerializer.Serialize(item);
                var msg = new ServiceBusMessage(text);
                _sbSender.SendMessageAsync(msg).GetAwaiter().GetResult();
            }

            return item;
        }

        public IReadOnlyCollection<WorkItem> GetQueuedItems()
        {
            // Azure Storage Queue does not provide a straightforward way to list messages; return empty in this simple implementation
            return Array.Empty<WorkItem>();
        }

        public IReadOnlyCollection<WorkItem> GetProcessedItems()
        {
            if (_tableClient == null)
            {
                return Array.Empty<WorkItem>();
            }

            var list = new List<WorkItem>();
            foreach (var entity in _tableClient.Query<TableEntity>())
            {
                var w = new WorkItem
                {
                    Id = int.TryParse(entity.RowKey, out var id) ? id : 0,
                    ApplicationName = entity.GetString("ApplicationName") ?? string.Empty,
                    Environment = entity.GetString("Environment") ?? string.Empty,
                    Status = entity.GetString("Status") ?? "Processed",
                    CreatedAt = entity.GetDateTime("CreatedAt") ?? DateTime.UtcNow,
                    ProcessedAt = entity.GetDateTime("ProcessedAt")
                };

                list.Add(w);
            }

            return list;
        }

        public IReadOnlyCollection<WorkItem> GetAllItems()
        {
            // For simplicity return processed items only
            return GetProcessedItems();
        }
    }
}
