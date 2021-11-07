
using System.Collections.Generic;
using System;

namespace SNCP.Model
{
    class Room
    {
        private string _name;
        private Dictionary<string, Client> _clients;
        private bool _isDefault;

        public Room(string name, bool isDefault)
        {
            this._name = name;
            this._clients = new Dictionary<string,Client>();
            this._isDefault = isDefault;
        }

        public string GetName ()
        {
            return this._name;
        }

        public void AddClient(Client client)
        {
            ForAllClients(c => c.SendStatus(client.GetName() + " Joined " + this._name));
            this._clients.Add(client.GetName(), client);
        }

        public void RemoveClient(Client client)
        {
            ForAllClients(c => c.SendStatus(client.GetName() + " Leaving " + this._name));
            this._clients.Remove(client.GetName());
        }

        public void SendMessageFromClient(string clientName, string message)
        {
            ForAllClients(c => c.SendMessage("RECEIVE " + c.GetName() + " " + message));
        }

        public bool isEmpty()
        {
            return this._clients.Count == 0;
        }

        public bool isDefault()
        {
            return this._isDefault;
        }

        public bool ContainsClient(Client client)
        {
            return this._clients.ContainsKey(client.GetName());
        }

        public bool ContainsClientName(string clientName)
        {
            return this._clients.ContainsKey(clientName);
        }

        public void ForAllClients(Action<Client> action)
        {
            foreach (Client c in this._clients.Values)
            {
                action.Invoke(c);
            }
        }
    }
}