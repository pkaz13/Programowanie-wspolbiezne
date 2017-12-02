using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rendezvous_v2
{
    public class Server
    {
        public int AmountOfResourceA { get; set; }
        public int AmountOfResourceB { get; set; }

        private Queue<Client> Clients = new Queue<Client>();
        private Object syncObject = new Object();

        public Server()
        {
            AmountOfResourceB = 1;
            AmountOfResourceA = 1;
        }

        public void GetRequest(Client client)
        {
            Clients.Enqueue(client);
            lock (syncObject)
            {
                Monitor.Pulse(syncObject);
            }
            
        }

        public void Run()
        {
            while (true)
            {
                lock (syncObject)
                {
                    while (Clients.Count == 0)
                    {
                        Console.WriteLine("Server is waiting for request...");
                        Monitor.Wait(syncObject);
                    }
                }
                
                Client client = Clients.Dequeue();
                bool takeResource = false;
                while (takeResource == false)
                {
                    switch (client.ResourceType)
                    {
                        case ResourceType.A:
                            if (AmountOfResourceA > 0)
                            {
                                Console.WriteLine($"Client {client.Name} takes resource A");
                                AmountOfResourceA--;
                                takeResource = true;
                                Thread.Sleep(10000);
                                AmountOfResourceA++;
                                Console.WriteLine($"Client {client.Name} releases resource A");
                            }
                            break;
                        case ResourceType.B:
                            if (AmountOfResourceB > 0)
                            {
                                Console.WriteLine($"Client {client.Name} takes resource B");
                                AmountOfResourceB--;
                                takeResource = true;
                                Thread.Sleep(10000);
                                AmountOfResourceB++;
                                Console.WriteLine($"Client {client.Name} releases resource B");
                            }
                            break;
                        case ResourceType.AB:
                            if (AmountOfResourceA > 0 && AmountOfResourceB > 0)
                            {
                                Console.WriteLine($"Client {client.Name} takes resource A and B");
                                AmountOfResourceA--;
                                AmountOfResourceB--;
                                takeResource = true;
                                Thread.Sleep(10000);
                                AmountOfResourceA++;
                                AmountOfResourceB++;
                                Console.WriteLine($"Client {client.Name} releases resources A and B");
                            }
                            break;
                    }
                    client.NotifyAboutResponse();
                }
            }
        }
    }
}
