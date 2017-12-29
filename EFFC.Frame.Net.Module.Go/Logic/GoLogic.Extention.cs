using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Module.Extend.WebGo.Logic
{
    public abstract partial class GoLogic
    {
        Extentions _ext = null;
        public Extentions ExtFunc
        {
            get
            {
                if (_ext == null) _ext = new Extentions(this);
                return _ext;
            }


        }
        public class Extentions
        {
            GoLogic _logic;

            public Extentions(GoLogic logic)
            {
                _logic = logic;
            }

        }
    }
}
