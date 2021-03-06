using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Base.Data
{
    public class FloatStd : StandardData<float>
    {
        public FloatStd()
        {
        }

        public FloatStd(float? o)
        {
            if (o != null)
                this.Value = o.Value;
        }

        public FloatStd(float o)
        {
            this.Value = o;
        }
        public override bool Equals(object obj)
        {
            if (obj is float)
            {
                return this == (float)obj;
            }
            else if (obj is FloatStd)
            {
                return this == (FloatStd)obj;
            }
            else
            {
                return base.Equals(obj);
            }
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static implicit operator FloatStd(double o)
        {
            return FloatStd.ParseStd(o);
        }
        public static implicit operator FloatStd(float o)
        {
            return FloatStd.ParseStd(o);
        }
        public static implicit operator FloatStd(decimal o)
        {
            return FloatStd.ParseStd(o);
        }
        public static implicit operator FloatStd(int o)
        {
            return FloatStd.ParseStd(o);
        }
        public static explicit operator double(FloatStd o)
        {
            return o.Value;
        }
        public static implicit operator float(FloatStd o)
        {
            return float.Parse(o.Value.ToString());
        }
        public static explicit operator int(FloatStd o)
        {
            return int.Parse(o.Value.ToString());
        }
        /// <summary>
        /// 正号運算
        /// </summary>
        /// <param name="o1"></param>
        /// <returns></returns>
        public static FloatStd operator +(FloatStd o1)
        {
            return new FloatStd(o1.Value);
        }
        /// <summary>
        /// 加運算
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public static FloatStd operator +(FloatStd o1, FloatStd o2)
        {
            return new FloatStd(o1.Value + o2.Value);
        }
        /// <summary>
        /// 负号運算
        /// </summary>
        /// <param name="o1"></param>
        /// <returns></returns>
        public static FloatStd operator -(FloatStd o1)
        {
            return new FloatStd(-o1.Value);
        }
        /// <summary>
        /// 减運算
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public static FloatStd operator -(FloatStd o1, FloatStd o2)
        {
            return new FloatStd(o1.Value - o2.Value);
        }
        /// <summary>
        /// 乘法運算
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public static DoubleStd operator *(FloatStd o1, FloatStd o2)
        {
            return new DoubleStd((double)o1.Value * o2.Value);
        }
        /// <summary>
        /// 除法運算
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public static DoubleStd operator /(FloatStd o1, FloatStd o2)
        {
            return new DoubleStd((double)o1.Value / o2.Value);
        }

        /// <summary>
        /// ++運算
        /// </summary>
        /// <param name="o1"></param>
        /// <returns></returns>
        public static FloatStd operator ++(FloatStd o1)
        {
            o1.Value++;
            return o1;
        }

        /// <summary>
        /// --運算
        /// </summary>
        /// <param name="o1"></param>
        /// <returns></returns>
        public static FloatStd operator --(FloatStd o1)
        {
            o1.Value--;
            return o1;
        }

        /// <summary>
        /// %運算
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public static FloatStd operator %(FloatStd o1, FloatStd o2)
        {
            return new FloatStd(o1.Value % o2.Value);
        }
        /// <summary>
        /// !=判斷
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public static bool operator !=(FloatStd o1, FloatStd o2)
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
        public static bool operator ==(FloatStd o1, FloatStd o2)
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
        public static bool operator >=(FloatStd o1, FloatStd o2)
        {
            return o1.Value >= o2.Value;
        }

        /// <summary>
        /// 小於等於判斷
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public static bool operator <=(FloatStd o1, FloatStd o2)
        {
            return o1.Value <= o2.Value;
        }

        /// <summary>
        /// 小於判斷
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public static bool operator <(FloatStd o1, FloatStd o2)
        {
            return o1.Value < o2.Value;
        }

        /// <summary>
        /// 大於判斷
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public static bool operator >(FloatStd o1, FloatStd o2)
        {
            return o1.Value > o2.Value;
        }

        /// <summary>
        /// 将Object转化成FloatStd数据
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static FloatStd ParseStd(object o)
        {
            if (IsFloat(o))
            {
                FloatStd rtn = new FloatStd();
                rtn.Value = float.Parse(o.ToString());
                return rtn;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// float判斷
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static bool IsFloat(object o)
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
                float t1;
                return float.TryParse(o.ToString(), out t1);
            }
        }
        /// <summary>
        /// 判断是否为float，如果不是float则返回指定的默认值，否则返回原值
        /// </summary>
        /// <param name="o"></param>
        /// <param name="defaultvalue">默认为0</param>
        /// <returns></returns>
        public static float IsNotFloatThen(object o, float defaultvalue = 0)
        {
            return IsFloat(o) ? ParseStd(o).Value : defaultvalue;
        }
    }
}
