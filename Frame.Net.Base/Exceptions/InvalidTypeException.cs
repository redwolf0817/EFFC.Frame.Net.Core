using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Base.Exceptions
{
    /// <summary>
    /// 无效类型异常
    /// </summary>
    public class InvalidTypeException:Exception
    {
       /// <summary>
       /// Contructor
       /// </summary>
        public InvalidTypeException()
            : base()
        {
        }
        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="msg"></param>
        public InvalidTypeException(string msg)
            : base(msg)
        {
        }
    }
}
