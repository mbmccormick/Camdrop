using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Camdrop.API
{
    public class UsersGetCurrentResponse : Response
    {
        public new List<User> items { get; set; }
    }
}
