using EFFC.Frame.Net.Base.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Base.Token
{
    public class TransactionTokenCollection
    {
        Dictionary<string, TransactionToken> _d = new Dictionary<string, TransactionToken>();
        TransactionToken _default;

        public TransactionToken[] Items
        {
            get
            {
                return _d.Values.ToArray();
            }
        }

        public string[] Keys
        {
            get
            {
                return _d.Keys.ToArray();
            }
        }

        public void Add(TransactionToken token)
        {
            if (!_d.ContainsKey(token.UniqueID))
            {
                _d.Add(token.UniqueID, token);
            }
            else
            {
                throw new ItemExistsException("Token added exists");
            }
        }

        public TransactionToken this[string key]
        {
            get
            {
                return _d[key];
            }
        }

        public void Remove(TransactionToken token)
        {
            if (token.CurrentStatus == TransactionToken.TransStatus.None)
            {
                _d.Remove(token.UniqueID);
                token.Release();
            }
            else
            {
                throw new FrameException("The token can't be removed because it's in using.");
            }
        }

        public void Clear()
        {
            _d.Clear();
        }
    }
}
