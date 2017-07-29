using ChatServer.Core;
using ChatServer.Core.CustomEventArgs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatServer
{
    public partial class Form1 : Form
    {
        private ChatServerManager chatServer;

        private delegate void ClientConnectedDelegate(object sender, ClientConnectedEventArgs args);

        public Form1()
        {
            InitializeComponent();

            // Init chat server manager
            chatServer = new ChatServerManager("127.0.0.1", 3434);

            // server events
            chatServer.ClientConnected += ChatServer_ClientConnected;
            chatServer.ClientDisconnected += ChatServer_ClientDisconnected;
            chatServer.NewMessage += ChatServer_NewMessage;

            // Start listening on a new thread
            var serverThread = new Thread(chatServer.Start);
            serverThread.Start();
            lbServer.Text = "Server running...";
        }

        private void ChatServer_NewMessage(object sender, ChatMessageEventArgs args)
        {
            if (InvokeRequired)
            {
                var dlg = new Delegates.ClientMessageEventHandler(ChatServer_NewMessage);
                Invoke(dlg, sender, args);
                return;
            }

            lbServer.Items.Add($"{args.Message.NickName} said: {args.Message.Content}");
        }

        private void ChatServer_ClientDisconnected(object sender, ClientConnectedEventArgs args)
        {
            if (InvokeRequired)
            {
                var dlg = new ClientConnectedDelegate(ChatServer_ClientConnected);
                Invoke(dlg, sender, args);
                return;
            }

            Text = $"{args.ClientsCount} Connected Clients";
            lbServer.Items.Add($"{args.ClientDetails.NickName} disconnected.");
        }

        private void ChatServer_ClientConnected(object sender, ClientConnectedEventArgs args)
        {
            if (InvokeRequired)
            {
                var dlg = new ClientConnectedDelegate(ChatServer_ClientConnected);
                Invoke(dlg, sender, args);
                return;
            }

            Text = $"{args.ClientsCount} Connected Clients";
            lbServer.Items.Add($"{args.ClientDetails.NickName} connected.");
        }
    }
}
