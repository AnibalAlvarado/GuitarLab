using Entity.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Models
{
    public class Guitarist : GenericModel
    {
        public SkillLevel SkillLevel { get; set; } = SkillLevel.Beginner;

        public int ExperienceYears { get; set; }

        // Relaciones
        public List<GuitaristLesson> GuitaristLessons { get; set; } = new List<GuitaristLesson>();
    }
}
