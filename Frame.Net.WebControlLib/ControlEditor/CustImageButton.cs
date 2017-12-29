using System;
using System.ComponentModel;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
//using Project.BaseItem;

namespace EFFC.Frame.Net.WebControlLib
{
    /// <summary>
    /// Expand ImageButton control
    /// Create ConfirmMsg attribute,Show the confirmMsg when the button onclick;
    /// Create CheckSaved attribute,check current page is or isn't saved when the button onclick
    /// <see cref="System.Web.UI.WebControls.ImageButton"/>
    /// </summary>
    [ToolboxData("<{0}:CustImageButton runat=\"server\"></{0}:CustImageButton>")]
    [ToolboxBitmapAttribute(typeof(ImageButton))]
    public class CustImageButton : ImageButton
    {
        /// <summary>
        /// UserType
        /// </summary>
        public enum EImageButtonType
        {
            /// <summary>
            /// View
            /// </summary>
            View,
            /// <summary>
            /// Edit
            /// </summary>
            Edit,
            /// <summary>
            /// Delete
            /// </summary>
            Delete,
        }

        #region property

        /// <summary>
        /// imagebutton type:view,edit,delete
        /// </summary>
        [Category("CustProperty"), Description("imagebutton摸")]
        public EImageButtonType ButtonType
        {
            set
            {
                this.SetButton(value);
            }
        }

        #endregion

        #region Private Method
        /// <summary>
        /// Show different images when the type is different
        /// </summary>
        /// <param name="ButtonType"></param>
        private void SetButton(EImageButtonType ButtonType)
        {
            if (ButtonType == EImageButtonType.View)
            {
                if (string.IsNullOrEmpty(this.ToolTip))
                    this.ToolTip = EImageButtonType.View.ToString();
                this.ImageUrl = "~/App_Themes/Default/icon/View.gif";
            }
            else if (ButtonType == EImageButtonType.Edit)
            {
                if (string.IsNullOrEmpty(this.ToolTip))
                    this.ToolTip = EImageButtonType.Edit.ToString();
                this.ImageUrl = "~/App_Themes/Default/icon/edit.gif";
            }
            else if (ButtonType == EImageButtonType.Delete)
            {
                if (string.IsNullOrEmpty(this.ToolTip))
                    this.ToolTip = EImageButtonType.Delete.ToString();
                this.ImageUrl = "~/App_Themes/Default/icon/delete.gif";
            }
        }
        #endregion
    }
}
