using Confluent.Kafka;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Kafka
{
    public class KafkaProducer
    {
        private readonly IProducer<Null, string> _producer;
        private readonly KafkaSettings _settings;

        public KafkaProducer(IOptions<KafkaSettings> options)
        {
            _settings = options.Value;

            var config = new ProducerConfig
            {
                BootstrapServers = _settings?.BootstrapServers ?? "localhost:9092"
            };

            _producer = new ProducerBuilder<Null, string>(config).Build();
        }

        public async Task PublishAsync<T>(T message)
        {
            var json = JsonSerializer.Serialize(message);
            await _producer.ProduceAsync(_settings.Topic, new Message<Null, string> { Value = json });
        }
    }   
}
