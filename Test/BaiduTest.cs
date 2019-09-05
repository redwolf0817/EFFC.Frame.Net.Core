using EFFC.Frame.Net.Base.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Test
{
    class BaiduTest
    {
        static string APP_ID = "15224606";
        static string API_KEY = "oeylPOn2hvUM6NG4002zGE5f";
        static string SECRET_KEY = "VlhCvWwuYLWVfEWB5Uc8wqPrqilobny8";
        public static void test()
        {
            //UserAdd();
            //Search();
            GetGrouplist();
        }

        protected static void Detect()
        {
            // 设置APPID/AK/SK
            var client = new Baidu.Aip.Face.Face(API_KEY, SECRET_KEY);
            client.Timeout = 60000;  // 修改超时时间


            var image = ComFunc.StreamToBase64String(new FileStream("C:/Users/admin/Desktop/微信图片_20190104172518.jpg", FileMode.Open));

            var imageType = "BASE64";

            // 调用人脸检测，可能会抛出网络等异常，请使用try/catch捕获
            //var result = client.Detect(image, imageType);
            //Console.WriteLine(result);
            // 如果有可选参数
            var options = new Dictionary<string, object>{
        {"face_field", "age,beauty,expression,faceshape,gender,glasses,landmark,race,quality,facetype"},
        {"max_face_num", 2},
        {"face_type", "LIVE"}
    };
            // 带参数调用人脸检测
            var result = client.Detect(image, imageType, options);

            Console.WriteLine(result);
        }

        protected static void Search()
        {
            var client = new Baidu.Aip.Face.Face(API_KEY, SECRET_KEY);
            client.Timeout = 60000;  // 修改超时时间
            var image = "";
            using (var s = new FileStream("C:/Users/admin/Desktop/微信图片_20190107152711.jpg", FileMode.Open))
            {
                image = ComFunc.StreamToBase64String(s);
            }
           

            var imageType = "BASE64";

            var groupIdList = "F001";

            // 调用人脸搜索，可能会抛出网络等异常，请使用try/catch捕获
            var result = client.Search(image, imageType, groupIdList);
            Console.WriteLine(result);
            // 如果有可选参数
    //        var options = new Dictionary<string, object>{
    //    {"quality_control", "NORMAL"},
    //    {"liveness_control", "LOW"},
    //    {"user_id", "233451"},
    //    {"max_user_num", 3}
    //};
    //        // 带参数调用人脸搜索
    //        result = client.Search(image, imageType, groupIdList, options);
    //        Console.WriteLine(result);
        }

        protected static void UserAdd()
        {
            var client = new Baidu.Aip.Face.Face(API_KEY, SECRET_KEY);
            client.Timeout = 60000;  // 修改超时时间
            var image = ComFunc.StreamToBase64String(new FileStream("C:/Users/admin/Desktop/2.jpg", FileMode.Open));

            var imageType = "BASE64";

            var groupId = "F001";

            var userId = "F0001";

            // 调用人脸注册，可能会抛出网络等异常，请使用try/catch捕获
            var result = client.UserAdd(image, imageType, groupId, userId);
            Console.WriteLine(result);
    //        // 如果有可选参数
    //        var options = new Dictionary<string, object>{
    //    {"user_info", "user's info"},
    //    {"quality_control", "NORMAL"},
    //    {"liveness_control", "LOW"}
    //};
    //        // 带参数调用人脸注册
    //        result = client.UserAdd(image, imageType, groupId, userId, options);
    //        Console.WriteLine(result);
        }

        public static void FaceGetlist(string userId)
        {
            var client = new Baidu.Aip.Face.Face(API_KEY, SECRET_KEY);
            client.Timeout = 60000;  // 修改超时时间

            var groupId = "F001";

            // 调用获取用户人脸列表，可能会抛出网络等异常，请使用try/catch捕获
            var result = client.FaceGetlist(userId, groupId);
            Console.WriteLine(result);
        }

        public static void GroupGetusers()
        {
            var client = new Baidu.Aip.Face.Face(API_KEY, SECRET_KEY);
            client.Timeout = 60000;  // 修改超时时间
            var groupId = "F001";

            // 调用获取用户列表，可能会抛出网络等异常，请使用try/catch捕获
            //var result = client.GroupGetusers(groupId);
            //Console.WriteLine(result);
            // 如果有可选参数
            var options = new Dictionary<string, object>{
        {"start", 0},
        {"length", 50}
    };
            // 带参数调用获取用户列表
            var result = client.GroupGetusers(groupId, options);
            Console.WriteLine(result);
        }
        public static void PersonVerify()
        {
            var client = new Baidu.Aip.Face.Face(API_KEY, SECRET_KEY);
            client.Timeout = 60000;  // 修改超时时间
            var image = ComFunc.StreamToBase64String(new FileStream("C:/Users/admin/Desktop/2.jpg", FileMode.Open));

            var imageType = "BASE64";

            var idCardNumber = "420104198108172011";

            var name = "张三";

            // 调用身份验证，可能会抛出网络等异常，请使用try/catch捕获
            var result = client.PersonVerify(image, imageType, idCardNumber, name);
            Console.WriteLine(result);
            // 如果有可选参数
    //        var options = new Dictionary<string, object>{
    //    {"quality_control", "NORMAL"},
    //    {"liveness_control", "LOW"}
    //};
    //        // 带参数调用身份验证
    //        result = client.PersonVerify(image, imageType, idCardNumber, name, options);
    //        Console.WriteLine(result);
        }

        public static void GetGrouplist()
        {
            var client = new Baidu.Aip.Face.Face(API_KEY, SECRET_KEY);
            client.Timeout = 60000;  // 修改超时时间
            // 调用组列表查询，可能会抛出网络等异常，请使用try/catch捕获
            var result = client.GroupGetlist();
            Console.WriteLine(result);
        }
    }
}
