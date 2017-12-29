using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace EFFC.Frame.Net.Base.Common
{
    /// <summary>
    /// 将数组对象进行动态化，可以根据数组中对象类型的名称来获取指定的对象
    /// 如：FrameExposeArray.From(new obj[]{"a",1,httpcontext}).String.value 就可以获得值"a"
    /// 注意事项：
    /// 1.如果数组中同类型的对象有多个，则会形成一个同类型对象的列表，这个时候如果需要获得对象则需要通过下标进行访问，如果下标超过列表的长度，则只返回列表的最后一个对象
    /// 如:FrameExposeArray.From(new obj[]{"a","b",httpcontext}).String[2] 则返回“b”
    /// 2..value只会返回同类型对象列表中的第一个对象，如果要访问整个对象列表，请将对象转化成FrameExposeArray强类型通过下标来访问即可
    /// 如：FrameExposeArray.From(new obj[]{"a","b",httpcontext}).String.value只会返回“a”，((FrameExposeArray)FrameExposeArray.From(new obj[]{"a","b",httpcontext}))["string"]则返回["a","b"]
    /// 3..string会造成编译错误，可以使用String替代，如：FrameExposeArray.From(new obj[]{"a","b",httpcontext}).string会造成编译错误，改为FrameExposeArray.From(new obj[]{"a","b",httpcontext}).String
    /// </summary>
    public class FrameExposedArray:DynamicObject
    {
        protected object[] m_array;
        protected Dictionary<string, List<object>> m_typeobject;
        protected string currentTypeName = "";
        protected FrameExposedArray(object[] array)
        {
            m_array = array;
            m_typeobject = new Dictionary<string, List<object>>();
            if (array != null)
            {
                foreach (var item in array)
                {
                    if (item == null) continue;
                    var t_item = item.GetType();
                    var simpletypename = t_item.Name.ToLower();
                    simpletypename = simpletypename.IndexOf("`") > 0 ? simpletypename.Split('`')[0]: simpletypename;
                    if (!m_typeobject.ContainsKey(t_item.FullName))
                    {
                        m_typeobject.Add(t_item.FullName, new List<object>());
                    }
                    if (!m_typeobject.ContainsKey(simpletypename))
                    {
                        m_typeobject.Add(simpletypename, new List<object>());
                    }

                    m_typeobject[t_item.FullName].Add(item);
                    m_typeobject[simpletypename].Add(item);
                }
            }
        }
        /// <summary>
        /// 将数组转化为动态对象，可以根据Type类型来获取对应的数组的值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static dynamic From(object[] obj)
        {
            return new FrameExposedArray(obj);
        }
        /// <summary>
        /// 根据下标获取数组对象
        /// </summary>
        /// <param name="index">数组列表的下标，如果超过数组长度则返回数组中的最后一个对象</param>
        /// <returns></returns>
        public object this[int index]
        {
            get
            {
                if(Array.Length > index)
                {
                    return Array[index];
                }
                else
                {
                    return Array[Array.Length - 1];
                }
            }
        }
        /// <summary>
        /// 根据类型名称获取同类型对象列表
        /// </summary>
        /// <param name="typename"></param>
        /// <returns></returns>
        public List<object> this[string typename]
        {
            get
            {
                if (m_typeobject.ContainsKey(typename.ToLower()))
                {
                    return m_typeobject[typename.ToLower()];
                }
                else
                {
                    return new List<object>();
                }
            }
        }
        /// <summary>
        /// 根据类型名称和指定下标获取数组中的对象
        /// </summary>
        /// <param name="typename">类型名称</param>
        /// <param name="index">同类型名称对象列表中的下标，如果下标超过列表长度，则只抓取最后一个对象</param>
        /// <returns>如果未找到则返回null</returns>
        public object this[string typename,int index]
        {
            get
            {
                if (m_typeobject.ContainsKey(typename.ToLower()))
                {
                    var lcount = m_typeobject[typename.ToLower()].Count;
                    if (lcount <= 0)
                    {
                        return null;
                    }else if(lcount == 1)
                    {
                        return m_typeobject[typename.ToLower()][0];
                    }
                    else
                    {
                        return lcount > index ? m_typeobject[typename.ToLower()][index] : m_typeobject[typename.ToLower()][lcount - 1];
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        public object[] Array { get { return m_array; } }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var typename = binder.Name.ToLower();
            if(typename != "value")
            {
                currentTypeName = typename;
                result = this;
            }
            else
            {
                if(currentTypeName == "")
                {
                    result = this;
                }
                else
                {
                    if (m_typeobject.ContainsKey(currentTypeName))
                    {
                        if (m_typeobject[currentTypeName].Count > 0)
                        {
                            result = m_typeobject[currentTypeName][0];
                        }
                        else
                        {
                            result = null;
                        }
                    }
                    else
                    {
                        result = null;
                    }
                }
            }

            return true;
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            if (currentTypeName == "")
            {
                if (indexes.Length <= 0)
                {
                    result = null;
                }
                else if (indexes.Length == 1)
                {
                    var typename = ComFunc.nvl(indexes[0]).ToLower();
                    currentTypeName = typename;
                    result = this;
                }
                else
                {
                    if (indexes[1] is int)
                    {
                        result = this[currentTypeName, (int)indexes[1]];
                    }
                    else
                    {
                        result = null;
                    }
                }
            }
            else
            {
                if (indexes.Length <= 0)
                {
                    result = null;
                }
                else
                {
                    if (m_typeobject.ContainsKey(currentTypeName))
                    {
                        if(indexes[0] is int)
                        {
                            var idx = (int)indexes[0];
                            if(idx < m_typeobject[currentTypeName].Count)
                            {
                                result = m_typeobject[currentTypeName][idx];
                            }
                            else
                            {
                                result = m_typeobject[currentTypeName][m_typeobject[currentTypeName].Count-1];
                            }
                        }
                        else
                        {
                            result = null;
                        }
                    }
                    else
                    {
                        result = null;
                    }
                }
            }
            return true;
        }
    }
}
