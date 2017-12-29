using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.ComponentModel;
using System.Drawing;

namespace EFFC.Frame.Net.WebControlLib
{
    /// <summary>
    /// Expand Image control
    /// </summary>
    [ToolboxBitmapAttribute(typeof(System.Web.UI.WebControls.Image))]
    public class CustImage : System.Web.UI.WebControls.Image
    {
        private bool _IsSetSize;
        /// <summary>
        /// is or isn't set image's size(set max height and max width)
        /// </summary>
        public bool IsSetSize
        {
            get { return _IsSetSize; }
            set { _IsSetSize = value; }
        }

        /// <summary>
        /// when image is disable then show this image
        /// </summary>
        public String BlankImageUrl
        {
            get
            {
                return ViewState["BlankImageUrl"] == null ? "~/App_Themes/Default/images/icon/noImage.gif" : ViewState["BlankImageUrl"].ToString();
            }
            set
            {
                ViewState["BlankImageUrl"] = value;
            }
        }
        private bool m_IsShowEffect;
        /// <summary>
        /// 
        /// </summary>
        [Category("CustProperty"),Description("Is or isn't open the image's show type,default is false"),DefaultValue(false)]
        public bool IsShowEffect
        {
            get { return m_IsShowEffect; }
            set { m_IsShowEffect = value; }
        }

        private string m_Href;
        /// <summary>
        /// Image link url
        /// </summary>
        [Category("CustProperty"), Description("Image link url")]
        public string Href
        {
            get { return m_Href; }
            set { m_Href = value; }
        }

        /// <summary>
        /// Override Render method.set image style by custom attribute
        /// </summary>
        /// <param name="writer"></param>
        protected override void Render(HtmlTextWriter writer)
        {
            if (this.IsShowEffect)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Href, this.Href);
                writer.AddAttribute(HtmlTextWriterAttribute.Rel, "lightbox[example]");
                writer.RenderBeginTag(HtmlTextWriterTag.A);
                this.Attributes.Add("onload", "initLightbox();");
            }

            if (this.IsSetSize)
            {
                this.Attributes.Add("onload", string.Format("SetImgSize(this,{0},{1});this.onload = null;", this.Width.Value, this.Height.Value));
            }

            this.Attributes.Add("onerror", string.Format("this.src='{0}'", this.ResolveUrl(BlankImageUrl)));
            base.Render(writer);
            if (this.IsShowEffect)
            {
                writer.RenderEndTag();
            }

        }
    }
}
