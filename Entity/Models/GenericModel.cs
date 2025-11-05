using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Models
{
    public abstract class GenericModel : BaseModel
    {
        [StringLength(maximumLength: 255)]
        public string Name { get; set; } = null!;
    }
}
