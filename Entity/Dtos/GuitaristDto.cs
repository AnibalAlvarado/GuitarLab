using Entity.Dtos;
using Entity.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Dtos
{
    public class GuitaristDto : GenericDto
    {
        public SkillLevel SkillLevel { get; set; } = SkillLevel.Beginner;
        public string SkillLevelName => SkillLevel.ToString();

        public int ExperienceYears { get; set; }
    }
}
