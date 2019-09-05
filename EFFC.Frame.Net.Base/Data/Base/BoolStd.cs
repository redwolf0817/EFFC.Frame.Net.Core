using EFFC.Frame.Net.Base.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Base.Data.Base
{
    /// <summary>
    /// Bool类型基本数据
    /// </summary>
    public class BoolStd:StandardData<bool>
    {
        /// <summary>
        /// default constructor
        /// </summary>
        public BoolStd()
        {
        }
        /// <summary>
        /// default constructor with default value
        /// </summary>
        /// <param name="o"></param>
        public BoolStd(string o)
        {
            if (IsBool(o))
            {
                this.Value = bool.Parse(o);
            }
        }
        /// <summary>
        /// default constructor with default value
        /// </summary>
        /// <param name="o"></param>
        public BoolStd(bool o)
        {
            this.Value = o;
        }
        /// <summary>
        /// default constructor with default value
        /// </summary>
        /// <param name="o"></param>
        public BoolStd(bool? o)
        {
            if (o != null)
                this.Value = o.Value;
        }
        /// <summary>
        /// 隐性转化
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static implicit operator BoolStd(bool o)
        {
            return BoolStd.ParseStd(o);
        }
        /// <summary>
        /// ==判斷
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public static bool operator ==(BoolStd o1, BoolStd o2)
        {
            object o = o1;
            object oo = o2;
            if (o == null || oo == null)
            {
                return o == oo;
            }
            else
            {
                return o1.Value == o2.Value;
            }
        }
        /// <summary>
        /// !=判斷
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public static bool operator !=(BoolStd o1, BoolStd o2)
        {
            object o = o1;
            object oo = o2;
            if (o == null || oo == null)
            {
                return o != oo;
            }
            else
            {
                return o1.Value != o2.Value;
            }
        }
        /// <summary>
        /// bool判斷
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static bool IsBool(object o)
        {
            if (o == null || o == DBNull.Value)
            {
                return false;
            }
            else if (ComFunc.nvl(o) == "1")
            {
                return true;
            }
            else if (ComFunc.nvl(o) == "0")
            {
                return true;
            }
            else
            {
                bool t1;
                return bool.TryParse(o.ToString(), out t1);
            }
        }
        /// <summary>
        /// 判断是否为bool，如果不是bool则返回指定的默认值，否则返回原值
        /// </summary>
        /// <param name="o"></param>
        /// <param name="defaultvalue">默认为0</param>
        /// <returns></returns>
        public static bool IsNotBoolThen(object o, bool defaultvalue = false)
        {
            return IsBool(o) ? ParseStd(o).Value : defaultvalue;
        }
        /// <summary>
        /// 将bool值转化为其它类型的值
        /// </summary>
        /// <param name="v">bool值</param>
        /// <param name="is_true_value">如果为true则返回该值</param>
        /// <param name="is_false_value">如果为false则返回该值</param>
        /// <returns></returns>
        public static object ConvertTo(object v,object is_true_value,object is_false_value)
        {
            return IsNotBoolThen(v) ? is_true_value : is_false_value;
        }
        public override bool Equals(object obj)
        {
            if (obj is bool)
            {
                return this.Value == (bool)obj;
            }
            else if (obj is BoolStd)
            {
                return this.Value == ((BoolStd)obj).Value;
            }
            else
            {
                return base.Equals(obj);
            }
        }

        /// <summary>
        /// 将Object转化成boolStd数据
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static BoolStd ParseStd(object o)
        {
            if (IsBool(o))
            {
                BoolStd rtn = new BoolStd();
                if(ComFunc.nvl(o) == "1")
                {
                    rtn.Value = true;
                }else if(ComFunc.nvl(o) == "0")
                {
                    rtn.Value = false;
                }
                else
                {
                    rtn.Value = bool.Parse(o.ToString());
                }
                return rtn;
            }
            else
            {
                return null;
            }
        }

    }
}
