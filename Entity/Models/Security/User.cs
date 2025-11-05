using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Models.Security
{
    public class User : BaseModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }

    }
}
