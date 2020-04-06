using ChecksumHandlerLib;
using ConnectionHandlerLib;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PatchManager
{
    public class PatchServer
    {
        List<TcpClient> clients = new List<TcpClient>();
        string filesPath = "";
        bool running;

        public PatchServer()
        {
        }

        public void Start()
        {
            if (!running)
                Task.Run(() => HandleIncomingConnections());
        }

        public void Stop()
        {
            running = false;
        }

        public void HandleIncomingConnections()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, 10000);
            listener.Start();
            running = true;
            Console.WriteLine("Started handling incomming connections");
            while (running)
            {
                TcpClient client = listener.AcceptTcpClient();

                lock (clients)
                {
                    clients.Add(client);
                    Task.Run(() => HandleClientConnection(client));
                }
            }
            Console.WriteLine("Stopped handling incomming connections");
        }

        public void HandleClientConnection(TcpClient client)
        {
            while (ConnectionHandler.Connected(client))
            {

            }

            lock (clients)
                clients.Remove(client);
        }
    }
}
