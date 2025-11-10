using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Models
{
    public class Lesson : GenericModel
    {
        public string? Description { get; set; }
        public int TechniqueId { get; set; }

        // Relaciones
        public Technique Technique { get; set; }
        public List<GuitaristLesson> GuitaristLessons { get; set; } = new List<GuitaristLesson>();
        public List<LessonExercise> LessonExercises { get; set; } = new List<LessonExercise>();
    }
}
