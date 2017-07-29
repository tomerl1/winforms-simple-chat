using Chat.Common;
using ChatServer.Core.CustomEventArgs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatServer.Core
{
    public class ChatClient : IDisposable
    {
        #region Fields
        private TcpClient _client;
        private NetworkStream _stream;
        private BinaryWriter _writer;
        private bool _running;
        #endregion

        #region Properties
        public Guid Id { get; private set; }
        public string NickName { get; private set; }
        public Color Color { get; private set; }
        #endregion

        #region Events
        public event Delegates.ClientMessageEventHandler IncomingMessage;
        public event Delegates.NewClientDetailsEventHandler ReceivedClientDetails;
        public event Delegates.ClientDiconnectedEventHandler Disconnected;
        #endregion

        #region Event Raising Methods
        public void RaiseIncomingMessageEvent(Message message)
        {
            IncomingMessage?.Invoke(this, new ChatMessageEventArgs(message));
        }

        public void RaiseNewClientDetailsEvent(ClientDetails clientDetails)
        {
            ReceivedClientDetails?.Invoke(this, new ClientDetailsEventArgs(clientDetails));
        }

        public void RaiseClientDisconnectedEvent()
        {
            Disconnected?.Invoke(this, new EventArgs());
        }
        #endregion

        public ChatClient(TcpClient tcpClient)
        {
            _client = tcpClient;
            _stream = _client.GetStream();
            _writer = new BinaryWriter(_stream, Encoding.UTF8);

            Id = Guid.NewGuid();
        }
        
        public void Dispose()
        {
            if (_client != null)
            {
                _client.Close();
            }

            if (_stream != null)
            {
                _stream.Close();
            }
        }

        public void Start()
        {
            var reader = new BinaryReader(_stream, Encoding.UTF8);

            try
            {
                // first we need to get the nickname & color
                // so the first message from the client is expected to be ClientDetails
                var xmlClientDetails = reader.ReadString();
                var clientDetails = XmlSerialization.DeserializeXml<ClientDetails>(xmlClientDetails);
                NickName = clientDetails.NickName;
                Color = Color.FromName(clientDetails.Color);
                Console.WriteLine($"[{Id}] Received client details: [{clientDetails}]");
                RaiseNewClientDetailsEvent(clientDetails);

                // finally we can start reading "normal" messages
                _running = true;
                while (_running)
                {
                    var xmlMessage = reader.ReadString();
                    var incomingMessage = XmlSerialization.DeserializeXml<Message>(xmlMessage);
                    RaiseIncomingMessageEvent(incomingMessage);
                }
            }
            catch (Exception ex)
            {
                // connection was closed
                Console.WriteLine($"[{Id}] Client disconnected");
                RaiseClientDisconnectedEvent();
            }
            finally
            {
                reader.Close();
            }
        }

        public void SendMessage(Message message)
        {
            var xmlMessage = XmlSerialization.SerializeXml(message);
            _writer.Write(xmlMessage);
        }
    }
}
