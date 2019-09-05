using EFFC.Frame.Net.Base.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Test
{
    public class MiniTools
    {
        public static void Test()
        {
            var log_path = @"E:\Debug-20180628.txt;E:\Debug-20180629.txt;E:\Debug-20180630.txt;E:\Debug-20180701.txt;E:\Debug-20180702.txt;E:\Debug-20180703.txt;";
            var list_source = new List<string>();
            foreach(var path in log_path.Split(';', StringSplitOptions.RemoveEmptyEntries))
            {
                if (File.Exists(path))
                {
                    foreach(var s in File.ReadAllLines(path))
                    {
                        var index = s.IndexOf("request content:orders=");
                        if(index > 0)
                        {
                            list_source.Add(s.Substring(index).Replace("request content:orders=", ""));
                        }
                    }
                }
            }

            var sb = new StringBuilder();
            foreach(var s in list_source)
            {
                sb.AppendLine(formatXml(ComFunc.Base64DeCode(ComFunc.UrlDecode(s))));
            }

            File.WriteAllText("e:/hongxun.txt", sb.ToString());
        }

        public static string formatXml(string xml)
        {
            XmlDocument xd;
            xd = new XmlDocument();
            //防止xxe攻击
            xd.XmlResolver = null;
            xd.LoadXml(xml as string);

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            XmlTextWriter xtw = null;
            try
            {
                xtw = new XmlTextWriter(sw);
                xtw.Formatting = Formatting.Indented;
                xtw.Indentation = 1;
                xtw.IndentChar = '\t';
                xd.WriteTo(xtw);
            }
            finally
            {
                if (xtw != null)
                    xtw.Close();
            }
            return sb.ToString();
        }
    }
}
