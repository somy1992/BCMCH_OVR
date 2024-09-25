using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCMCHOVR.Helpers
{
    public interface IJsonList
    {
    }

    public class JsonList
    {
        private Dictionary<string, string> Jlist { get; set; }

        public JsonList()
        {
            Jlist = new Dictionary<string, string>();
        }

        public void Add(string key, string value)
        {
            Jlist.Add(key, value);
        }

        public Dictionary<string, string> GetList()
        {
            return Jlist;
        }
    }
}
