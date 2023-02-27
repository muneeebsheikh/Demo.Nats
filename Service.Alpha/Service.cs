using NATS.Client;
using Service.Base;

using NATS.Client.Rx.Ops;
using System.Text;
using Storage;
using Newtonsoft.Json;

namespace Service.Alpha
{

    internal class Service : ServiceBase
    {
        public Service() : base("alpha")
        {

        }

        protected override List<Type> Types => new List<Type> { typeof(ObjectModel) };

        public override async Task RunAsync()
        {
            using var cn = new ConnectionFactory().CreateConnection();
            var observable = NatsHelper.CreateObservable("alpha", cn);

            observable.Subscribe(o => {
                var obj = NatsHelper.GetData(o.Json, o.Type);
                Console.WriteLine($"ALPHA: {obj}");
                //Console.WriteLine($"ALPHA: {NatsHelper.GetData<Model>(o.Data).Message}");
            }, err => Console.WriteLine(err.Message), () => Console.WriteLine("ALPHA: Completed."));

            var cts = new CancellationTokenSource();

            var rnd = new Random();

            await SendMessageAsync(cts.Token, cn);


            Console.ReadKey();
            cts.Cancel();

            //throw new NotImplementedException();
        }

        public void SendMessage(CancellationToken token, IConnection cn)
        {
            Task.Run(async () =>
            {
                var rnd = new Random();
                while (!token.IsCancellationRequested)
                {
                    //var val = rnd.Next(-10, 40).ToString();
                    var val = new Model()
                    {
                        Message = "hello From Alpha to Beta"
                    };
                    var msg = new Msg()
                    {
                        //Data = Encoding.ASCII.GetBytes(val),
                        Data = NatsHelper.GetBytes(val),
                        Reply = "alpha",
                        Subject = "foo"
                    };
                    //cn.Publish(msg);
                    var res = await cn.RequestAsync(msg, 1000, token);
                    //var res = await cn.RequestAsync("beta", msg.Data);




                    Console.WriteLine("REPLY: " + res.Reply);
                    Console.WriteLine("REPLY data length: " + res.Data.Length);
                    Console.WriteLine($"Sent to beta: {val.Message}");


                    //val.Message = "hello to theta";
                    //cn.Publish(new Msg()
                    //{
                    //    Data = GetBytes(val),
                    //    Subject = "theta"
                    //});
                    //Console.WriteLine($"Sent to theta: {val.Message}");

                    await Task.Delay(1000, token);
                }
            }, token);

        }

        public async Task SendMessageAsync(CancellationToken token, IConnection cn)
        {
            #region no Task
            //var token = cts.Token;
            while (!token.IsCancellationRequested)
            {
                //var val = rnd.Next(-10, 40).ToString();
                var val = new Model()
                {
                    Message = "hello From Alpha to Beta"
                };
                var msg = new Msg()
                {
                    //Data = Encoding.ASCII.GetBytes(val),
                    Data = NatsHelper.GetBytes(val),
                    Reply = "alpha",
                    //Subject = "beta"
                };
                Console.WriteLine("Press to send message..." + Environment.NewLine + "1. beta " + Environment.NewLine + "2. worker");
                var key = Console.ReadKey();
                if(key.KeyChar == '1')
                {
                    msg.Subject = "beta";
                }
                else
                {
                    msg.Subject = "connectionWorker";
                }
                cn.Publish(msg);
                //var res = await cn.RequestAsync(msg, 1000 * 2, token);
                //var res = await cn.RequestAsync("beta", msg.Data);

                Console.WriteLine($"Sent to beta: {val.Message}");
                //Console.WriteLine("REPLY: " + res.Reply);
                //Console.WriteLine("REPLY Message: " + GetData<Model>(res.Data).Message);
                //res.Respond(GetBytes(new Model() { Message = "New Message from Alpha"}));
                //Task.Delay(1000, token).Wait();
            }
            #endregion
        }


    }
}
