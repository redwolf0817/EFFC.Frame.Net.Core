using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.WebControlLib.Interfaces;

namespace EFFC.Frame.Net.WebControlLib.TypeConvert
{
    public class EnumPropertieByActionTypeConvert : StringConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType.Name == typeof(PropertiesByAction).Name)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            return ComFunc.EnumParse<PropertiesByAction>(ComFunc.nvl(value));
        }
    }
}
