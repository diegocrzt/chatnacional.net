using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting server");
                
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ip = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1234);

            socket.Bind(ip);
            socket.Listen(256);

            while (true) {
                Socket client = socket.Accept();
                IPEndPoint clientIp = (IPEndPoint)client.RemoteEndPoint;


                Console.WriteLine("Client connected from " + clientIp.Address + " : " + clientIp.Port);

            }
        }
    }

    class ClientHandler {
        private 
        public ClientHandler(Socket client) { 
        }
    }
}
