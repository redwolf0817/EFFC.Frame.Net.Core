
namespace EFFC.Frame.Net.Business.Unit.DOD
{
    public partial class DODBaseUnit 
    {

        private InputPHelper _input;
        /// <summary>
        /// 传入参数操作
        /// </summary>
        public virtual InputPHelper InputP
        {
            get
            {
                if (_input == null)
                    _input = new InputPHelper(this);
                return _input;
            }
        }

        public class InputPHelper
        {
            DODBaseUnit _unit;
            public InputPHelper(DODBaseUnit logic)
            {
                _unit = logic;
            }

            public object this[string key]
            {
                get
                {
                    return _unit._p[key];
                }
            }
            /// <summary>
            /// 当前需要的属性名称
            /// </summary>
            public string CurrentPropertyName
            {
                get
                {
                    return _unit._p.PropertyName;
                }
            }
        }
    }
}
