
using EFFC.Frame.Net.Module.Extend.EBA;
using EFFC.Frame.Net.Module.Extend.EBA.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace B2B_BatchApplication.Business
{
    public class HWSync:ScheduleLogic
    {
        //HWRestCall hw = new HWRestCall();
        [EBAFrequence(EBAFrequenceAttribute.FrequenceType.Second, 20)]
        [EBADesc("PO同步")]
        [EBAGroup("hw", "华为发货功能使用")]
        [EBARepeatWhenException(typeof(NotSupportedException),2)]
        object SyncPO(dynamic args)
        {
            throw new NotSupportedException("测试");
            return true;//hw.POSync();
        }
        [EBAFrequence(EBAFrequenceAttribute.FrequenceType.Hour, 3)]
        [EBADesc("发货状态同步")]
        [EBAGroup("hw", "华为发货功能使用")]
        object SyncDelivery(dynamic args)
        {
            return true; ;
        }
        //先暂时不跑IMEI，IMEI现在跑的时间太慢，调整完毕再开启
        [EBAFrequence(EBAFrequenceAttribute.FrequenceType.Hour, 3)]
        [EBADesc("IMEI同步")]
        [EBAGroup("hw", "华为发货功能使用")]
        [EBAIsOpen(false)]
        object SyncIMEI(dynamic args)
        {
            return true; ;
        }
    }
}
