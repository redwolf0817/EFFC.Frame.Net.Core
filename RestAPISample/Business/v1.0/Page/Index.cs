using EFFC.Frame.Net.Module.Extend.EWRA.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestAPISample.Business.v1._0.Page
{
    public class Index : MyRestLogic
    {
        [EWRAAuth(false)]
        [EWRARoute("get", "/index")]
        [EWRARouteDesc("获取所有功能的列表")]
        [EWRAOutputDesc("返回结果", @"{
result:[数据集]
}")]
        object GetList()
        {
            SetContentType(EFFC.Frame.Net.Module.Extend.EWRA.Constants.RestContentType.HTML);
            SetViewPath("~/index.html");
            var data = new List<object>();
            data.Add(new
            {
                id = 1,
                name = "尹川"
            });
            data.Add(new
            {
                id = 2,
                name = "罗建"
            });
            return new
            {
                title = "武汉市仪表电子学校！",
                welcome = "欢迎来到武汉市仪表电子学校！",
                data
            };
        }
    }
}
