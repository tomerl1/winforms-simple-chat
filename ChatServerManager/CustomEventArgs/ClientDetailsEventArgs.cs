using Chat.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.Core.CustomEventArgs
{
    public class ClientDetailsEventArgs : EventArgs
    {
        public ClientDetails ClientDetails { get; private set; }

        public ClientDetailsEventArgs(ClientDetails clientDetails)
        {
            ClientDetails = clientDetails;
        }
    }
}
