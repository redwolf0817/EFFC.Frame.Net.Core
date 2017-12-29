using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using EFFC.Frame.Net.Base.Interfaces.DataConvert;

namespace EFFC.Frame.Net.Data.DataConvert
{
    public class Xml2List<T> : IDataConvert<string,List<T>>
    {
        public List<T> ConvertTo(string obj)
        {
            try
            {
                using (StringReader sr = new StringReader(obj))
                {
                    XmlSerializer xmldes = new XmlSerializer(typeof(List<T>), new XmlRootAttribute("root"));

                    return (List<T>)xmldes.Deserialize(sr);
                }
            }
            catch (Exception e)
            {

                return null;
            }
        }
    }
}
