using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Models.Security
{
    public class Role : GenericModel
    {
        public int UserModification { get; set; }
        public int UserCreation { get; set; }
    }
}
