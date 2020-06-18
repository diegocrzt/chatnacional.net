using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Text;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting server");
                
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ip = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12345);

            // TODO: Read, all the information from config files

            socket.Bind(ip);
            socket.Listen(1024); // TODO: handle situations of >1024 connections
            List<Socket> clients = new List<Socket>();
            

            while (true) {
                Socket client = socket.Accept();
                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClient));
                clientThread.Start(client);
                clients.Add( client);
            }
        }

        private static void HandleClient(Object obj){
            Socket client = (Socket) obj;
            string welcomeMessage = "Simple National Channel Server";
            byte[] message = Encoding.UTF8.GetBytes(welcomeMessage);
            client.Send(message,message.Length,0);
            byte[] buffer= new byte[2048];
            while(true) {
                client.Receive(buffer,buffer.Length,0);
            }
        }
    }
}
