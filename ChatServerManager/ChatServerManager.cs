using Chat.Common;
using ChatServer.Core.CustomEventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatServer.Core
{
    public class ChatServerManager
    {
        #region Fields
        private Dictionary<Guid, ChatClient> _clientsPool;
        private TcpListener _listener;
        private bool _running;
        private int _activeClients;
        private object _sync;
        #endregion

        #region Properties
        public int ClientCount
        {
            get
            {
                return _activeClients;
            }
        }
        #endregion

        #region Events
        public event Delegates.ClientConnectedEventHandler ClientConnected;
        public event Delegates.ClientConnectedEventHandler ClientDisconnected;
        public event Delegates.ClientMessageEventHandler NewMessage;
        #endregion

        #region Events Raising Methods
        public void RaiseClientConnectedEvent(object sender, ClientDetailsEventArgs args)
        {
            lock (_sync)
            {
                _activeClients++;
            }

            ClientConnected?.Invoke(this, new ClientConnectedEventArgs(ClientCount, args.ClientDetails));
        }

        private void RaiseIncomingMessageEvent(object sender, ChatMessageEventArgs args)
        {
            NewMessage?.Invoke(sender, args);
            Broadcast(args.Message);
        }
        #endregion

        public ChatServerManager(string ipAddress, int port)
        {
            _sync = new object();
            _running = false;
            _activeClients = 0;
            _listener = InitListener(ipAddress, port);
        }

        public void Start()
        {
            _running = true;
            _clientsPool = new Dictionary<Guid, ChatClient>();


            // listening (main) loop
            try
            {
                _listener.Start();

                while (_running)
                {
                    var newTcpClient = _listener.AcceptTcpClient();
                    Console.WriteLine("New client connected");

                    var chatClient = new ChatClient(newTcpClient);
                    chatClient.IncomingMessage += RaiseIncomingMessageEvent;
                    chatClient.ReceivedClientDetails += RaiseClientConnectedEvent;
                    chatClient.Disconnected += ChatClient_Disconnected;

                    lock (_sync)
                    {
                        _clientsPool.Add(chatClient.Id, chatClient);
                    }

                    // start the client in a thread, otherwise it will block main loop
                    Thread clientThread = new Thread(chatClient.Start);
                    clientThread.Start();
                }
            }
            finally
            {
                _listener.Stop();
            }
        }

        private void ChatClient_Disconnected(object sender, EventArgs args)
        {
            var chatClient = (ChatClient)sender;

            lock (_sync)
            {
                _activeClients--;
                _clientsPool.Remove(chatClient.Id);
            }

            var details = new ClientDetails()
            {
                NickName = chatClient.NickName,
                Color = chatClient.Color.ToString()
            };

            ClientDisconnected?.Invoke(this, new ClientConnectedEventArgs(_activeClients, details));
        }

        private TcpListener InitListener(string ipAddress, int port)
        {
            var ip = IPAddress.Parse(ipAddress);
            var listener = new TcpListener(ip, port);
            return listener;
        }

        private void Broadcast(Message message)
        {
            IEnumerable<ChatClient> clients = null;

            lock (_sync)
            {
                // for quick release - get the list of clients to send data to
                clients = _clientsPool.Select(kvp => kvp.Value);
            }

            foreach (var client in clients)
            {
                try
                {
                    client.SendMessage(message);
                }
                catch (Exception ex)
                {
                    // in case the connection get interrupted
                    // nothing to do really...
                }
            }
        }
    }
}
