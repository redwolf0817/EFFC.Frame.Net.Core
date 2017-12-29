using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Parameter;
using EFFC.Frame.Net.Base.ResouceManage;
using EFFC.Frame.Net.Base.Token;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Data.Parameters
{
    public class ConsoleParameter:ParameterStd
    {
        /// <summary>
        /// DBConnectionString
        /// </summary>
        public string DBConnectionString
        {
            get
            {
                return ComFunc.nvl(this[DomainKey.CONFIG, ParameterKey.DBCONNECT_STRING]);
            }
            set
            {
                this[DomainKey.CONFIG, ParameterKey.DBCONNECT_STRING] = value;
            }
        }
        /// <summary>
        /// 请求的logic
        /// </summary>
        public string Logic
        {
            get
            {
                return ComFunc.nvl(GetValue(ParameterKey.LOGIC));
            }
            set
            {
                SetValue(ParameterKey.LOGIC, value);
            }
        }
        /// <summary>
        /// 请求的action
        /// </summary>
        public string Action
        {
            get
            {
                return ComFunc.nvl(GetValue(ParameterKey.ACTION));
            }
            set
            {
                SetValue(ParameterKey.ACTION, value);
            }
        }

        public TextWriter Out
        {
            get
            {
                if(this["out"] == null){
                    this["out"] = Console.Out;
                }
                return (TextWriter)this["out"];
            }
        }

        public override object Clone()
        {
            return this.Clone<ConsoleParameter>();
        }
    }
}
