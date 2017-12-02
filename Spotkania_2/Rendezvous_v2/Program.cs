using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rendezvous_v2
{
    class Program
    {
        static List<Client> Clients = new List<Client>();

        static Server server = new Server();
        static Thread serverThread;
        static Thread threadClientA;
        static Thread threadClientB;
        static Thread threadClientAB;

        static void Main(string[] args)
        {
            serverThread = new Thread(server.Run);
            serverThread.Start();

            Client clientA = new Client()
            {
                Name = "Client A",
                ResourceType = ResourceType.A,
                Server = server,
            };
            Clients.Add(clientA);
            Client clientB = new Client()
            {
                Name = "Client B",
                ResourceType = ResourceType.B,
                Server = server,
            };
            Clients.Add(clientB);
            Client clientAB = new Client()
            {
                Name = "Client AB",
                ResourceType = ResourceType.AB,
                Server = server,
            };
            Clients.Add(clientAB);

            threadClientA = new Thread(ClientRun);
            threadClientA.Name = "Client A";
            threadClientA.Start();
            threadClientB = new Thread(ClientRun);
            threadClientB.Name = "Client B";
            threadClientB.Start();
            threadClientAB = new Thread(ClientRun);
            threadClientAB.Name = "Client AB";
            threadClientAB.Start();



        }

        static void ClientRun()
        {
            while (true)
            {
                Client currentClient = Clients.FirstOrDefault(x => x.Name == Thread.CurrentThread.Name);
                if (currentClient != null)
                {
                    currentClient.SendRequest();
                }
            }
        }
    }
}

