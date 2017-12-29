using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using EFFC.Frame.Net.Base.Interfaces.DataConvert;

namespace EFFC.Frame.Net.Data.DataConvert
{
    public class List2Xml<T> : IDataConvert<List<T>,string>
    {
        public string ConvertTo(List<T> obj)
        {
            MemoryStream Stream = new MemoryStream();
            //创建序列化对象
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);
            XmlSerializer xml = new XmlSerializer(typeof(List<T>), new XmlRootAttribute("root"));
            try
            {
                //序列化对象
                xml.Serialize(Stream, obj, ns);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            Stream.Position = 0;
            StreamReader sr = new StreamReader(Stream);
            string str = sr.ReadToEnd();
            return str;
        }
    }
}
