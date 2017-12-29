using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace EFFC.Frame.Net.WebControlLib.Navigation
{
    public class FrameworkSiteMap : Control, INamingContainer
    {

        public override void RenderControl(HtmlTextWriter writer)
        {
            Render(writer);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (DesignMode)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Id, this.ClientID);
                writer.AddAttribute(HtmlTextWriterAttribute.Name, this.UniqueID);
                writer.RenderBeginTag(HtmlTextWriterTag.Div);



                writer.RenderEndTag();
            }
            else
            {

            }
        }
    }
}
