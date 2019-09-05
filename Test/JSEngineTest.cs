using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data.Base;
using Microsoft.AspNetCore.NodeServices;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Test
{
    public class JSEngineTest
    {
        public static void Test()
        {
            var s = FrameDLRObject.IsJsonArrayThen(@"[{
  ""Name"":""3"",
  ""Value"":""3""
}]");
            var xml = @"<xml><ToUserName><![CDATA[gh_e136c6e50636]]></ToUserName>
<FromUserName><![CDATA[oMgHVjngRipVsoxg6TuX3vz6glDg]]></FromUserName>
<CreateTime>1408090651</CreateTime>
<MsgType><![CDATA[event]]></MsgType>
<Event><![CDATA[pic_sysphoto]]></Event>
<EventKey><![CDATA[6]]></EventKey>
<SendPicsInfo><Count>1</Count>
<PicList><item><PicMd5Sum><![CDATA[1b5f7c23b5bf75682a53e7b6d163e185]]></PicMd5Sum>
</item>
</PicList>
</SendPicsInfo>
</xml>";
            XmlDocument doc = new XmlDocument();
            doc.XmlResolver = null;
            doc.LoadXml(xml);
            var root = doc.FirstChild;
            var f = FrameDLRObject.CreateInstance(xml, EFFC.Frame.Net.Base.Constants.FrameDLRFlags.SensitiveCase);
            //var services = new ServiceCollection();
            //services.AddNodeServices(options => {
            //    options.LaunchWithDebugging = false;
            //    options.ProjectPath = Path.Combine(ComFunc.GetProjectRoot(), "js");
            //});
            //var serviceProvider = services.BuildServiceProvider();
            //var nodeServices = serviceProvider.GetRequiredService<INodeServices>();
            //for (int i = 0; i < 1; i++)
            //{
            //    var dobj = FrameDLRObject.CreateInstance();
            //    dobj.id = "1100";
            //    var cc = new ConsoleL();
            //    var result = nodeServices.InvokeAsync<string>("addNumbers.js", cc).Result;
            //    Console.WriteLine(result);
            //}

        }

        public class ConsoleL
        {
            public string id { get; set; }
            public void WriteLine(string msg)
            {
                Console.WriteLine(msg);
            }
        }
    }
}
