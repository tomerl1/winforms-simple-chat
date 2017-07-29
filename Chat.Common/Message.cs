using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Common
{
    [Serializable]
    public class Message
    {
        public string NickName { get; set; }
        public string Content { get; set; }
    }
}
