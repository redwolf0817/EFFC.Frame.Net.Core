using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EFFC.Frame.Net.Base.Data;

namespace EFFC.Frame.Net.Tag.Core
{
    public class ContextTextCollection:DataCollection
    {
        Dictionary<string, TextEntity> _d = new Dictionary<string, TextEntity>();
        public TextEntity this[string key]
        {
            get
            {
                if (_d.ContainsKey(key))
                    return _d[key];
                else
                    return null;
            }
            set
            {
                if (_d.ContainsKey(key))
                {
                    _d.Remove(key);
                }

                _d.Add(key, value);
            }
        }

        public void AddEntity(string originaltext, string uid, string processedText)
        {
            TextEntity te = new TextEntity();
            te.OriginalText = originaltext;
            te.UID = uid;
            te.ProcessText = processedText;
            this[uid] = te;
        }

        public void AddEntity(string originaltext, string uid, string processedText, string argsstr, string tagname)
        {
            TextEntity te = new TextEntity();
            te.OriginalText = originaltext;
            te.UID = uid;
            te.ProcessText = processedText;

            this[uid] = te;
        }

        

        public sealed class TextEntity
        {
            public string OriginalText
            {
                get;
                set;
            }

            public string UID
            {
                get;
                set;
            }

            public string TagName
            {
                get;
                set;
            }

            public Dictionary<string, string> Args
            {
                get;
                set;
            }

            public string ProcessText
            {
                get;
                set;
            }
        }
    }
}
