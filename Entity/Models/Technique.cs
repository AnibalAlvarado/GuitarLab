using MySqlX.XDevAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Models
{
    public class Technique : GenericModel
    {
        public string? Description { get; set; }

        // Relaciones
        public List<Lesson> Lessons { get; set; } = new List<Lesson>();
    }
}
