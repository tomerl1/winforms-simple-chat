using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Chat.Common;
using ChatClient.Core;
using System.Threading;
using ChatServer.Core.CustomEventArgs;

namespace ChatClient
{
    public partial class ChatForm : Form
    {
        private ChatClientManager _manager;

        public ChatForm(ClientDetails clientDetails, string ipAddress, string port)
        {
            InitializeComponent();

            _manager = new ChatClientManager(clientDetails, ipAddress, int.Parse(port));
            _manager.IncomingMessage += Manager_IncomingMessage;
            _manager.Connected += Manager_Connected;

            lbMessages.Items.Add($"Connecting to server... ({ipAddress}:{port})");

            // connect method is blocking so it needs to run in a new thread
            var chatThread = new Thread(_manager.Connect);
            chatThread.Start();
        }

        private void Manager_Connected(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                var handler = new EventHandler(Manager_Connected);
                Invoke(handler, sender, e);
                return;
            }

            lbMessages.Items.Add("Connected to server");
        }

        private void Manager_IncomingMessage(object sender, ChatMessageEventArgs args)
        {
            if (InvokeRequired)
            {
                var handlerDelegate = new ChatClientManager.ClientMessageEventHandler(Manager_IncomingMessage);
                Invoke(handlerDelegate, sender, args);
                return;
            }

            lbMessages.Items.Add($"{args.Message.NickName} said: {args.Message.Content}");
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            var text = txtMessage.Text;
            txtMessage.Text = string.Empty;

            if (text.Length > 0)
            {
                _manager.SendMessage(text);
            }
        }

        private void txtMessage_KeyPress(object sender, KeyPressEventArgs e)
        {
            // send messages on 'Enter' key
            if (e.KeyChar == (char)Keys.Return)
            {
                btnSend_Click(this, e);
            }
        }
    }
}
