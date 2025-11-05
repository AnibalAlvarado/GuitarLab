using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Models.Security
{
    public class Form : GenericModel
    {
        public string Url { get; set; }
        public string Icon { get; set; }
        public int Module { get; set; }
        public bool SuperAdmin { get; set; }
        public int ModuleId { get; set; }

    }
}
