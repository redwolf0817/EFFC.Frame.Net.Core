using System;
using System.Collections.Generic;
using System.Text;
using System.Resources;

namespace EFFC.Frame.Net.Base.Data
{
    public class DecimalStd : StandardData<decimal>
    {
        /// <summary>
        /// default construct
        /// </summary>
        public DecimalStd()
        {
        }
        /// <summary>
        /// constructor with default value
        /// </summary>
        /// <param name="o"></param>
        public DecimalStd(decimal o)
        {
            this.Value = o;
        }
        /// <summary>
        /// constructor with default object
        /// </summary>
        /// <param name="o"></param>
        public DecimalStd(decimal? o)
        {
            this.Value = o.Value;
        }
        /// <summary>
        /// 隱含轉換成decimal
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static implicit operator decimal(DecimalStd o)
        {
            return o.Value;
        }
        /// <summary>
        /// 強轉成float類型
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static explicit operator float(DecimalStd o)
        {
            return float.Parse(o.Value.ToString());
        }
        /// <summary>
        /// 強轉成double類型
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static explicit operator double(DecimalStd o)
        {
            return double.Parse(o.Value.ToString());
        }
        /// <summary>
        /// 隱含轉換成decimalStd類型
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static implicit operator DecimalStd(float o)
        {
            return DecimalStd.ParseStd(o);
        }
        /// <summary>
        /// 隱含轉換成decimalStd類型
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static implicit operator DecimalStd(double o)
        {
            return DecimalStd.ParseStd(o);
        }
        /// <summary>
        /// 隱含轉換成decimalStd類型
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static implicit operator DecimalStd(int o)
        {
            return DecimalStd.ParseStd(o);
        }
        /// <summary>
        /// 隱含轉換成decimalStd類型
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static implicit operator DecimalStd(decimal o)
        {
            return DecimalStd.ParseStd(o);
        }
        /// <summary>
        /// 正號運算
        /// </summary>
        /// <param name="o1"></param>
        /// <returns></returns>
        public static DecimalStd operator +(DecimalStd o1)
        {
            return new DecimalStd(o1.Value);
        }
        /// <summary>
        /// 加運算
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public static DecimalStd operator +(DecimalStd o1, DecimalStd o2)
        {
            return new DecimalStd(o1.Value + o2.Value);
        }
        /// <summary>
        /// 加運算
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public static DecimalStd operator +(DecimalStd o1, decimal o2)
        {
            return DecimalStd.ParseStd(o1.Value + o2);
        }
        /// <summary>
        /// 加運算
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public static DecimalStd operator +(DecimalStd o1, int o2)
        {
            return DecimalStd.ParseStd(o1.Value + o2);
        }
        /// <summary>
        /// 加運算
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public static DecimalStd operator +(DecimalStd o1, float o2)
        {
            return DecimalStd.ParseStd(o1.Value + decimal.Parse(o2.ToString()));
        }
        /// <summary>
        /// 加運算
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public static DecimalStd operator +(DecimalStd o1, double o2)
        {
            return DecimalStd.ParseStd(o1.Value + decimal.Parse(o2.ToString()));
        }
        /// <summary>
        /// ++運算
        /// </summary>
        /// <param name="o1"></param>
        /// <returns></returns>
        public static DecimalStd operator ++(DecimalStd o1)
        {
            o1.Value++;
            return o1;
        }
        /// <summary>
        /// 负号運算
        /// </summary>
        /// <param name="o1"></param>
        /// <returns></returns>
        public static DecimalStd operator -(DecimalStd o1)
        {
            return new DecimalStd(-o1.Value);
        }
        /// <summary>
        /// 减運算
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public static DecimalStd operator -(DecimalStd o1, DecimalStd o2)
        {
            return new DecimalStd(o1.Value - o2.Value);
        }
        /// <summary>
        /// 减運算
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public static DecimalStd operator -(DecimalStd o1, decimal o2)
        {
            return DecimalStd.ParseStd(o1.Value - o2);
        }
        /// <summary>
        /// 减運算
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public static DecimalStd operator -(DecimalStd o1, int o2)
        {
            return DecimalStd.ParseStd(o1.Value - o2);
        }
        /// <summary>
        /// 减運算
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public static DecimalStd operator -(DecimalStd o1, float o2)
        {
            return DecimalStd.ParseStd(o1.Value - decimal.Parse(o2.ToString()));
        }
        /// <summary>
        /// 减運算
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public static DecimalStd operator -(DecimalStd o1, double o2)
        {
            return DecimalStd.ParseStd(o1.Value - decimal.Parse(o2.ToString()));
        }
        /// <summary>
        /// --運算
        /// </summary>
        /// <param name="o1"></param>
        /// <returns></returns>
        public static DecimalStd operator --(DecimalStd o1)
        {
            o1.Value--;
            return o1;
        }
        /// <summary>
        /// 乘法運算
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public static DecimalStd operator *(DecimalStd o1, DecimalStd o2)
        {
            return DecimalStd.ParseStd(o1.Value * o2.Value);
        }
        /// <summary>
        /// 乘法運算
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public static DecimalStd operator *(DecimalStd o1, decimal o2)
        {
            return DecimalStd.ParseStd(o1.Value * o2);
        }
        /// <summary>
        /// 乘法運算
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public static DecimalStd operator *(DecimalStd o1, int o2)
        {
            return DecimalStd.ParseStd(o1.Value * o2);
        }
        /// <summary>
        /// 乘法運算
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public static DecimalStd operator *(DecimalStd o1, float o2)
        {
            return DecimalStd.ParseStd(o1.Value * decimal.Parse(o2.ToString()));
        }
        /// <summary>
        /// 乘法運算
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public static DecimalStd operator *(DecimalStd o1, double o2)
        {
            return DecimalStd.ParseStd(o1.Value * decimal.Parse(o2.ToString()));
        }
        /// <summary>
        /// 除法運算
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public static DecimalStd operator /(DecimalStd o1, DecimalStd o2)
        {
            return new DecimalStd(o1.Value / o2.Value);
        }
        /// <summary>
        /// 除法運算
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public static DecimalStd operator /(DecimalStd o1, decimal o2)
        {
            return DecimalStd.ParseStd(o1.Value / o2);
        }
        /// <summary>
        /// 除法運算
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public static DecimalStd operator /(DecimalStd o1, int o2)
        {
            return DecimalStd.ParseStd(o1.Value / o2);
        }
        /// <summary>
        /// 除法運算
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public static DecimalStd operator /(DecimalStd o1, float o2)
        {
            return DecimalStd.ParseStd(o1.Value / decimal.Parse(o2.ToString()));
        }
        /// <summary>
        /// 除法運算
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public static DecimalStd operator /(DecimalStd o1, double o2)
        {
            return DecimalStd.ParseStd(o1.Value / decimal.Parse(o2.ToString()));
        }

