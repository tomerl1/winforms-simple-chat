using ChatServer.Core.CustomEventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.Core
{
    public class Delegates
    {
        public delegate void ClientConnectedEventHandler(object sender, ClientConnectedEventArgs args);
        public delegate void NewClientDetailsEventHandler(object sender, ClientDetailsEventArgs args);
        public delegate void ClientMessageEventHandler(object sender, ChatMessageEventArgs args);
        public delegate void ClientDiconnectedEventHandler(object sender, EventArgs args);
    }
}
