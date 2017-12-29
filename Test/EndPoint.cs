using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Test
{
    class EndPoint
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
    }
}
