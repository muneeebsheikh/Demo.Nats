using Storage;

namespace Service.Beta
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Starting Service Beta...");
            if (!NatsHelper.CheckNatsStatus())
            {
                Console.WriteLine("NATS not running");
                return;
            }
            else
                Console.WriteLine("NATS running...");
            await new Service().RunAsync();

        }
    }
}