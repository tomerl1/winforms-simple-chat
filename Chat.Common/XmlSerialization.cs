using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Chat.Common
{
    public class XmlSerialization
    {
        private XmlSerialization() { }

        public static T DeserializeXml<T>(string xml) where T : class
        {
            var serializer = new XmlSerializer(typeof(T));
            var returnValue = default(T);

            using (var reader = new StringReader(xml))
            {
                returnValue = (T)serializer.Deserialize(reader);
            }

            return returnValue;
        }

        public static string SerializeXml<T>(T obj)
        {
            var serializer = new XmlSerializer(typeof(T));
            var xml = string.Empty;

            StringBuilder sb = new StringBuilder();
            using (var writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, obj);
                writer.Flush();
                xml = sb.ToString();
            }

            return xml;
        }
    }
}
