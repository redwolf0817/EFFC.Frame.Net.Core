using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace EFFC.Frame.Net.Base.Data.Base
{
    public class StringStd : StandardData<String>
    {
        /// <summary>
        /// constructor
        /// </summary>
        public StringStd()
        {
            this.Value = "";
        }
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="value"></param>
        public StringStd(string value)
        {
            this.Value = value;
        }


        /// <summary>
        /// 转换成StringStd类型
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static StringStd ParseStd(string o)
        {
            StringStd rtn = new StringStd();
            rtn.Value = o;
            return rtn;
        }
        /// <summary>
        /// 將StringStd[]转化成String[]
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string[] ToStringArray(StringStd[] str)
        {
            string[] rtn = new string[str.Length];
            for (int i = 0; i < str.Length; i++)
            {
                rtn[i] = str[i].Value;
            }

            return rtn;
        }
        /// <summary>
        /// 將StringStd[]转化成String[]
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static StringStd[] ToStringStdArray(string[] str)
        {
            StringStd[] rtn = new StringStd[str.Length];
            for (int i = 0; i < str.Length; i++)
            {
                rtn[i] = new StringStd(str[i]);
            }

            return rtn;
        }
        /// <summary>
        /// 隐含转换
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static implicit operator StringStd(string s)
        {
            return StringStd.ParseStd(s);
        }
        /// <summary>
        /// 隐含转换
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static implicit operator string(StringStd s)
        {
            return s.Value;
        }
        /// <summary>
        /// 相等判定
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        /// <summary>
        /// 判斷兩個指定的 System.StringStd 物件是否具有不同的值。
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(StringStd a, StringStd b)
        {
            object o = a;
            object oo = b;
            if (o == null || oo == null)
            {
                return o != oo;
            }
            else
            {
                return a.Value != b.Value;
            }
        }
        /// <summary>
        /// 判斷兩個指定的 System.StringStd 物件是否具有相同的值。
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(StringStd a, StringStd b)
        {
            object o = a;
            object oo = b;
            if (o == null || oo == null)
            {
                return o == oo;
            }
            else
            {
                return a.Value == b.Value;
            }
        }

        /// <summary>
        /// 取得這個執行個體中的字元數。
        /// </summary>
        public int Length { get { return Value.Length; } }

        /// <summary>
        /// 取得這個執行個體中指定字元位置的字元。
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public char this[int index] { get { return this.Value[index]; } }

        /// <summary>
        /// 傳回對 System.StringStd 這個執行個體的參考。
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new StringStd(this.Value);
        }
        /// <summary>
        /// 比較兩個指定的 System.StringStd 物件。
        /// </summary>
        /// <param name="strA"></param>
        /// <param name="strB"></param>
        /// <returns></returns>
        public static int Compare(StringStd strA, StringStd strB)
        {
            return string.Compare(strA.Value, strB.Value);
        }
        /// <summary>
        /// 比較兩個指定的 System.StringStd 物件，忽略或承認它們的大小寫。
        /// </summary>
        /// <param name="strA"></param>
        /// <param name="strB"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public static int Compare(StringStd strA, StringStd strB, bool ignoreCase)
        {
            return string.Compare(strA.Value, strB.Value, ignoreCase);
        }
        /// <summary>
        /// 比較兩個指定的 System.StringStd 物件。參數可指定比較作業是否使用目前的文化特性或不因文化特性而異、接受或忽略大小寫，以及使用文字或序數的排序規則。
        /// </summary>
        /// <param name="strA"></param>
        /// <param name="strB"></param>
        /// <param name="comparisonType"></param>
        /// <returns></returns>
        public static int Compare(StringStd strA, StringStd strB, StringComparison comparisonType)
        {
            return string.Compare(strA.Value, strB.Value, comparisonType);
        }
        /// <summary>
        /// 比較兩個指定的 System.StringStd 物件，忽略或承認它們的大小寫，並使用與文化特性相關的資訊影響比較。
        /// </summary>
        /// <param name="strA"></param>
        /// <param name="strB"></param>
        /// <param name="ignoreCase"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static int Compare(StringStd strA, StringStd strB, bool ignoreCase, CultureInfo culture)
        {
            return string.Compare(strA.Value, strB.Value, ignoreCase, culture);
        }
        /// <summary>
        /// 比較兩個指定的 System.StringStd 物件的子字串。
        /// </summary>
        /// <param name="strA"></param>
        /// <param name="indexA"></param>
        /// <param name="strB"></param>
        /// <param name="indexB"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static int Compare(StringStd strA, int indexA, StringStd strB, int indexB, int length)
        {
            return string.Compare(strA.Value, indexA, strB.Value, indexB, length);
        }
        /// <summary>
        /// 比較兩個指定的 System.StringStd 物件子字串，忽略或承認它們的大小寫。
        /// </summary>
        /// <param name="strA"></param>
        /// <param name="indexA"></param>
        /// <param name="strB"></param>
        /// <param name="indexB"></param>
        /// <param name="length"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public static int Compare(StringStd strA, int indexA, StringStd strB, int indexB, int length, bool ignoreCase)
        {
            return string.Compare(strA.Value, indexA, strB.Value, indexB, length, ignoreCase);
        }
        /// <summary>
        /// 比較兩個指定的 System.StringStd 物件的子字串。
        /// </summary>
        /// <param name="strA"></param>
        /// <param name="indexA"></param>
        /// <param name="strB"></param>
        /// <param name="indexB"></param>
        /// <param name="length"></param>
        /// <param name="comparisonType"></param>
        /// <returns></returns>
        public static int Compare(StringStd strA, int indexA, StringStd strB, int indexB, int length, StringComparison comparisonType)
        {
            return string.Compare(strA.Value, indexA, strB.Value, indexB, length, comparisonType);
        }
        /// <summary>
        /// 比較兩個指定的 System.StringStd 物件的子字串，忽略或承認它們的大小寫，並使用與文化特性相關的資訊影響比較。
        /// </summary>
        /// <param name="strA"></param>
        /// <param name="indexA"></param>
        /// <param name="strB"></param>
        /// <param name="indexB"></param>
        /// <param name="length"></param>
        /// <param name="ignoreCase"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static int Compare(StringStd strA, int indexA, StringStd strB, int indexB, int length, bool ignoreCase, CultureInfo culture)
        {
            return string.Compare(strA.Value, indexA, strB.Value, indexB, length, ignoreCase, culture);
        }
        /// <summary>
        /// 藉由評估每個字串中對應的 System.Char 物件之數字值，比較兩個指定的 System.StringStd 物件。
        /// </summary>
        /// <param name="strA"></param>
        /// <param name="strB"></param>
        /// <returns></returns>
        public static int CompareOrdinal(StringStd strA, StringStd strB)
        {
            return string.CompareOrdinal(strA.Value, strB.Value);
        }
        /// <summary>
        /// 藉由評估每個子字串中對應的 System.Char 物件之數字值，比較兩個指定的 System.StringStd 物件之子字串。
        /// </summary>
        /// <param name="strA"></param>
        /// <param name="indexA"></param>
        /// <param name="strB"></param>
        /// <param name="indexB"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static int CompareOrdinal(StringStd strA, int indexA, StringStd strB, int indexB, int length)
        {
            return string.CompareOrdinal(strA.Value, indexA, strB.Value, indexB, length);
        }
        /// <summary>
        /// 比較這個執行個體與指定的 System.Object。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int CompareTo(object value)
        {
            return this.Value.CompareTo(value);
        }
        /// <summary>
        /// 比較這個執行個體與指定的 System.StringStd 物件。
        /// </summary>
        /// <param name="strB"></param>
        /// <returns></returns>
        public int CompareTo(StringStd strB)
        {
            return this.CompareTo(strB.Value);
        }
        /// <summary>
        /// 建立指定物件的 System.StringStd 表示。
        /// </summary>
        /// <param name="arg0"></param>
        /// <returns></returns>
        public static StringStd Concat(object arg0)
        {
            return new StringStd(string.Concat(arg0));
        }
        /// <summary>
        /// 串連指定 System.Object 陣列中元素的 System.StringStd 表示。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static StringStd Concat(params object[] args)
        {
            return new StringStd(string.Concat(args));
        }
        /// <summary>
        /// 串連指定 System.StringStd 陣列中的元素。
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static StringStd Concat(params StringStd[] values)
        {
            string[] strs = new string[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                strs[i] = values[i].Value;
            }

            return new StringStd(string.Concat(strs));
        }
        /// <summary>
        /// 串連兩個指定物件的 System.StringStd 表示。
        /// </summary>
        /// <param name="arg0"></param>
        /// <param name="arg1"></param>
        /// <returns></returns>
        public static StringStd Concat(object arg0, object arg1)
        {
            return new StringStd(string.Concat(arg0, arg1));
        }
        /// <summary>
        /// 串連 System.StringStd 的兩個指定執行個體。
        /// </summary>
        /// <param name="str0"></param>
        /// <param name="str1"></param>
        /// <returns></returns>
        public static StringStd Concat(StringStd str0, StringStd str1)
        {
            return new StringStd(string.Concat(str0.Value, str1.Value));
        }
        /// <summary>
        /// 串連三個指定物件的 System.StringStd 表示。
        /// </summary>
        /// <param name="arg0"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <returns></returns>
        public static StringStd Concat(object arg0, object arg1, object arg2)
        {
            return new StringStd(string.Concat(arg0, arg1, arg2));
        }
        /// <summary>
        /// 串連 System.StringStd 的三個指定執行個體。
        /// </summary>
        /// <param name="str0"></param>
        /// <param name="str1"></param>
        /// <param name="str2"></param>
        /// <returns></returns>
        public static StringStd Concat(StringStd str0, StringStd str1, StringStd str2)
        {
            return new StringStd(string.Concat(str0.Value, str1.Value, str2.Value));
        }
        /// <summary>
        /// 串連四個指定的物件之 System.StringStd 表示和選擇性變數長度參數清單中所指定的任何物件。
        /// </summary>
        /// <param name="arg0"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <returns></returns>
        public static StringStd Concat(object arg0, object arg1, object arg2, object arg3)
        {
            return new StringStd(string.Concat(arg0, arg1, arg2, arg3));
        }
        /// <summary>
        /// 串連 System.StringStd 的四個指定執行個體。
        /// </summary>
        /// <param name="str0"></param>
        /// <param name="str1"></param>
        /// <param name="str2"></param>
        /// <param name="str3"></param>
        /// <returns></returns>
        public static StringStd Concat(StringStd str0, StringStd str1, StringStd str2, StringStd str3)
        {
            return new StringStd(string.Concat(str0.Value, str1.Value, str2.Value, str3.Value));
        }
        /// <summary>
        /// 傳回值，指出指定的 System.StringStd 物件是否會出現在這個字串內。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Contains(StringStd value)
        {
            return this.Value.Contains(value.Value);
        }
        /// <summary>
        /// 使用與指定的 System.StringStd 相同的值，建立 System.StringStd 的新執行個體。
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static StringStd Copy(StringStd str)
        {
            return new StringStd(string.Copy(str.Value));
        }
        /// <summary>
        /// 將字元的指定數目從這個執行個體的指定位置，複製到 Unicode 字元陣列的指定位置。
        /// </summary>
        /// <param name="sourceIndex"></param>
        /// <param name="destination"></param>
        /// <param name="destinationIndex"></param>
        /// <param name="count"></param>
        public void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count)
        {
            this.Value.CopyTo(sourceIndex, destination, destinationIndex, count);
        }
        /// <summary>
        /// 判斷這個執行個體的結尾是否符合指定的字串。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool EndsWith(StringStd value)
        {
            return this.Value.EndsWith(value.Value);
        }
        /// <summary>
        /// 判斷當使用指定之比較選項進行比較時，此字串的結尾是否符合指定之字串。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparisonType"></param>
        /// <returns></returns>
        public bool EndsWith(StringStd value, StringComparison comparisonType)
        {
            return this.Value.EndsWith(value.Value, comparisonType);
        }
        /// <summary>
        /// 判斷當使用指定之文化特性進行比較時，此字串的結尾是否符合指定之字串。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ignoreCase"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public bool EndsWith(StringStd value, bool ignoreCase, CultureInfo culture)
        {
            return this.Value.EndsWith(value.Value, ignoreCase, culture);
        }
        /// <summary>
        /// 判斷這個執行個體和另一個指定的 System.StringStd 物件是否具有相同的值。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Equals(StringStd value)
        {
            return this.Value.Equals(value.Value);
        }
        /// <summary>
        /// 判斷兩個指定的 System.StringStd 物件是否具有相同的值。
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool Equals(StringStd a, StringStd b)
        {
            return string.Equals(a.Value, b.Value);
        }
        /// <summary>
        ///  判斷這個字串和指定的 System.StringStd 物件是否具有相同的值。參數可指定用於比較的文化特性、大小寫及排序規則。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparisonType"></param>
        /// <returns></returns>
        public bool Equals(StringStd value, StringComparison comparisonType)
        {
            return this.Value.Equals(value.Value, comparisonType);
        }
        /// <summary>
        /// 判斷兩個指定的 System.StringStd 物件是否具有相同的值。參數可指定用於比較的文化特性、大小寫及排序規則。
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="comparisonType"></param>
        /// <returns></returns>
        public static bool Equals(StringStd a, StringStd b, StringComparison comparisonType)
        {
            return string.Equals(a.Value, b.Value, comparisonType);
        }
        /// <summary>
        /// 以與指定的 System.Object 執行個體值相等的文字，取代指定的 System.StringStd 中的格式項目。
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arg0"></param>
        /// <returns></returns>
        public static StringStd Format(StringStd format, object arg0)
        {
            return new StringStd(string.Format(format.Value, arg0));
        }
        /// <summary>
        /// 以與指定陣列中對應的 System.Object 執行個體值相等的文字，取代指定的 System.StringStd 中的格式項目。
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static StringStd Format(StringStd format, params object[] args)
        {
            return new StringStd(string.Format(format.Value, args));
        }
        /// <summary>
        /// 以與指定陣列中對應的 System.Object 執行個體值相等的文字，取代指定的 System.StringStd 中的格式項目。指定的參數提供特定文化特性的格式資訊。
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static StringStd Format(IFormatProvider provider, StringStd format, params object[] args)
        {
            return new StringStd(string.Format(provider, format.Value, args));
        }
        /// <summary>
        /// 以與兩個指定的 System.Object 執行個體值相等的文字，取代指定的 System.StringStd 中的格式項目。
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arg0"></param>
        /// <param name="arg1"></param>
        /// <returns></returns>
        public static StringStd Format(StringStd format, object arg0, object arg1)
        {
            return new StringStd(string.Format(format.Value, arg0, arg1));
        }
        /// <summary>
        /// 以與三個指定的 System.Object 執行個體值相等的文字，取代指定的 System.StringStd 中的格式項目。
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arg0"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <returns></returns>
        public static StringStd Format(StringStd format, object arg0, object arg1, object arg2)
        {
            return new StringStd(string.Format(format.Value, arg0, arg1, arg2));
        }
        /// <summary>
        /// 擷取可以逐一查看這個字串中個別字元的物件。
        /// </summary>
        /// <returns></returns>
        public CharEnumerator GetEnumerator()
        {
            return this.Value.GetEnumerator();
        }
        /// <summary>
        /// 傳回這個字串的雜湊程式碼。
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }
        /// <summary>
        /// 傳回類別 System.StringStd 的 System.TypeCode。
        /// </summary>
        /// <returns></returns>
        public TypeCode GetTypeCode()
        {
            return this.Value.GetTypeCode();
        }
        /// <summary>
        /// 報告這個字串中指定之 Unicode 字元的第一個符合項目的索引。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int IndexOf(char value)
        {
            return this.Value.IndexOf(value);
        }
        /// <summary>
        /// 報告這個執行個體中指定 System.StringStd 第一個符合項目的索引。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int IndexOf(StringStd value)
        {
            return this.Value.IndexOf(value.Value);
        }
        /// <summary>
        /// 報告這個字串中指定之 Unicode 字元的第一個符合項目的索引。搜尋從指定的字元位置開始。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public int IndexOf(char value, int startIndex)
        {
            return this.Value.IndexOf(value, startIndex);
        }
        /// <summary>
        /// 報告這個執行個體中指定 System.StringStd 第一個符合項目的索引。搜尋從指定的字元位置開始。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public int IndexOf(StringStd value, int startIndex)
        {
            return this.Value.IndexOf(value.Value, startIndex);
        }
        /// <summary>
        /// 報告目前 System.StringStd 物件中所指定字串之第一個出現的索引。參數會指定要用於指定字串的搜尋類型。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparisonType"></param>
        /// <returns></returns>
        public int IndexOf(StringStd value, StringComparison comparisonType)
        {
            return this.Value.IndexOf(value.Value, comparisonType);
        }
        /// <summary>
        /// 報告這個執行個體中指定字元第一個符合項目的索引。搜尋從指定的字元位置開始，並檢視指定數目的字元位置。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public int IndexOf(char value, int startIndex, int count)
        {
            return this.Value.IndexOf(value, startIndex, count);
        }
        /// <summary>
        /// 報告這個執行個體中指定 System.StringStd 第一個符合項目的索引。搜尋從指定的字元位置開始，並檢視指定數目的字元位置。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public int IndexOf(StringStd value, int startIndex, int count)
        {
            return this.Value.IndexOf(value.Value, startIndex, count);
        }
        /// <summary>
        /// 報告目前 System.StringStd 物件中所指定字串之第一個出現的索引。參數會指定目前字串中的開始搜尋位置和要用於指定字串的搜尋類型。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="startIndex"></param>
        /// <param name="comparisonType"></param>
        /// <returns></returns>
        public int IndexOf(StringStd value, int startIndex, StringComparison comparisonType)
        {
            return this.Value.IndexOf(value.Value, startIndex, comparisonType);
        }
        /// <summary>
        /// 報告目前 System.StringStd 物件中所指定字串之第一個出現的索引。參數會指定目前字串中的開始搜尋位置、目前字串中要搜尋的字元數目，以及要用於指定字串的搜尋類型。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <param name="comparisonType"></param>
        /// <returns></returns>
        public int IndexOf(StringStd value, int startIndex, int count, StringComparison comparisonType)
        {
            return this.Value.IndexOf(value.Value, startIndex, count, comparisonType);
        }
        /// <summary>
        /// 報告指定 Unicode 字元陣列中的任何字元於這個執行個體中第一個符合項目的索引。
        /// </summary>
        /// <param name="anyOf"></param>
        /// <returns></returns>
        public int IndexOfAny(char[] anyOf)
        {
            return this.Value.IndexOfAny(anyOf);
        }
        /// <summary>
        /// 報告指定 Unicode 字元陣列中的任何字元於這個執行個體中第一個符合項目的索引。搜尋從指定的字元位置開始。
        /// </summary>
        /// <param name="anyOf"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public int IndexOfAny(char[] anyOf, int startIndex)
        {
            return this.Value.IndexOfAny(anyOf, startIndex);
        }
        /// <summary>
        /// 報告指定 Unicode 字元陣列中的任何字元於這個執行個體中第一個符合項目的索引。搜尋從指定的字元位置開始，並檢視指定數目的字元位置。
        /// </summary>
        /// <param name="anyOf"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public int IndexOfAny(char[] anyOf, int startIndex, int count)
        {
            return this.Value.IndexOfAny(anyOf, startIndex, count);
        }
        /// <summary>
        /// 在這個執行個體的指定索引位置，插入 System.StringStd 的指定執行個體。
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public StringStd Insert(int startIndex, StringStd value)
        {
            return new StringStd(this.Value.Insert(startIndex, value.Value));
        }
        /// <summary>
        /// 指出指定的 System.StringStd 物件是否為 null 或 System.StringStd.Empty 字串。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(StringStd value)
        {
            return string.IsNullOrEmpty(value.Value);
        }
        /// <summary>
        /// 將指定 System.StringStd 陣列每個元素之間的指定分隔符號 System.StringStd 串連，產生單一的串連字串。
        /// </summary>
        /// <param name="separator"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static StringStd Join(StringStd separator, StringStd[] value)
        {

            return new StringStd(string.Join(separator.Value, StringStd.ToStringArray(value)));
        }
        /// <summary>
        /// 將指定 System.StringStd 陣列每個元素之間的指定分隔符號 System.StringStd 串連，產生單一的串連字串。參數指定要使用的第一個陣列元素和元素數目。
        /// </summary>
        /// <param name="separator"></param>
        /// <param name="value"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static StringStd Join(StringStd separator, StringStd[] value, int startIndex, int count)
        {
            return new StringStd(string.Join(separator.Value, StringStd.ToStringArray(value), startIndex, count));
        }
        /// <summary>
        /// 報告這個執行個體中指定 Unicode 字元最後項目的索引位置。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int LastIndexOf(char value)
        {
            return this.Value.LastIndexOf(value);
        }
        /// <summary>
        /// 報告這個執行個體中指定 System.StringStd 最後項目的索引位置。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int LastIndexOf(StringStd value)
        {
            return this.Value.LastIndexOf(value.Value);
        }
        /// <summary>
        /// 報告這個執行個體中指定 Unicode 字元最後項目的索引位置。搜尋從指定的字元位置開始。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public int LastIndexOf(char value, int startIndex)
        {
            return this.Value.LastIndexOf(value, startIndex);
        }
        /// <summary>
        /// 報告這個執行個體中指定 System.StringStd 最後項目的索引位置。搜尋從指定的字元位置開始。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public int LastIndexOf(StringStd value, int startIndex)
        {
            return this.Value.LastIndexOf(value.Value, startIndex);
        }
        /// <summary>
        /// 報告目前 System.StringStd 物件中所指定字串之最後一個項目的索引。參數會指定要用於指定字串的搜尋類型。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparisonType"></param>
        /// <returns></returns>
        public int LastIndexOf(StringStd value, StringComparison comparisonType)
        {
            return this.Value.LastIndexOf(value.Value, comparisonType);
        }
        /// <summary>
        /// 報告這個執行個體中子字串的指定 Unicode 字元最後項目的索引位置。搜尋從指定的字元位置開始，並檢視指定數目的字元位置。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public int LastIndexOf(char value, int startIndex, int count)
        {
            return this.Value.LastIndexOf(value, startIndex, count);
        }
        /// <summary>
        /// 報告這個執行個體中指定 System.StringStd 最後項目的索引位置。搜尋從指定的字元位置開始，並檢視指定數目的字元位置。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public int LastIndexOf(StringStd value, int startIndex, int count)
        {
            return this.Value.LastIndexOf(value.Value, startIndex, count);
        }
        /// <summary>
        /// 報告目前 System.StringStd 物件中所指定字串之最後一個項目的索引。參數會指定目前字串中的開始搜尋位置和要用於指定字串的搜尋類型。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="startIndex"></param>
        /// <param name="comparisonType"></param>
        /// <returns></returns>
        public int LastIndexOf(StringStd value, int startIndex, StringComparison comparisonType)
        {
            return this.Value.LastIndexOf(value.Value, startIndex, comparisonType);
        }
        /// <summary>
        /// 報告這個執行個體中指定 System.StringStd 物件的最後一個項目的索引位置。參數會指定目前字串中的開始搜尋位置、目前字串中要搜尋的字元數目，以及要用於指定字串的搜尋類型。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <param name="comparisonType"></param>
        /// <returns></returns>
        public int LastIndexOf(StringStd value, int startIndex, int count, StringComparison comparisonType)
        {
            return this.Value.LastIndexOf(value.Value, startIndex, count, comparisonType);
        }
        /// <summary>
        /// 報告 Unicode 陣列中的一個或多個指定字元在這個執行個體中最後項目的索引位置。
        /// </summary>
        /// <param name="anyOf"></param>
        /// <returns></returns>
        public int LastIndexOfAny(char[] anyOf)
        {
            return this.Value.LastIndexOfAny(anyOf);
        }
        /// <summary>
        /// 報告 Unicode 陣列中的一個或多個指定字元在這個執行個體中最後項目的索引位置。搜尋從指定的字元位置開始。
        /// </summary>
        /// <param name="anyOf"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public int LastIndexOfAny(char[] anyOf, int startIndex)
        {
            return this.Value.LastIndexOfAny(anyOf, startIndex);
        }
        /// <summary>
        /// 報告 Unicode 陣列中的一個或多個指定字元在這個執行個體中最後項目的索引位置。搜尋從指定的字元位置開始，並檢視指定數目的字元位置。
        /// </summary>
        /// <param name="anyOf"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public int LastIndexOfAny(char[] anyOf, int startIndex, int count)
        {
            return this.Value.LastIndexOfAny(anyOf, startIndex, count);
        }
        /// <summary>
        /// 將這個執行個體中的字元靠右對齊，以空格在左側填補至指定的總長度。
        /// </summary>
        /// <param name="totalWidth"></param>
        /// <returns></returns>
        public StringStd PadLeft(int totalWidth)
        {
            return new StringStd(this.Value.PadLeft(totalWidth));
        }
        /// <summary>
        /// 將這個執行個體中的字元靠右對齊，以指定的 Unicode 字元在左側填補至指定的總長度。
        /// </summary>
        /// <param name="totalWidth"></param>
        /// <param name="paddingChar"></param>
        /// <returns></returns>
        public StringStd PadLeft(int totalWidth, char paddingChar)
        {
            return new StringStd(this.Value.PadLeft(totalWidth, paddingChar));
        }
        /// <summary>
        /// 將這個字串中的字元靠左對齊，以空格在右側填補至指定的總長度。
        /// </summary>
        /// <param name="totalWidth"></param>
        /// <returns></returns>
        public StringStd PadRight(int totalWidth)
        {
            return new StringStd(this.Value.PadRight(totalWidth));
        }
        /// <summary>
        /// 將這個字串中的字元靠左對齊，以指定的 Unicode 字元在右側填補至指定的總長度。
        /// </summary>
        /// <param name="totalWidth"></param>
        /// <param name="paddingChar"></param>
        /// <returns></returns>
        public StringStd PadRight(int totalWidth, char paddingChar)
        {
            return new StringStd(this.Value.PadRight(totalWidth, paddingChar));
        }
        /// <summary>
        /// 從這個字串中的指定之位置開始刪除所有字元，一直到最後一個位置為止。
        /// </summary>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public StringStd Remove(int startIndex)
        {
            return new StringStd(this.Value.Remove(startIndex));
        }
        /// <summary>
        /// 將指定字元數從起始於指定位置的這個執行個體中刪除。
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public StringStd Remove(int startIndex, int count)
        {
            return new StringStd(this.Value.Remove(startIndex, count));
        }
        /// <summary>
        /// 以另一個指定的 Unicode 字元，取代這個執行個體中指定的 Unicode 字元的所有項目。
        /// </summary>
        /// <param name="oldChar"></param>
        /// <param name="newChar"></param>
        /// <returns></returns>
        public StringStd Replace(char oldChar, char newChar)
        {
            return new StringStd(this.Value.Replace(oldChar, newChar));
        }
        /// <summary>
        /// 以另一個指定的 System.StringStd，取代這個執行個體中指定的 System.StringStd 的所有項目。
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public StringStd Replace(StringStd oldValue, StringStd newValue)
        {
            return new StringStd(this.Value.Replace(oldValue.Value, newValue.Value));
        }
        /// <summary>
        /// 傳回 System.StringStd 在這個執行個體中包含子字串的陣列，它是由指定的 System.Char 陣列項目分隔的。
        /// </summary>
        /// <param name="separator"></param>
        /// <returns></returns>
        public StringStd[] Split(params char[] separator)
        {
            return StringStd.ToStringStdArray(this.Value.Split(separator));
        }
        /// <summary>
        /// 傳回 System.StringStd 在這個執行個體中包含子字串的陣列，它是由指定的 System.Char 陣列項目分隔的。參數可指定要傳回的子字串數目的最大值。
        /// </summary>
        /// <param name="separator">Unicode 字元陣列，分隔這個執行個體中的子字串；空陣列，不包含分隔符號；或 null。</param>
        /// <param name="count">要傳回的子字串之最大數目。</param>
        /// <returns>陣列，其元素包含了這個執行個體中由 separator 內的一或多個字元所分隔的子字串。如需詳細資訊，請參閱＜備註＞一節。</returns>
        public StringStd[] Split(char[] separator, int count)
        {
            return StringStd.ToStringStdArray(this.Value.Split(separator, count));
        }
        /// <summary>
        /// 傳回 System.StringStd 陣列，其中包含這個字串中的子字串，都由指定的 System.Char 陣列元素所分隔。參數指定是否傳回空白的陣列元素。
        /// </summary>
        /// <param name="separator">Unicode 字元陣列 (可分隔這個字串中的子字串)、不含任何分隔符號的空白陣列，或 null。</param>
        /// <param name="options"> 指定 System.StringSplitOptions.RemoveEmptyEntries，省略傳回陣列的空白陣列元素；或 System.StringSplitOptions.None，在傳回陣列中包含空白陣列元素。</param>
        /// <returns>陣列，其元素包含了這個字串中由 separator 內的一或多個字元所分隔的子字串。如需詳細資訊，請參閱＜備註＞一節。</returns>
        public StringStd[] Split(char[] separator, StringSplitOptions options)
        {
            return StringStd.ToStringStdArray(this.Value.Split(separator, options));
        }
        /// <summary>
        /// 傳回 System.StringStd 陣列，其中包含這個字串中的子字串，都由指定的 System.StringStd 陣列元素所分隔。參數指定是否傳回空白的陣列元素。
        /// </summary>
        /// <param name="separator">字串陣列 (可分隔這個字串中的子字串)、不含任何分隔符號的空白陣列，或 null。</param>
        /// <param name="options">指定 System.StringSplitOptions.RemoveEmptyEntries，省略傳回陣列的空白陣列元素；或 System.StringSplitOptions.None，在傳回陣列中包含空白陣列元素。</param>
        /// <returns>陣列，其元素包含了這個字串中由 separator 內的一或多個字串所分隔的子字串。如需詳細資訊，請參閱＜備註＞一節。</returns>
        public StringStd[] Split(string[] separator, StringSplitOptions options)
        {
            return StringStd.ToStringStdArray(this.Value.Split(separator, options));
        }
        /// <summary>
        /// 傳回 System.StringStd 陣列，其中包含這個字串中的子字串，都由指定的 System.StringStd 陣列元素所分隔。參數指定是否傳回空白的陣列元素。
        /// </summary>
        /// <param name="separator"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public StringStd[] Split(StringStd[] separator, StringSplitOptions options)
        {
            return StringStd.ToStringStdArray(this.Value.Split(StringStd.ToStringArray(separator), options));
        }
        /// <summary>
        /// 傳回 System.StringStd 陣列，其中包含這個字串中的子字串，都由指定的 System.StringStd 陣列元素所分隔。傳回空白的陣列元素。
        /// </summary>
        /// <param name="separator"></param>
        /// <returns></returns>
        public StringStd[] Split(string separator)
        {
            string[] s = new string[1];
            s[0] = separator;
            return this.Split(s, StringSplitOptions.None);
        }
        /// <summary>
        /// 傳回 System.StringStd 陣列，其中包含這個字串中的子字串，都由指定的 System.StringStd 陣列元素所分隔。傳回空白的陣列元素。
        /// </summary>
        /// <param name="separator"></param>
        /// <returns></returns>
        public StringStd[] Split(StringStd separator)
        {
            return this.Split(separator.Value);
        }
        /// <summary>
        /// 傳回 System.StringStd 陣列，其中包含這個字串中的子字串，都由指定的 System.Char 陣列元素所分隔。參數指定傳回的子字串最大數目，以及是否傳回空的陣列元素。
        /// </summary>
        /// <param name="separator">Unicode 字元陣列 (可分隔這個字串中的子字串)、不含任何分隔符號的空白陣列，或 null。</param>
        /// <param name="count">要傳回的子字串之最大數目。</param>
        /// <param name="options">指定 System.StringSplitOptions.RemoveEmptyEntries，省略傳回陣列的空白陣列元素；或 System.StringSplitOptions.None，在傳回陣列中包含空白陣列元素。</param>
        /// <returns> 陣列，其元素包含了這個字串中由 separator 內的一或多個字元所分隔的子字串。如需詳細資訊，請參閱＜備註＞一節。</returns>
        public StringStd[] Split(char[] separator, int count, StringSplitOptions options)
        {
            return StringStd.ToStringStdArray(this.Value.Split(separator, count, options));
        }
        /// <summary>
        /// 傳回 System.StringStd 陣列，其中包含這個字串中的子字串，都由指定的 System.StringStd 陣列元素所分隔。參數指定傳回的子字串最大數目，以及是否傳回空的陣列元素。
        /// </summary>
        /// <param name="separator">字串陣列 (可分隔這個字串中的子字串)、不含任何分隔符號的空白陣列，或 null。</param>
        /// <param name="count">要傳回的子字串之最大數目。</param>
        /// <param name="options">指定 System.StringSplitOptions.RemoveEmptyEntries，省略傳回陣列的空白陣列元素；或 System.StringSplitOptions.None，在傳回陣列中包含空白陣列元素。</param>
        /// <returns>陣列，其元素包含了這個字串中由 separator 內的一或多個字串所分隔的子字串。如需詳細資訊，請參閱＜備註＞一節。</returns>
        public StringStd[] Split(StringStd[] separator, int count, StringSplitOptions options)
        {
            return StringStd.ToStringStdArray(this.Value.Split(StringStd.ToStringArray(separator), count, options));
        }
        /// <summary>
        /// 判斷這個執行個體的開頭是否符合指定的字串。
        /// </summary>
        /// <param name="value">要比較的 System.StringStd。</param>
        /// <returns>如果 value 符合這個字串的開頭，則為 true，否則為 false。</returns>
        public bool StartsWith(StringStd value)
        {
            return this.Value.StartsWith(value.Value);
        }
        /// <summary>
        /// 判斷當使用指定之比較選項進行比較時，此字串的開頭是否符合指定之字串。
        /// </summary>
        /// <param name="value">要比較的 System.StringStd 物件。</param>
        /// <param name="comparisonType">其中一個 System.StringComparison 值，可決定要如何比較這個字串和 value。</param>
        /// <returns> 如果 value 參數符合這個字串的開頭，則為 true，否則為 false。</returns>
        public bool StartsWith(StringStd value, StringComparison comparisonType)
        {
            return this.Value.StartsWith(value.Value, comparisonType);
        }
        /// <summary>
        /// 判斷當使用指定之文化特性進行比較時，此字串的開頭是否符合指定之字串。
        /// </summary>
        /// <param name="value">要比較的 System.StringStd 物件。</param>
        /// <param name="ignoreCase">true 表示在比較這個字串與 value 時要忽略大小寫，否則為 false。</param>
        /// <param name="culture">判斷此字串和 value 如何進行比較的文化特性資訊。如果 culture 是 null，則會使用目前的文化特性。</param>
        /// <returns>如果 value 參數符合這個字串的開頭，則為 true，否則為 false。</returns>
        public bool StartsWith(StringStd value, bool ignoreCase, CultureInfo culture)
        {
            return this.Value.StartsWith(value.Value, ignoreCase, culture);
        }
        /// <summary>
        /// 從這個執行個體擷取子字串。子字串從指定的字元位置開始。
        /// </summary>
        /// <param name="startIndex">這個執行個體中的子字串起始字元位置。</param>
        /// <returns>System.StringStd 物件，其相當於此執行個體中開始於 startIndex 的子字串；如果 startIndex 等於此執行個體的長度，則為
        ///     System.StringStd.Empty。</returns>
        public StringStd Substring(int startIndex)
        {
            return new StringStd(this.Value.Substring(startIndex));
        }
        /// <summary>
        /// 從這個執行個體擷取子字串。子字串起始於指定的字元位置，並且具有指定的長度。
        /// </summary>
        /// <param name="startIndex">子字串的起始索引。</param>
        /// <param name="length">子字串中的字元數。</param>
        /// <returns>System.StringStd，其相當於此執行個體中開始於 startIndex 且長度為 length 的子字串；如果 startIndex 等於此執行個體的長度，且
        ///     length 為零，則為 System.StringStd.Empty。</returns>
        public StringStd Substring(int startIndex, int length)
        {
            return new StringStd(this.Value.Substring(startIndex, length));
        }
        /// <summary>
        /// 將這個執行個體中的字元複製到 Unicode 字元陣列中。
        /// </summary>
        /// <returns>Unicode 字元陣列，其元素是這個執行個體的個別字元。如果這個執行個體是空字串，則傳回的陣列會是空的且長度為零。</returns>
        public char[] ToCharArray()
        {
            return this.Value.ToCharArray();
        }
        /// <summary>
        /// 將這個執行個體的指定子字串字元複製到 Unicode 字元陣列。
        /// </summary>
        /// <param name="startIndex">這個執行個體中的子字串起始位置。</param>
        /// <param name="length">這個執行個體中的子字串長度。</param>
        /// <returns>Unicode 字元陣列，其元素是從 startIndex 字元位置起始的這個執行個體中的 length 字元數。</returns>
        public char[] ToCharArray(int startIndex, int length)
        {
            return this.Value.ToCharArray(startIndex, length);
        }
        /// <summary>
        /// 傳回轉換成小寫的這個 System.StringStd 複本，透過的方式是使用目前文化特性的大小寫規則。
        /// </summary>
        /// <returns>小寫的 System.StringStd。</returns>
        public StringStd ToLower()
        {
            return new StringStd(this.Value.ToLower());
        }
        /// <summary>
        /// 傳回轉換成小寫的這個 System.StringStd 複本，透過的方式是使用指定之文化特性的大小寫規則。
        /// </summary>
        /// <param name="culture">System.Globalization.CultureInfo 物件，提供文化特性的特定大小寫規則。</param>
        /// <returns>小寫的 System.StringStd。</returns>
        public StringStd ToLower(CultureInfo culture)
        {
            return new StringStd(this.Value.ToLower(culture));
        }
        /// <summary>
        /// 傳回轉換成小寫的這個 System.StringStd 物件之複本，透過的方式是使用不因文化特性而異的大小寫規則。
        /// </summary>
        /// <returns>小寫的 System.StringStd 物件。</returns>
        public StringStd ToLowerInvariant()
        {
            return new StringStd(this.Value.ToLowerInvariant());
        }
        /// <summary>
        /// 傳回轉換成大寫的這個 System.StringStd 複本，透過的方式是使用目前文化特性的大小寫規則。
        /// </summary>
        /// <returns>大寫的 System.StringStd。</returns>
        public StringStd ToUpper()
        {
            return new StringStd(this.Value.ToUpper());
        }
        /// <summary>
        /// 傳回轉換成大寫的這個 System.StringStd 複本，透過的方式是使用指定之文化特性的大小寫規則。
        /// </summary>
        /// <param name="culture">System.Globalization.CultureInfo 物件，提供文化特性的特定大小寫規則。</param>
        /// <returns>大寫的 System.StringStd。</returns>
        public StringStd ToUpper(CultureInfo culture)
        {
            return new StringStd(this.Value.ToUpper(culture));
        }
        /// <summary>
        /// 傳回轉換成大寫的這個 System.StringStd 物件之複本，透過的方式是使用不因文化特性而異的大小寫規則。
        /// </summary>
        /// <returns>大寫的 System.StringStd 物件。</returns>
        public StringStd ToUpperInvariant()
        {
            return new StringStd(this.Value.ToUpperInvariant());
        }
        /// <summary>
        /// 將所有泛空白字元的項目從這個執行個體的開頭和結尾移除。
        /// </summary>
        /// <returns>新的 System.StringStd，等於從開頭和結尾移除泛空白字元後的這個執行個體。</returns>
        public StringStd Trim()
        {
            return new StringStd(this.Value.Trim());
        }
        /// <summary>
        /// 將陣列中指定的字元集之所有項目從這個執行個體的開頭和結尾移除。
        /// </summary>
        /// <param name="trimChars">要移除的 Unicode 字元陣列或 null。</param>
        /// <returns>剩餘的 System.StringStd，從這個執行個體的開頭和結尾移除 trimChars 中字元的所有項目之後。如果 trimChars 為 null，則反而會移除泛空白字元。</returns>
        public StringStd Trim(params char[] trimChars)
        {
            return new StringStd(this.Value.Trim(trimChars));
        }
        /// <summary>
        /// 將陣列中指定的字元集之所有項目從這個執行個體的結尾移除。
        /// </summary>
        /// <param name="trimChars">要移除的 Unicode 字元陣列或 null。</param>
        /// <returns>剩餘的 System.StringStd，從 trimChars 中移除字元的所有項目之後。如果 trimChars 為 null，則反而會移除泛空白字元。</returns>
        public StringStd TrimEnd(params char[] trimChars)
        {
            return new StringStd(this.Value.TrimEnd(trimChars));
        }
        /// <summary>
        /// 將陣列中指定的字元集之所有項目從這個執行個體的開頭移除。
        /// </summary>
        /// <param name="trimChars"></param>
        /// <returns>在 trimChars 中之字元的所有項目移除之後，剩餘的 System.StringStd。如果 trimChars 為 null，則反而會移除泛空白字元。</returns>
        public StringStd TrimStart(params char[] trimChars)
        {
            return new StringStd(this.Value.TrimStart(trimChars));
        }

    }
}
