using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Data.FlowData
{
    /// <summary>
    /// 流程版本号模型
    /// </summary>
    public class FlowVersion
    {
        /// <summary>
        /// 主版本号
        /// </summary>
        public int Main { get; set; }
        /// <summary>
        /// 子版本号
        /// </summary>
        public int Sub { get; set; }
        /// <summary>
        /// 扩展版本号
        /// </summary>
        public string Extention { get; set; }

        public static bool operator ==(FlowVersion o1, FlowVersion o2)
        {
            if (o1 == null && o2 == null)
            {
                return false;
            }
            FlowVersion o = o1;
            FlowVersion oo = o2;

            if (o.Main == oo.Main && o.Sub == oo.Sub)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool operator >(FlowVersion o1, FlowVersion o2)
        {
            if (o1 == null && o2 == null)
            {
                return false;
            }
            FlowVersion o = o1;
            FlowVersion oo = o2;

            if (o.Main > oo.Main)
            {
                return true;
            }
            else if (o.Main == oo.Main)
            {
                if (o.Sub > oo.Sub)
                {
                    return true;
                }
                else if (o.Sub == oo.Sub)
                {
                    var cresult = o.Extention.CompareTo(oo.Extention);
                    if (cresult > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public static bool operator <(FlowVersion o1, FlowVersion o2)
        {
            return o2 > o1;
        }

        public static bool operator >=(FlowVersion o1, FlowVersion o2)
        {
            if (o1 == null && o2 == null)
            {
                return false;
            }
            FlowVersion o = o1;
            FlowVersion oo = o2;

            if (o.Main >= oo.Main)
            {
                if (o.Sub >= oo.Sub)
                {
                    var cresult = o.Extention.CompareTo(oo.Extention);
                    if (cresult >= 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static bool operator <=(FlowVersion o1, FlowVersion o2)
        {
            return o2 >= o1;
        }

        public static bool operator !=(FlowVersion o1, FlowVersion o2)
        {
            return !(o1 == o2);
        }

        public override string ToString()
        {
            return Main + "." + Sub + "." + Extention;
        }

        public static FlowVersion Parse(string str)
        {
            FlowVersion rtn = new FlowVersion();
            if (ComFunc.nvl(str) != "")
            {
                var ss = str.Split('.');
                if (ss.Length >=2)
                {
                    if (IntStd.IsInt(ss[0]))
                    {
                        rtn.Main = IntStd.ParseStd(ss[0]);
                    }
                    else
                    {
                        throw new InvalidCastException("flow version formated failed:lack of main");
                    }

                    if (IntStd.IsInt(ss[1]))
                    {
                        rtn.Sub = IntStd.ParseStd(ss[1]);
                    }
                    else
                    {
                        throw new InvalidCastException("flow version formated failed:lack of sub");
                    }

                    if (ss.Length > 2)
                    {
                        rtn.Extention = ss[2];
                    }
                }
                else
                {
                    throw new InvalidCastException("flow version formated failed:the string's format is not \"main.sub[.extention]\"");
                }
                
            }
            return rtn;

        }
    }
}
