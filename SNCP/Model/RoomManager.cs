using System.Collections.Generic;

namespace SNCP.Model
{
    class RoomManager
    {
        private string _defaultRoomName;
        private Room _defaultRoom;
        private Dictionary<string, Room> _rooms;

        private Dictionary<string, List<Room>> _roomsPerClient;
        private Dictionary<string, List<Client>> _clientsPerRoom;


        public RoomManager(string defaultRoomName)
        {
            this._defaultRoom = new Room(defaultRoomName, true);
            this._defaultRoomName = defaultRoomName;
            this._rooms = new Dictionary<string, Room>();
            this._rooms.Add(defaultRoomName, this._defaultRoom);
            this._clientsPerRoom = new Dictionary<string, List<Client>>();
            this._clientsPerRoom.Add(defaultRoomName, new List<Client>());
            this._roomsPerClient = new Dictionary<string, List<Room>>();
        }

        public void JoinDefaultRoom(Client client)
        {
            JoinRoom(this._defaultRoomName, client);
        }

        public void JoinRoom(string roomName, Client client)
        {
            Room room;
            if (!this._rooms.TryGetValue(roomName, out room))
            {
                room = new Room(roomName, false);
                this._rooms.Add(roomName, room);
            }
            if (!room.ContainsClient(client))
            {
                room.AddClient(client);
            }


            List<Room> rooms;
            if (!this._roomsPerClient.TryGetValue(client.GetName(), out rooms))
            {
                rooms = new List<Room>();
                this._roomsPerClient.Add(client.GetName(), rooms);
            }
            if (!rooms.Contains(room))
            {
                rooms.Add(room);
            }

            List<Client> clients;
            if (!this._clientsPerRoom.TryGetValue(roomName, out clients))
            {
                clients = new List<Client>();
                this._clientsPerRoom.Add(roomName, clients);
            }

            if (!clients.Contains(client))
            {
                clients.Add(client);
            }

        }


        public List<Room> RoomsForClient(string clientName)
        {
            List<Room> ret;
            if(!this._roomsPerClient.TryGetValue(clientName, out ret))
            {
                return new List<Room>();
            }

            return ret;

        }

        public Room GetRoomForclient(string clientName, string roomName)
        {
            Room room;
            if (!this._rooms.TryGetValue(roomName, out room)) {
                return null;
            };

            if(!room.ContainsClientName(clientName))
            {
                return null;
            }

            return room;
        }

        public void DisposeClient(Client client) {
            List<Room> rooms;
            this._roomsPerClient.TryGetValue(client.GetName(), out rooms);
            foreach (Room r in rooms) {
                LeaveRoom(r.GetName(), client);
            }
            this._defaultRoom.RemoveClient(client);
        }

        public void LeaveRoom(string roomName, Client client)
        {
            if (!this._defaultRoomName.Equals(roomName))
            {
                Room room;
                if (!this._rooms.TryGetValue(roomName, out room))
                    return;
                room.RemoveClient(client);


                List<Room> rooms;
                List<Client> clients;
                if (!this._clientsPerRoom.TryGetValue(roomName, out clients))
                    return;
                clients.Remove(client);

                if (!this._roomsPerClient.TryGetValue(client.GetName(), out rooms))
                    return;
                rooms.Remove(room);

                if (room.isEmpty() && !room.isDefault())
                {
                    this._rooms.Remove(roomName);
                    this._clientsPerRoom.Remove(roomName);
                    this._rooms.Remove(roomName);
                }
            }
            else
            {
                client.SendError("Can't leave the deafult room");
            }
        }
    }
}