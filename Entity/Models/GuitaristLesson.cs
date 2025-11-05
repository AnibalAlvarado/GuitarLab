using Entity.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Models
{
    public class GuitaristLesson : BaseModel
    {
        public int GuitaristId { get; set; }
        public int LessonId { get; set; }

        public LessonStatus Status { get; set; } = LessonStatus.NotStarted;
        public double ProgressPercent { get; set; } = 0;

        // Relaciones
        public Guitarist Guitarist { get; set; } = new Guitarist();
        public Lesson Lesson { get; set; } = new Lesson();
    }
}
