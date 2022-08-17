using System;

namespace KafkaProducer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var producer = new Producer();

            Console.ReadKey();
        }
    }
}
