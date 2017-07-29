using Chat.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.Core.CustomEventArgs
{
    public class ClientConnectedEventArgs : EventArgs
    {
        public int ClientsCount { get; private set; }
        public ClientDetails ClientDetails { get; private set; }

        public ClientConnectedEventArgs(int clientsCount, ClientDetails clientDetails)
        {
            ClientsCount = clientsCount;
            ClientDetails = clientDetails;
        }
    }
}
