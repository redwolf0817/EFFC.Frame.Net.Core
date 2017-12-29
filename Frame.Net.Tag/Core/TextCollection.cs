using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JoJo.Frame.Net.Base.Data;

namespace JoJo.Frame.Net.Tag.Core
{
    public class TextCollection:DataCollection
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


        public sealed class TextEntity
        {
            Dictionary<string, string> _processedTexts = new Dictionary<string, string>();

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

            public Dictionary<string, string> Blocks
            {
                get
                {
                    return _processedTexts;
                }
            }
        }
    }
}
