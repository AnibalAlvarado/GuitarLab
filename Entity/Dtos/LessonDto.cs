using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Dtos
{
    public class LessonDto : GenericDto
    {
        public string? Description { get; set; }
        public int TechniqueId { get; set; }

        // Relaciones
        public string? Technique { get; set; }

    }
}