        /// <summary>
        /// %運算
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public static DecimalStd operator %(DecimalStd o1, DecimalStd o2)
        {
            return new DecimalStd(o1.Value % o2.Value);
        }
        /// <summary>
        /// %運算
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public static DecimalStd operator %(DecimalStd o1, decimal o2)
        {
            return DecimalStd.ParseStd(o1.Value % o2);
        }
        /// <summary>
        /// %運算
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public static DecimalStd operator %(DecimalStd o1, int o2)
        {
            return DecimalStd.ParseStd(o1.Value % o2);
        }
        /// <summary>
        /// %運算
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public static DecimalStd operator %(DecimalStd o1, float o2)
        {
            return DecimalStd.ParseStd(o1.Value % decimal.Parse(o2.ToString()));
        }
        /// <summary>
        /// %運算
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public static DecimalStd operator %(DecimalStd o1, double o2)
        {
            return DecimalStd.ParseStd(o1.Value % decimal.Parse(o2.ToString()));
        }
        /// <summary>
        /// !=判斷
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public static bool operator !=(DecimalStd o1, DecimalStd o2)
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
        /// ==判斷
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public static bool operator ==(DecimalStd o1, DecimalStd o2)
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
        /// >=判斷
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public static bool operator >=(DecimalStd o1, DecimalStd o2)
        {
            return o1.Value >= o2.Value;
        }

        /// <summary>
        /// 小於等於判斷
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public static bool operator <=(DecimalStd o1, DecimalStd o2)
        {
            return o1.Value <= o2.Value;
        }

        /// <summary>
        /// 小于判斷
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public static bool operator <(DecimalStd o1, DecimalStd o2)
        {
            return o1.Value < o2.Value;
        }

        /// <summary>
        /// 大于判斷
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public static bool operator >(DecimalStd o1, DecimalStd o2)
        {
            return o1.Value > o2.Value;
        }

        /// <summary>
        /// 将Object转化成DecimalStd数据
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static DecimalStd ParseStd(object o)
        {
            if (IsDecimal(o))
            {
                DecimalStd rtn = new DecimalStd();
                rtn.Value = Decimal.Parse(o.ToString());
                return rtn;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Decimal判斷
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static bool IsDecimal(object o)
        {
            if (o == null || o == DBNull.Value)
            {
                return false;
            }
            else if (o.ToString().Length <= 0)
            {
                return false;
            }
            else
            {
                decimal t1;
                return decimal.TryParse(o.ToString(), out t1);
            }
        }
        /// <summary>
        /// 判断是否为decimal，如果不是decimal则返回指定的默认值，否则返回原值
        /// </summary>
        /// <param name="o"></param>
        /// <param name="defaultvalue">默认为0</param>
        /// <returns></returns>
        public static decimal IsNotDecimalThen(object o, decimal defaultvalue = 0)
        {
            return IsDecimal(o) ? ParseStd(o).Value : defaultvalue;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public string ToString(IFormatProvider format)
        {
            return Value.ToString(format);
        }
        /// <summary>
        /// equals
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is DecimalStd || obj is decimal)
                return this == DecimalStd.ParseStd(obj);
            else
                return false;
        }
        /// <summary>
        /// Hash Code
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
