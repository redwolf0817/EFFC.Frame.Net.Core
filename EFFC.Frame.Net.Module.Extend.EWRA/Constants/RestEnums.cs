using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Module.Extend.EWRA.Constants
{
    /// <summary>
    /// Rest状态码
    /// </summary>
    public enum RestStatusCode
    {
        /// <summary>
        /// 未设置
        /// </summary>
        NONE = 0,
        /// <summary>
        ///  [GET]：服务器成功返回用户请求的数据
        /// </summary>
        OK = 200,
        /// <summary>
        ///  [POST/PUT/PATCH]：用户新建或修改数据成功
        /// </summary>
        CREATED = 201,
        /// <summary>
        /// [*]：表示一个请求已经进入后台排队（异步任务）
        /// </summary>
        ACCEPTED = 202,
        /// <summary>
        /// [DELETE]：用户删除数据成功
        /// </summary>
        NO_CONTENT = 204,
        /// <summary>
        /// [POST/PUT/PATCH]：用户发出的请求有错误，服务器没有进行新建或修改数据的操作
        /// </summary>
        INVALID_REQUEST = 400,
        /// <summary>
        /// [*]：表示用户没有权限（令牌、用户名、密码错误）
        /// </summary>
        UNAUTHORIZED = 401,
        /// <summary>
        /// [*] 表示用户得到授权（与401错误相对），但是访问是被禁止的
        /// </summary>
        FORBIDDEN = 403,
        /// <summary>
        /// [*]：用户发出的请求针对的是不存在的记录，服务器没有进行操作
        /// </summary>
        NOT_FOUND = 404,
        /// <summary>
        /// [GET]：用户请求的格式不可得（比如用户请求JSON格式，但是只有XML格式）或不接受当前的处理方式
        /// </summary>
        NOT_ACCEPTABLE = 406,
        /// <summary>
        /// [GET]：用户请求的资源被永久删除或未找到，且不会再得到的
        /// </summary>
        GONE = 410,
        /// <summary>
        /// [POST/PUT/PATCH] 当创建一个对象时，发生一个验证错误
        /// </summary>
        UNPROCESABLE_ENTITY = 422,
        /// <summary>
        /// [*]：服务器发生错误，用户将无法判断发出的请求是否成功
        /// </summary>
        INTERNAL_SERVER_ERROR = 500
    }
    /// <summary>
    /// Rest请求返回的contenttype类型
    /// </summary>
    public enum RestContentType
    {
        /// <summary>
        /// 返回json数据格式
        /// </summary>
        JSON,
        /// <summary>
        /// 返回文本
        /// </summary>
        TXT
    }
}
