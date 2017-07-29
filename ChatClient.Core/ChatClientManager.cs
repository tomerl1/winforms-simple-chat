using Chat.Common;
using ChatServer.Core.CustomEventArgs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient.Core
{
    public class ChatClientManager
    {
        #region Fields
        private string _ipAddress;
        private int _port;
        private ClientDetails _clientDetails;
        private TcpClient _tcpClient;
        private NetworkStream _stream;
        private bool _active;
        private BinaryWriter _writer;
        #endregion

        #region Delegates
        public delegate void ClientMessageEventHandler(object sender, ChatMessageEventArgs args);
        #endregion

        #region Events
        public event ClientMessageEventHandler IncomingMessage;
        public event EventHandler Connected;
        #endregion

        #region Events Raising Methods
        public void RaiseClientMessageEvent(Message message)
        {
            IncomingMessage?.Invoke(this, new ChatMessageEventArgs(message));
        }

        public void RaiseConnectedEvent()
        {
            Connected?.Invoke(this, new EventArgs());
        }
        #endregion

        public ChatClientManager(ClientDetails clientDetails, string ipAddress, int port)
        {
            _clientDetails = clientDetails;
            _ipAddress = ipAddress;
            _port = port;
        }

        public void Connect()
        {
            _tcpClient = new TcpClient();
            _tcpClient.Connect(IPAddress.Parse(_ipAddress), _port);
            Console.WriteLine("Connected");
            RaiseConnectedEvent();

            _stream = _tcpClient.GetStream();

            _writer = new BinaryWriter(_stream, Encoding.UTF8);

            // first send client details
            var xmlClientDetails = XmlSerialization.SerializeXml(_clientDetails);
            _writer.Write(xmlClientDetails);

            // now start listening to messages
            _active = true;
            var reader = new BinaryReader(_stream, Encoding.UTF8);

            try
            {
                while (_active)
                {
                    var xmlMessage = reader.ReadString();
                    var message = XmlSerialization.DeserializeXml<Message>(xmlMessage);
                    RaiseClientMessageEvent(message);
                }
            }
            catch (Exception ex)
            {
                // disconnected
            }
            finally
            {
                reader.Close();
                _tcpClient.Close();
                _writer.Close();
            }
        }

        public void SendMessage(string text)
        {
            // wrap in Message
            var message = new Message()
            {
                NickName = _clientDetails.NickName,
                Content = text
            };

            var xmlMessage = XmlSerialization.SerializeXml(message);

            _writer.Write(xmlMessage);
        }
    }
}
