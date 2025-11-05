using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Models.Security
{
    public class UserRole : BaseModel
    {
        public int RoleId { get; set; }
        public int UserId { get; set; }
    }
}
