using System.Collections.Generic;
using System.Linq;

namespace BanCheckerWPF.Classes
{
    public class EncryptedMessage
    {
        public EncryptedMessage(string key)
        {
            Key = key;
        }

        public string Key { get; set; }
        public List<object> MessageList = new List<object>();

        public override string ToString()
        {
            var ret = MessageList.Aggregate("{", (current, o) => current + (o.ToString() + ","));
            ret = ret.Substring(0, ret.Length - 1);
            ret += "}_" + Key;
            return ret;
        }
    }
}
