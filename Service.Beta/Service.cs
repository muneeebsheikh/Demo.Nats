using NATS.Client;
using Service.Base;
using NATS.Client.Rx.Ops;
using System.Text;
using Newtonsoft.Json;
using Storage;

namespace Service.Beta
{
    internal class Service : ServiceBase
    {
        public Service() : base("beta")
        {

        }
        public override async Task RunAsync()
        {
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;

            using var cn = new ConnectionFactory().CreateConnection();
            var observable_beta = NatsHelper.CreateObservable("beta", cn);
            var observable_theta = NatsHelper.CreateObservable("theta", cn);

            observable_beta.Subscribe(o => {
                var msg = (Model)NatsHelper.GetData(o.Json, o.Type);
                Console.WriteLine($"BETA: {msg.Message}");


                Task.Delay(1000).Wait();

                if (!string.IsNullOrEmpty(o.Reply))
                {
                    Console.WriteLine("Replying to message");
                    o.Respond(NatsHelper.GetBytes(new Model() { Message = "Hello Back from BETA" }));
                }

            }, err => Console.WriteLine(err.Message), () => Console.WriteLine("BETA: Completed."));

            while (!token.IsCancellationRequested)
            {
                await Task.Delay(1000 * 3, token);
                Console.WriteLine("Listening...");
            }


            //Console.ReadKey();
            //throw new NotImplementedException();
        }
    }
}
