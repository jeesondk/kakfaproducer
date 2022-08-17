using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading;
using Confluent.Kafka;

namespace KafkaProducer
{
    internal class Producer
    {
        private readonly ProducerConfig _config;
        Action<DeliveryReport<Null, string>> deliveryHandler = DeliveryHandler;
        private readonly CancellationTokenSource cts;

        private Guid parrentEventId = Guid.Empty;

        public Producer()
        {
           this._config  = new ProducerConfig
            {
                BootstrapServers = "localhost:29092,localhost:39092",
                ClientId = "ProducerExample01",
                Partitioner = Partitioner.Random,
           };

            cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) => {
                e.Cancel = true; // prevent the process from terminating.
                cts.Cancel();
            };


            Run();

        }

        private void Run()
        {
            using(var producer = new ProducerBuilder<Null, string>(_config).Build())
            {
                Console.WriteLine("Producer working...");

                while (!cts.Token.IsCancellationRequested)
                {
                    
                    Guid eventId = Guid.NewGuid();
                    if(parrentEventId == Guid.Empty)
                        parrentEventId = eventId;

                    var timeEvent = new TimeStampRecord { EventId = eventId, ParentEvent = parrentEventId, TimeStamp = DateTimeOffset.UtcNow };
                    var message = JsonSerializer.Serialize<TimeStampRecord>(timeEvent);

                    producer.Produce("HeartBeats2", new Message<Null, string> { Value = message }, deliveryHandler);
                    parrentEventId = eventId;



                    Thread.Sleep(1000);
                }

                producer.Flush(cts.Token);


                Console.WriteLine("Producer done...");
            }
        }

        public static void DeliveryHandler(DeliveryReport<Null, string> deliveryReport)
        {
            Console.WriteLine($"Message status: {deliveryReport.Status}, at: {deliveryReport.Topic}, offset: {deliveryReport.Offset}, partition: {deliveryReport.Partition}, topic partition: {deliveryReport.TopicPartition}");
        }
    }
}
