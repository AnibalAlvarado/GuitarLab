using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Models
{
    public class LessonExercise : BaseModel
    {
        public int LessonId { get; set; }
        public int ExerciseId { get; set; }

        // Relaciones
        public Lesson Lesson { get; set; } = new Lesson();
        public Exercise Exercise { get; set; } = new Exercise();
    }
}
