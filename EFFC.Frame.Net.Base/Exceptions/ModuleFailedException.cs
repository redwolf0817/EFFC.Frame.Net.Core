using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Base.Exceptions
{
    public class ModuleFailedException:Exception
    {
        public ModuleFailedException() : base()
        {

        }
        public ModuleFailedException(string s) : base(s)
        {

        }
    }
}
