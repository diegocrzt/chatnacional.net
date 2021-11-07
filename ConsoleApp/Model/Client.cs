using System;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Text;
using SNCP.Protocol;



namespace SNCP.Model
{

    class Client
    {

        private Socket _socket;
        private string _name;
        private Thread _thread;
        private RoomManager _roomManager;
        private bool _activeClient;

        public Client(Socket socket, RoomManager roomManager)
        {
            this._socket = socket;
            this._name = "TODO: Random Unique Name";
            this._thread = new Thread(new ParameterizedThreadStart(HandleClient));
            this._roomManager = roomManager;
            this._activeClient = true;
        }

        public string GetName()
        {
            return this._name;
        }

        public void Start()
        {
            Console.WriteLine("Spawning a new thread...");
            this._thread.Start(this);
        }

        public void SendMessage(string message)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(message.Trim());
            Console.WriteLine("Sending " + bytes.Length + " bytes ");
            this._socket.Send(bytes, bytes.Length, 0);
        }

        public void DoLogin(string name)
        {
            if (name == null || "".Equals(name))
            {
                SendError("Invalid Name");
            }

            this._name = name;
            SendMessage("Welcome " + this._name);
        }

        public void DisconnectFromChannel(string channel)
        {
            if (channel == null || "".Equals(channel))
            {
                Console.Error.WriteLine("No channel received");
                SendError("No channel received, disconnecting");
                CloseConnection();
            }
            else
            {
                this._roomManager.LeaveRoom(channel, this);
            }
        }

        public void SendToChannel(string channel, string messageToChannel)
        {
            this._roomManager.JoinRoom(channel, this);
            Room room = this._roomManager.GetRoomForclient(this.GetName(), channel);

            if (room == null)
            {
                SendError("Client not in channel");
            }
            else
            {
                room.ForAllClients(c => c.SendMessage(messageToChannel));
            }
        }

        public void ConnectToChannel(string channel)
        {
            this._roomManager.JoinRoom(channel, this);
        }

        public void ManageMessage(string message)
        {

            Command command = new Command(message);
            string cmd = command.GetCommand();
            switch (cmd)
            {
                case "DISCONNECT":
                    DisconnectFromChannel(command.GetElementAt(1));
                    break;
                case "LOGOUT":
                    CloseConnection();
                    break;
                case "LOGIN":
                    DoLogin(command.GetElementAt(1));
                    break;
                case "SEND":
                    SendToChannel(command.GetElementAt(1), command.GetFrom(2));
                    break;
                case "CONNECT":
                    ConnectToChannel(command.GetElementAt(1));
                    break;
                default:
                    Console.Error.WriteLine("Invalid command '" + cmd + "'");
                    SendError("Invalid Command");
                    break;
            }
        }

        public void SendStatus(string statusMessage)
        {
            SendMessage("STATUS " + statusMessage);
        }

        public void SendError(string errorMessage)
        {
            SendMessage("ERROR " + errorMessage);
        }

        public bool InternalStatus()
        {
            return this._socket.IsBound && this._thread.IsAlive;
        }

        public void CloseConnection()
        {
            this._activeClient = false;
            SendStatus("BYE");
            this._socket.Close();
            Console.WriteLine("Finalizing Thread...");
            //this._thread.Abort();
        }

        public void DisposeClient()
        {
            this._roomManager.DisposeClient(this);
            this._socket.Dispose();
            this._activeClient = false;
            //this._socket.Dispose();
            //this._thread.Abort();
        }

        public void JoinDefaultRoom()
        {
            this._roomManager.JoinDefaultRoom(this);
        }

        public string Receive()
        {
            byte[] buffer = new byte[1024];
            int received = this._socket.Receive(buffer, buffer.Length, 0);
            byte[] sizedBuffer = new byte[received];
            for (int i = 0; i < received; i++)
            {
                sizedBuffer[i] = buffer[i];
            }
            Console.WriteLine("Received " + received + " byte(s)");
            return Encoding.UTF8.GetString(sizedBuffer, 0, received);
        }
        private static void HandleClient(Object obj)
        {
            Client client = (Client)obj;
            string welcomeMessage = "Simple National Channel Server";
            client.SendMessage(welcomeMessage);
            client.JoinDefaultRoom();
            while (true && client._activeClient)
            {
                try
                {
                    string receivedMessage = client.Receive();
                    Console.WriteLine(receivedMessage);
                    client.ManageMessage(receivedMessage);
                }
                catch (SocketException se)
                {
                    Console.Error.WriteLine("Failing I don't know why " + se);
                    client.DisposeClient();
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine("Failing I don't know why " + e);
                }


            }

            Console.WriteLine("Disposed client");

        }

    }
}

