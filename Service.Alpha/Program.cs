using NATS.Client;
using Storage;

namespace Service.Alpha
{
    internal class Program
    {
        //static void Main(string[] args)
        //{
        //    Console.WriteLine("Starting Service Alpha...");

        //    if(!Service.CheckNatsStatus("http://127.0.0.7:8222"))
        //    {
        //        Console.WriteLine("NATS not running");
        //        return;
        //    }
        //    else
        //        Console.WriteLine("NATS running...");
        //    new Service().Run();

        //}


        static async Task Main(string[] args)
        {
            Console.WriteLine("Starting Service Alpha...");

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