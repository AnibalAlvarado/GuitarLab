using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Models.Security
{
    public abstract class GenericModel : BaseModel
    {
        [StringLength(maximumLength: 30)]
        public string Code { get; set; } = null!;

        [StringLength(maximumLength: 255)]
        public string Name { get; set; } = null!;
    }
}
