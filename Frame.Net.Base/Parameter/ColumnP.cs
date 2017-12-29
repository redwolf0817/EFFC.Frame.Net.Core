using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EFFC.Frame.Net.Base.Parameter;

namespace EFFC.Frame.Net.Base.Parameter
{
    /// <summary>
    /// column的参数结构定义
    /// </summary>
    public class ColumnP : ParameterStd
    {
        /// <summary>
        /// String的Type描述
        /// </summary>
        public static string String_Type = "System.String";
        /// <summary>
        /// Int的Type描述
        /// </summary>
        public static string Int32_Type = "System.Int32";
        /// <summary>
        /// Int64的Type描述
        /// </summary>
        public static string Int64_Type = "System.Int64";
        /// <summary>
        /// Doube的Type描述
        /// </summary>
        public static string Double_Type = "System.Double";
        /// <summary>
        /// Float的Type描述
        /// </summary>
        public static string Float_Type = "System.Float";
        /// <summary>
        /// DateTime的Type描述
        /// </summary>
        public static string DateTime_Type = "System.DateTime";

        /// <summary>
        /// 創建一個實例，栏位类型為默认的String类型，长度為无限制，允許為空
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public static ColumnP CreatInstanse(string column)
        {
            ColumnP cp = new ColumnP();
            cp.ColumnName = column;

            return cp;
        }
        /// <summary>
        /// 創建一個實例，長度為無限制，允許為空
        /// </summary>
        /// <param name="column"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ColumnP CreatInstanse(string column, string type)
        {
            ColumnP cp = new ColumnP();
            cp.ColumnName = column;
            cp.DataType = type;

            return cp;
        }
        /// <summary>
        /// 創建一個實例，允許為空
        /// </summary>
        /// <param name="column"></param>
        /// <param name="type"></param>
        /// <param name="MaxLength"></param>
        /// <returns></returns>
        public static ColumnP CreatInstanse(string column, string type, int MaxLength)
        {
            ColumnP cp = new ColumnP();
            cp.ColumnName = column;
            cp.DataType = type;
            cp.Length = MaxLength;

            return cp;
        }

        public ColumnP()
        {
            this.SetValue("DataType", String_Type);
            this.SetValue("Length", -1);
            this.SetValue("IsAllowNull", true);
            this.SetValue("IsPK", false);
            this.SetValue("IsAutoIncrement", false);
            this.SetValue("AutoIncrementSeed", 0);
            if (!IsAutoIncrement)
            {
                this.SetValue("AutoIncrementStep", -1);
            }
            else
            {
                this.SetValue("AutoIncrementStep", 0);
            }
        }

        public string ColumnName
        {
            get
            {
                return GetValue<string>("ColumnName");
            }
            set
            {
                SetValue("ColumnName", value);
            }
        }
        public string DataType
        {
            get
            {
                return GetValue<string>("DataType");
            }
            set
            {
                SetValue("DataType", value);
            }
        }
        public int Length
        {
            get
            {
                return GetValue<int>("Length");
            }
            set
            {
                SetValue("Length", value);
            }
        }
        public bool IsAllowNull
        {
            get
            {
                return GetValue<bool>("IsAllowNull");
            }
            set
            {
                if (!this.IsPK)
                {
                    SetValue("IsAllowNull", value);
                }
                else
                {
                    SetValue("IsAllowNull", false);
                }
            }
        }
        public bool IsPK
        {
            get
            {
                return GetValue<bool>("IsPK");
            }
            set
            {
                SetValue("IsPK", value);
                if (value)
                {
                    IsAllowNull = false;
                }
            }
        }
        public bool IsAutoIncrement
        {
            get
            {
                return GetValue<bool>("IsAutoIncrement");
            }
            set
            {
                SetValue("IsAutoIncrement", value);
            }
        }
        public int AutoIncrementSeed
        {
            get
            {
                return GetValue<int>("AutoIncrementSeed");
            }
            set
            {
                SetValue("AutoIncrementSeed", value);
            }
        }
        public int AutoIncrementStep
        {
            get
            {
                return GetValue<int>("AutoIncrementStep");
            }
            set
            {
                SetValue("AutoIncrementStep", value);
            }
        }
    }
}
