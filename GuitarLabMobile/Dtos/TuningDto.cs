using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarLabMobile.Dtos
{
    public class TuningDto : GenericDto
    {
        public string Notes { get; set; } = "EADGBE"; // default para standard

        // Relaciones
    }
}