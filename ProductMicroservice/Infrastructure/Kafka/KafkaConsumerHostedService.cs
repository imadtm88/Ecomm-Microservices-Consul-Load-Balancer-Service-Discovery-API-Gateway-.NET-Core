using Application.Contracts;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Kafka
{
    public class KafkaConsumerHostedService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly KafkaSettings _settings;

        public KafkaConsumerHostedService(IServiceProvider provider, IOptions<KafkaSettings> options)
        {
            _serviceProvider = provider;
            _settings = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var config = new ConsumerConfig
            {
                GroupId = _settings.GroupId,
                BootstrapServers = _settings.BootstrapServers,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            consumer.Subscribe(_settings.Topic);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = consumer.Consume(stoppingToken);
                    // Désérialiser le message en OrderCreatedEvent
                    var orderEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(result.Message.Value);

                    Console.WriteLine($"[Kafka] Order received: {orderEvent?.Id}");
                    // TODO: Appliquer la logique, par exemple mettre à jour le stock en fonction de orderEvent.Items
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Kafka Error: {ex.Message}");
                }

                await Task.Delay(100); // pour éviter le CPU spinning
            }
        }
    }
}
