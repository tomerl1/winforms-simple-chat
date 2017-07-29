using Chat.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatClient
{
    public partial class ConnectToChatForm : Form
    {
        public ConnectToChatForm()
        {
            InitializeComponent();

            // fill colors comobo box
            var colors = Enum.GetNames(typeof(KnownColor));
            cbColor.Items.AddRange(colors);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close(); // exit client
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            Visible = false;

            var clientDetails = new ClientDetails() {
                NickName = txtNickname.Text,
                Color = (string)cbColor.SelectedItem
            };

            ChatForm chat = new ChatForm(clientDetails, txtIp.Text, txtPort.Text);
            chat.FormClosed += Chat_FormClosed;

            chat.Show();
        }

        private void Chat_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Visible = true;
            Close();
        }
    }
}
