using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Test
{
    public class FrameArrayTest
    {
        public static void Test()
        {
            var obj = FrameDLRObject.CreateInstance(new
            {
                touser = "",
                toparty = "",
                totag = "",
                msgtype = "text",
                agentid = "",
                text = new
                {
                    content = new object[] { new
                    {
                        title = "",
                        description = "",
                        url = ""
                    }
                    }
                },
                safe = 1
            }, FrameDLRFlags.SensitiveCase);

            var dt = DateTime.Now;
            var fea = FrameExposedArray.From(new object[] { "a","b", 1, DateTime.Now ,DateTime.Now.AddDays(1)});
            var n = fea.String.value;
            var n1 = fea.String[1];
            dt = DateTime.Now;
            fea = FrameExposedArray.From(new object[] { 2.3,5.6,new Dictionary<string,object>(),'s',new StaticClassTest(), 1, DateTime.Now, DateTime.Now.AddDays(1) });
            var n2 = fea.dictionary.value;
            Console.WriteLine($"time cast {(DateTime.Now - dt).TotalMilliseconds}ms");
            Console.ReadKey();
        }
    }
}
