using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RadiosMotorola.Models
{

    public class PayloadPost_ID
    {
        public string alias { get; set; }
        public IList<string> allowed_location { get; set; }
    }

    public class PayloadPost_ID_Location
    {
        public string location { get; set; }
    }

    public class PayloadGet_ID
    {
        public string location { get; set; }
    }

}
