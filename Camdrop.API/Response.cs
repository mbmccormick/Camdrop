using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Camdrop.API
{
    public class Response
    {
        public int status { get; set; }
        public virtual object items { get; set; }
        public string status_description { get; set; }
        public string status_detail { get; set; }
    }
}
