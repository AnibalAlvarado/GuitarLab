using GuitarLabMobile.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarLabMobile.Dtos
{
    public class GuitaristLessonDto : BaseDto
    {
        public int GuitaristId { get; set; }
        public int LessonId { get; set; }

        public LessonStatus Status { get; set; } = LessonStatus.NotStarted;
        public string StatusName => Status.ToString();


        public double ProgressPercent { get; set; } = 0;

        // Relaciones
        public string? Guitarist { get; set; }
        public string? Lesson { get; set; }
    }
}