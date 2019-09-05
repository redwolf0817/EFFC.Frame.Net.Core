using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Unit.DB.Parameters;
using EFFC.Frame.Net.Unit.DB.Unit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestAPISample.Unit
{
    public abstract class BaseDBUnit : IDBUnit<UnitParameter>
    {
        static Dictionary<string, FrameExposedClass> dTypes = new Dictionary<string, FrameExposedClass>();
        static object lockobj = new object();

        public Func<UnitParameter, dynamic> GetSqlFunc(string flag)
        {
            lock (lockobj)
            {
                if (!dTypes.ContainsKey(Name))
                {
                    dTypes.Add(Name, FrameExposedClass.From(this.GetType(), false, true));
                }
            }
            var methodname = flag;
            return (up) =>
            {
                var result = new object();
                var rtn = FrameDLRObject.CreateInstance();
                rtn.presql = "";
                rtn.sql = "";
                rtn.aftersql = "";
                rtn.orderby = "";
                rtn.spname = "";
                dTypes[Name].InvokeMethod(methodname, this, out result, new object[] { up, rtn });
                return rtn;
            };
        }

        string _name = "";
        public string Name
        {
            get
            {
                if (_name == "")
                {
                    _name = this.GetType().Name.ToLower().Replace("unit", "");
                }

                return _name;
            }
        }
    }
}
