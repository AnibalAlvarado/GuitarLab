using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Models
{
    public class Tuning : GenericModel
    {
        public string Notes { get; set; } = "EADGBE"; // default para standard

        // Relaciones
        public List<Exercise> Exercises { get; set; } = new List<Exercise>();
    }
}
