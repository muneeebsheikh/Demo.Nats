using NATS.Client;
using System.Xml.Linq;

namespace Storage
{
    public class Model
    {
        public string Message { get; set; }

    }

    public class ObjectModel
    {
        public object Message { get; set; }
        public Type Type { get; set; }
    }

}