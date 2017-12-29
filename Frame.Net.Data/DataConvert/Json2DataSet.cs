using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Json;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Interfaces.DataConvert;
using EFFC.Frame.Net.Base.Common;
using Newtonsoft.Json;
using System.IO;
using EFFC.Frame.Net.Base.Parameter;

namespace EFFC.Frame.Net.Data.DataConvert
{
    public class Json2DataSet : IDataConvert<DataSetStd>
    {
        public DataSetStd ConvertTo(object obj)
        {
            if (obj == null)
                return null;

            DataSetStd rtn = new DataSetStd();
            string jsonstr = "";
            
            if (obj is string)
            {
                jsonstr = ComFunc.nvl(obj);
            }
            else
            {
                throw new Exception("Json2DataSet无法转化" + obj.GetType().FullName + "类型数据!");
            }
            JsonReader reader = new JsonTextReader(new StringReader(jsonstr));
            string tablename = "";
            int index = 0;
            bool isStartArray = false;
            string currentColumn = "";
            DataTableStd currentdts = null;
            while (reader.Read())
            {
                Console.WriteLine(reader.TokenType + "\t\t" + reader.ValueType + "\t\t" + reader.Value);
                if (!isStartArray)
                {
                    if (reader.TokenType == JsonToken.PropertyName)
                    {
                        tablename = ComFunc.nvl(reader.Value);
                    }
                    if (reader.TokenType == JsonToken.StartArray)
                    {
                        isStartArray = true;
                        currentdts = new DataTableStd();
                    }
                }
                else
                {
                    //列表数据结束
                    if (reader.TokenType == JsonToken.EndArray)
                    {
                        isStartArray = false;
                        rtn.Tables.Add(currentdts);
                    }
                    //一行资料结束
                    if (reader.TokenType == JsonToken.EndObject)
                    {
                        currentdts.AddNewRow();
                        index++;
                    }
                    
                    //抓取资料栏位
                    if (reader.TokenType == JsonToken.PropertyName)
                    {
                        if (index == 0)
                        {
                            currentdts.AddColumn(ColumnP.CreatInstanse(ComFunc.nvl(reader.Value)));
                        }
                        currentColumn = ComFunc.nvl(reader.Value);
                    }
                    //抓取资料数据
                    if (reader.TokenType == JsonToken.String
                        || reader.TokenType == JsonToken.Boolean
                        || reader.TokenType == JsonToken.Bytes
                        || reader.TokenType == JsonToken.Date
                        || reader.TokenType == JsonToken.Float
                        || reader.TokenType == JsonToken.Integer)
                    {
                        currentdts.SetNewRowValue(reader.Value, currentColumn);
                    }

                    
                }
            }


            return rtn;
        }
    }
}
