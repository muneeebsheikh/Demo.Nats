using NATS.Client;
using NATS.Client.Rx;
using NATS.Client.Rx.Ops;
using Service.Base;
using Storage;

namespace WorkerService1
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        protected ConnectionFactory Connectionfactory;
        protected IConnection Conn;
        protected const string NATSIP = "127.0.0.1";
        protected const string NATSPORT = "4222";
        protected const string NATSMONITORINGPORT = "8222";

        //static IConnection Conn;

        private static volatile object _lock = new();

        public Worker(ILogger<Worker> logger)
        {
            SetConnection();
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var observable = NatsHelper.CreateObservable("connectionWorker", Conn);

            observable.Subscribe(o =>
            {
                SetConnection();
                _logger.LogInformation($"Request Recieved from {(o.HasHeaders ? o.Header["name"] : "anonymous")}");
                o.Respond(NatsHelper.GetBytes(new ObjectModel() { Message = 100 }));
                _logger.LogInformation($"Responded.");
            }, err => Console.WriteLine($"Worker Error - {err.Message}"), ()=>Console.WriteLine("Worker completed."));


            while (!stoppingToken.IsCancellationRequested)
            {
                //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                
                await Task.Delay(1000 * 5, stoppingToken);
            }
        }

        private void SetConnection()
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
            }
        }


    }
}