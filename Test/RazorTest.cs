using EFFC.ChakraCore;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data.Base;
using JavaScriptEngineSwitcher.ChakraCore;
using JavaScriptEngineSwitcher.Core;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.AspNetCore.Razor.Language.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Test
{
    class RazorTest
    {
        public static void Test()
        {
            //JsEngineSwitcher engineSwitcher = JsEngineSwitcher.Instance;
            //IJsEngine js = null;
            //engineSwitcher.EngineFactories
            //    .AddEFFCChakraCore();

            //engineSwitcher.DefaultEngineName = EFFCChakraCoreJsEngine.EngineName;

            //js = engineSwitcher.CreateDefaultEngine();
            //js.EmbedHostObject("f_p_0", "ych");
            //js.EmbedHostObject("f_p_1", 22);
            //js.Execute($"var js ={{name:f_p_0,age:f_p_1}};");
            //var re = js.GetVariableValue("js");
            //var fec = FrameExposedObject.From(re.GetType());

            //js.Dispose();
            var md5 = ComFunc.getMD5_String(ComFunc.getMD5_String("sa").ToLower()+"8128").ToLower();
            //var serializer = JsonSerializer.Create();
            var json = "{name:'ych',age:22,schools:[{name:'小学',address:''},{name:'中学',address:''}],birth:{0}}";
            var dobj = FrameDLRObject.CreateInstanceFromat(json, DateTime.Now);
            var jarray = "[{name:'小学',address:''},{name:'中学',address:''}]";
            var aobj = FrameDLRObject.CreateArray(jarray);

            Console.WriteLine(dobj.tojsonstring());

        }
    }
}
