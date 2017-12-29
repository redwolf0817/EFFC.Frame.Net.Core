using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Data.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Data.UnitData
{
    public class DOCollection:DataCollection
    {
        /// <summary>
        /// 获取的属性值
        /// </summary>
        public PropertyValueDefine PropertyValue
        {
            get;
            set;
        }
        /// <summary>
        /// 获取的列表值
        /// </summary>
        public List<FrameDLRObject> ListValue
        {
            get;
            set;
        }

        public class PropertyValueDefine:ICloneable
        {
            object _v = null;
            public PropertyValueDefine(object v)
            {
                _v = v;
            }
            public object Value
            {
                get
                {
                    return _v;
                }
            }

            public string StringValue
            {
                get
                {
                    return ComFunc.nvl(_v);
                }
            }

            public double DoubleValue
            {
                get
                {
                    if (DoubleStd.IsDouble(_v))
                    {
                        return DoubleStd.ParseStd(_v).Value;
                    }
                    else
                    {
                        throw new InvalidCastException("Can't cast value to double");
                    }
                }
            }
            public decimal DecimalValue
            {
                get
                {
                    if (DecimalStd.IsDecimal(_v))
                    {
                        return DecimalStd.ParseStd(_v).Value;
                    }
                    else
                    {
                        throw new InvalidCastException("Can't cast value to decimal");
                    }
                }
            }
            public int IntValue
            {
                get
                {
                    if (IntStd.IsInt(_v))
                    {
                        return IntStd.ParseStd(_v).Value;
                    }
                    else
                    {
                        throw new InvalidCastException("Can't cast value to int");
                    }
                }
            }
            public bool BoolValue
            {
                get
                {
                    return ToValue<bool>();
                }
            }

            public T ToValue<T>()
            {
                if (_v != null)
                {
                    if (_v is T)
                    {
                        return (T)_v;
                    }
                    else
                    {
                        throw new InvalidCastException(string.Format("Can't cast value to {0}", typeof(T).FullName));
                    }
                }
                else
                {
                    return default(T);
                }
            }

            public object Clone()
            {
                if(this._v is ICloneable)
                {
                   return new PropertyValueDefine(((ICloneable)_v).Clone());
                }else
                {
                    throw new InvalidOperationException("The value is not ICloneable object,the opration canceled");
                }
            }

            public DateTime ToDateTime
            {
                get
                {
                    if (Value is DateTime)
                    {
                        return (DateTime)Value;
                    }
                    else if(DateTimeStd.IsDateTime(Value))
                    {
                        return DateTimeStd.ParseStd(Value);
                    }
                    else
                    {
                        throw new InvalidCastException(string.Format("Can't cast value to {0}", typeof(DateTime).FullName));
                    }
                }
            }

            
        }

        public override object Clone()
        {
            var rtn = this.Clone<DOCollection>();
            rtn.PropertyValue = (PropertyValueDefine)this.PropertyValue.Clone();
            rtn.ListValue = new List<FrameDLRObject>();
            foreach(var item in this.ListValue)
            {
                rtn.ListValue.Add((FrameDLRObject)(item.Clone()));
            }
             return rtn;
        }
    }
}
