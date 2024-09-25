using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCMCHOVR.Models.Pages
{
    public class OVRRequest
    {
        public IDictionary<string, string> Details { get; set; }
        public IList<string> Witness { get; set; }

    }
    public class OVRActionRequest 
    {
        public IDictionary<string, string> Details { get; set; }
        public IList<string> Witness { get; set; }

    }
}
