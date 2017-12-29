using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Data;
using EFFC.Frame.Net.Data.LogicData;
using System;
using System.Dynamic;

namespace FrameTest
{
    class Program
    {
        static void Main(string[] args)
        {
            dynamic d = new ExpandoObject();
            d.userid = "ych";
            string a = d.userid;
            dynamic f = FrameDLRObject.CreateInstance();
            f.userid = "ych";
            string b = d.userid;

            var ld = new LogicData();

            var c = new LoginUserData() { UserID = "ych" };
            c.Actions.AddAction("f01", "", "", "Q");
            var json = c.ToJSONString();
            Console.WriteLine(json);
            FrameDLRObject obj = FrameDLRObject.CreateInstance(json, FrameDLRFlags.SensitiveCase);
            var lu = new LoginUserData();
            lu.TryParseJSON(obj);
        }
    }
}