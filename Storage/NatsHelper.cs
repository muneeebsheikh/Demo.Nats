using NATS.Client;
using NATS.Client.Rx;
using NATS.Client.Rx.Ops;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Storage
{
    public class NatsHelper
    {
        public const string NATSIP = "127.0.0.1";
        public const string NATSPORT = "4222";
        public const string NATSMONITORINGPORT = "8222";

        static IConnection Conn;

        private static volatile object _lock = new();

        public static IConnection GetConnection()
        {
            lock (_lock)
            {
                if (Conn == null)
                {
                    Console.WriteLine("Creating Connection...");
                    var Connectionfactory = new ConnectionFactory();
                    Options opts = ConnectionFactory.GetDefaultOptions();
                    opts.ServerDiscoveredEventHandler += (sender, args) =>
                    {
                        Console.WriteLine("A new server has joined the cluster:");
                        Console.WriteLine("    " + String.Join(", ", args.Conn.DiscoveredServers));
                    };
                    Conn = Connectionfactory.CreateConnection($"nats://{NATSIP}:{NATSPORT}");

                }
                return Conn;
            }
        }
        public static INATSObservable<Envelop> CreateObservable(string topic, IConnection cn)
        {
            var ob =
                cn.Observe(topic)
                    .Where(m => m.Data?.Any() == true)
                    .Select(m => GetEnv(m.Data));
            //.Select(m => BitConverter.ToInt32(m.Data, 0));
            //observables.Add(ob);
            return ob;
        }
        public static byte[] GetBytes(object obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            var bytes = Encoding.ASCII.GetBytes(json);
            return bytes;
        }

        public static T GetData<T>(byte[] data)
        {
            var json = Encoding.ASCII.GetString(data);
            var obj = JsonConvert.DeserializeObject<T>(json);
            return obj;
        }

        //public static T GetData(byte[] data)
        //{
        //    var json = Encoding.ASCII.GetString(data);
        //    var obj = JsonConvert.DeserializeObject<T>(json);
        //    return obj;
        //}

        public static object GetData(byte[] data)
        {
            var json = Encoding.ASCII.GetString(data);
            var obj = JsonConvert.DeserializeObject<object>(json);
            return obj;
        }

        public static object GetData(string json, Type type)
        {
            var obj = JsonConvert.DeserializeObject(json, type);
            return obj;
        }

        public static Envelop GetEnv(byte[] bytes)
        {
           return GetData<Envelop>(bytes);
        }

        public static bool CheckNatsStatus()
        {
            RestClient _restClient;

            try
            {
                _restClient = new RestClient($"http://{NATSIP}:{NATSMONITORINGPORT}");
                var request = new RestRequest("healthz");
                var response = _restClient.Get(request);

                if (response != null && response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"NATS Exception - {ex.Message}");
                return false;
                //throw;
            }
            return false;
        }

    }
}
