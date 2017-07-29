using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Common
{
    [Serializable]
    public class ClientDetails
    {
        public string NickName { get; set; }
        public string Color { get; set; }

        public override string ToString()
        {
            return string.Format("Name={0}, Color={1}", NickName, Color);
        }
    }
}
