using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

using SNCP.Model;

namespace SNCP
{
    class Daemon
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting server");
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ip = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12345);
            RoomManager rooms = new RoomManager("Paraguay");

            // TODO: Read, all the information from config files

            socket.Bind(ip);
            socket.Listen(1024); // TODO: handle situations of >1024 connections
            List<Client> clients = new List<Client>();

            while (true)
            {
                Console.WriteLine("Waiting for new clients...");
                Client client = new Client(socket.Accept(),rooms);
                client.Start();
                clients.Add(client);
                Console.WriteLine("We have " + clients.Count + " client(s)");
                _cleanClosed(clients);
            }
        }

        private static void _cleanClosed(List<Client> clients)
        {
            List<Client> toRemove = new List<Client>();
            foreach (Client c in clients)
            {
                if (!c.InternalStatus())
                {
                    toRemove.Add(c);
                }
            }

            foreach (Client c in toRemove)
            {
                Console.WriteLine("Removing " + c.GetName());
                clients.Remove(c);
            }
        }

    }
}
