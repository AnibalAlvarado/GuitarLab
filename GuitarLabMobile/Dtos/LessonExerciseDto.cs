using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarLabMobile.Dtos
{
    public class LessonExerciseDto : BaseDto
    {
        public int LessonId { get; set; }
        public int ExerciseId { get; set; }

        // Relaciones
        public string? Lesson { get; set; }
        public string? Exercise { get; set; }
    }
}