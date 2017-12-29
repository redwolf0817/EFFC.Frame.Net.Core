using EFFC.Frame.Net.Base.AttributeDefine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Base.ResouceManage.JsEngine
{
    public class ConsoleObject : BaseHostJsObject
    {
        StringBuilder sb = new StringBuilder();
        public override string Description
        {
            get { return "控制台输出"; }
        }

        public override string Name
        {
            get { return "ConsoleS"; }
        }
        [Desc("往控制台写入信息")]
        public void Write(string msg)
        {
            sb.Append(msg);
        }
        [Desc("往控制台写入信息")]
        public void WriteLine(string msg)
        {
            sb.AppendLine(msg);
        }
        [Desc("清空控制台信息")]
        public void Clear()
        {
            sb.Clear();
        }
        [Desc("输出控制台的信息")]
        public string OutMsg
        {
            get
            {
                return sb.ToString();
            }
        }
    }
}
