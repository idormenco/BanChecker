using System.Collections.Generic;
using System.Linq;

namespace BanCheckerWPF.Classes
{
    public class Message
    {
        public List<object> MessageList = new List<object>();
        public bool Fresh { get; set; }

        public Message()
        {
            
        }
        public Message(List<object> objects)
        {
            foreach (var o in objects)
            {
                MessageList.Add(o);
            }
        }
        public Message(List<object> objects,bool fresh )
        {
            Fresh = fresh;
            foreach (var o in objects)
            {
                MessageList.Add(o);
            }
        }

        public override string ToString()
        {
            var ret = MessageList.Aggregate("(", (current, o) => current + (o.ToString() + ","));
            ret = ret.Substring(0, ret.Length - 1);
            ret += ")";
            return ret;
        }
    }
}
