using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Dtos
{
    public class BaseDto
    {
        public int Id { get; set; }
        public bool Asset { get; set; }

    }
}
