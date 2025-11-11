using GuitarLabMobile.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarLabMobile.Models
{
    public class Exercise : GenericModel
    {
        public Difficulty Difficulty { get; set; } = Difficulty.Easy;
        public int BPM { get; set; }
        public string TabNotation { get; set; } = string.Empty;

        public int TuningId { get; set; }

        // Relaciones
        public Tuning Tuning { get; set; }
        public List<LessonExercise> LessonExercises { get; set; } = new List<LessonExercise>();
    }
}