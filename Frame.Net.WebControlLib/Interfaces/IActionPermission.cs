using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EFFC.Frame.Net.WebControlLib.Interfaces
{
    public enum PropertiesByAction
    {
        Enable,
        Visable
    }

    /// <summary>
    /// 權限控件需要根據授權的ActionID進行權限設定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="authorizedActionIDs">被授權的ActionID，可以為多個，麼個之間用口號分割</param>
    public delegate void ActionSettingEvent(object sender, string authorizedActionIDs);

    public interface IActionPermission
    {
        /// <summary>
        /// 權限設定的ActionID，允許多個ActionID，每個之間口號分割
        /// </summary>
        string ActionID { set; get; }

        event ActionSettingEvent SetAuthoritiesByAction;

        /// <summary>
        /// 功過ActionID進行本控件的權限控制
        /// </summary>
        /// <param name="authorizedActionid">被授權的ActionID</param>
        void ControlByPermission(string authorizedActionid);
    }
}
