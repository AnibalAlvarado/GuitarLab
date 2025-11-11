using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarLabMobile.Dtos
{
    public class GenericDto : BaseDto
    {
        [StringLength(maximumLength: 255)]
        public string Name { get; set; } = null!;
    }
}