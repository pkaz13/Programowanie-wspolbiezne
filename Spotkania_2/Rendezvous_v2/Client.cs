using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rendezvous_v2
{
    public class Client
    {
        public ResourceType ResourceType { get; set; }
        public Server Server { get; set; }
        public string Name { get; set; }

        private Object syncObject = new Object();
        private bool isWaiting = false;

        public void SendRequest()
        {
            Console.WriteLine($"Client {Name} is sending request to server...");
            isWaiting = true;
            Server.GetRequest(this);
            Console.WriteLine("Waiting for server response...");
            lock (syncObject)
            {
                while (isWaiting == true)
                {
                    Monitor.Wait(syncObject);
                }
            }
        }

        public void NotifyAboutResponse()
        {
            isWaiting = false;
            Console.WriteLine($"Client {Name} left server.");
            lock (syncObject)
            {
                Monitor.Pulse(syncObject);
            }
        }
    }

    public enum ResourceType
    {
        A, B, AB
    }
}
