using EFFC.Frame.Net.Base.Data.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Test.APIRest
{
    public class PointLogic
    {
        string _name = "";
        /// <summary>
        /// logic的名称，busi模块根据该属性来调用对应的logic
        /// </summary>
        public virtual string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_name))
                {
                    _name = this.GetType().Name.ToLower();
                    if (_name.EndsWith("logic")) _name = _name.Substring(0, _name.Length - 5);
                    if (_name.EndsWith("go")) _name = _name.Substring(0, _name.Length - 2);
                    if (_name.EndsWith("view")) _name = _name.Substring(0, _name.Length - 4);
                }

                return _name;
            }
        }

        public virtual List<object> get()
        {
            return new List<object>();
        }

        public virtual object get(string id)
        {
            return FrameDLRObject.CreateInstance();
        }

        public virtual object put()
        {
            return new
            {
                id=""
            };
        }
        public virtual object post()
        {
            return new
            {
                id = ""
            };
        }

        public virtual object patch(string id)
        {
            return new
            {
                id = ""
            };
        }

        public virtual bool delete(string id)
        {
            return true;
        }
    }
}
