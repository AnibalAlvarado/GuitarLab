using GuitarLabMobile.Dtos;
using GuitarLabMobile.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarLabMobile.Dtos
{
    public class ExerciseDto : GenericDto
    {
        public Difficulty Difficulty { get; set; } = Difficulty.Easy;

        public string DifficultyName => Difficulty.ToString();

        public int BPM { get; set; }
        public string TabNotation { get; set; } = string.Empty;

        public int TuningId { get; set; }

        // Relaciones
        public string? Tuning { get; set; }
    }
}