using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PlatformEngineer.Worker.Models;

namespace PlatformEngineer.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ServiceBusClient _sbClient;
        private readonly ServiceBusReceiver _sbReceiver;
        private readonly TableClient _tableClient;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;

            var serviceBusConnection = Environment.GetEnvironmentVariable("AZURE_SERVICEBUS_CONNECTION_STRING") ?? string.Empty;
            var queueName = Environment.GetEnvironmentVariable("SERVICEBUS_QUEUE_NAME") ?? "workitems";
            var tableConnection = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING") ?? "UseDevelopmentStorage=true";
            var tableName = Environment.GetEnvironmentVariable("TABLE_NAME") ?? "processeditems";

            if (string.IsNullOrWhiteSpace(serviceBusConnection))
            {
                throw new InvalidOperationException("AZURE_SERVICEBUS_CONNECTION_STRING environment variable is not set. Please set it before running the worker.");
            }

            _sbClient = new ServiceBusClient(serviceBusConnection);
            _sbReceiver = _sbClient.CreateReceiver(queueName);

            var tableService = new TableServiceClient(tableConnection);
            _tableClient = tableService.GetTableClient(tableName);
            _tableClient.CreateIfNotExists();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker running");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var msg = await _sbReceiver.ReceiveMessageAsync(TimeSpan.FromSeconds(5), cancellationToken: stoppingToken);
                    if (msg != null)
                    {
                        var messageText = msg.Body.ToString();
                        var work = JsonSerializer.Deserialize<WorkItem>(messageText);

                        if (work != null)
                        {
                            _logger.LogInformation("Processing {Id}", work.Id);

                            // Simulate processing
                            await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);

                            // Store processed state in Table Storage
                            var entity = new TableEntity(work.Id.ToString(), Guid.NewGuid().ToString())
                            {
                                ["ApplicationName"] = work.ApplicationName,
                                ["Environment"] = work.Environment,
                                ["Status"] = "Processed",
                                ["CreatedAt"] = work.CreatedAt,
                                ["ProcessedAt"] = DateTime.UtcNow
                            };

                            await _tableClient.AddEntityAsync(entity, stoppingToken);

                            // Complete the message
                            await _sbReceiver.CompleteMessageAsync(msg, stoppingToken);
                        }
                    }
                    else
                    {
                        await Task.Delay(500, stoppingToken);
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing queue");
                    await Task.Delay(1000, stoppingToken);
                }
            }
        }
    }
}
