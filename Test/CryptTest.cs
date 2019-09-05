using System;
using System.Collections.Generic;
using System.Text;
using EFFC.Frame.Net.Base.Common;

namespace Test
{
    public class CryptTest
    {
        public static void Test()
        {
            var s = ComFunc.getMD5_String("123");
            var body = @"{
  ""USER_ID"":""oa_app_0003"",
  ""CUST_ID"":""1000131460"",
  ""STATUS"":""1"",
  ""PAGE_SIZE"":500,
  ""CURRENT_PAGE"":1,
  ""START_DATE"":""2018-04-25 00:00:00"",
  ""END_DATE"":""2018-05-25 00:00:00""
}";
            body = body.Replace("\r", "");
            var key = "MIICYwIBAAKBgQDdxOYZnligFD/9/0WuurksM1Py8iGRZChGl6vSKlLUMm6uZ0t09+CF0x6QbF+2z1UcAO+sZXuwb06zZ4rsoR++XTOpTCz8CTK5uSIcI8kv9vrOH5xi1gJXu1MB8VdBWc0w2OTp4+3Euom+cnswBtqV7qcpcV1J9XFClKBiBgI43wIDAQABAoGBALE9npE3BqnZxVMg4/ZD8Z6r3xo5/i4PGEljCsXLYauKKv9kOuBfA6ixFKpkkxKuHd1luifUa8iVDRdBVvYGXzQgfzUm430Ju069JUU/uOsfT/0rhBZ7gooCT5tR00TOdvwvhT+qFH4Mgww85XLMsN53paiFAS/fhMl8tYIoIhMhAkUA+LR0AKQEDuzffdiBQZa6WoOXtnYwA4W/D4An2cIe8i2wRUsfObhaLh4nY9oyFFrJ8FeO9EF5pCjuHLZQxPL1b5F+w7kCPQDkRi7Gj8c0H1Mxqn6n31Ygc3zU4i7/YqjsnopQ5LpYl1qUl926JRVFCjlcStx/me86ao/0NuRAN+Yv3VcCRQDxdOjmslZKU0jL+kXLctXsCLRjbi1BTjlniCmobaHzx83KCJwBQu0ytw3REMbsIhTKZYehtmutqBs8/vg9rhABSIWE6QI9AIylPU6z6X2Qy1ZvgMf3z/4Aieo0TdamOARKDliXBMVuw62IAGIfVQKLsRnPOhoYgxAP9g/2/h0fE+Fr0wJFANOEUqkMaQ9p2Z2rP46Zmo31K45VIDhG3Ygwhv04egfftrB+O5hUeEsNlhwxOkaexr6VwlldtqisanmhMIINpw23vLD9";
            var base64 = ComFunc.Base64Code(body);
            var sign = ComFunc.SHA256hash(base64 + "|" + key).ToLower();
        }
    }
}
