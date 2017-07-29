using Chat.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.Core.CustomEventArgs
{
    public class ChatMessageEventArgs : EventArgs
    {
        public Message Message { get; private set; }

        public ChatMessageEventArgs(Message message)
        {
            Message = message;
        }
    }
}
