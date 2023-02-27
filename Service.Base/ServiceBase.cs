using NATS.Client;
using NATS.Client.Rx;
using NATS.Client.Rx.Ops;
using Newtonsoft.Json;
using RestSharp;
using Storage;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Schema;

namespace Service.Base
{
    public abstract class ServiceBase
    {
        protected readonly ConnectionFactory Connectionfactory;
        //protected readonly IConnection Conn;

        //protected EventHandler<MsgHandlerEventArgs> Event;
        protected IAsyncSubscription Subscription;

        protected List<INATSObservable<Msg>> observables;


        //protected const string NATSURL = "nats://localhost:4222";
        //protected const string NATSIP = "127.0.0.1";
        //protected const string NATSPORT = "4222";
        //protected const string NATSMONITORINGPORT = "8222";
        //public abstract void Run();
        public abstract Task RunAsync();

        public ServiceBase(string serviceTopic)
        {
            //Conn = NatsHelper.GetConnection();

            //using var cn = new ConnectionFactory().CreateConnection();
            //var headers = new MsgHeader
            //{
            //    { "name", serviceTopic }
            //};
            //var msg = new Msg()
            //{
            //    //Data = Encoding.ASCII.GetBytes(val),
            //    Data = NatsHelper.GetBytes(new Model()),
            //    Reply = serviceTopic,
            //    Subject = "connectionWorker",
            //    Header = headers
            //};
            ////Console.WriteLine("Press key to send message...");
            ////Console.ReadKey();
            //Console.WriteLine("Connecting with NATS worker....");
            //var res = cn.Request(msg);
            //Console.WriteLine("Responce recieved.");

            //Conn = NatsHelper.GetData<ConnectionModel>(res.Data).Connection;
            //if (Conn != null)
            //{
            //    Console.WriteLine("Cannot create Connection");
            //    return;
            //}
            //Console.WriteLine("Connected to NATS");

        }

        protected abstract List<Type> Types { get; }
        protected void ConvertToType(object @object, Type type)
        {
            var obj = Convert.ChangeType(@object, type);

        }
    }
}