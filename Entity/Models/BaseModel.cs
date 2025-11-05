using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Models
{
    public abstract class BaseModel
    {
        public int Id { get; set; }
        public bool Asset { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
