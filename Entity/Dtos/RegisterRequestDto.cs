using Entity.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Dtos
{
    public class RegisterRequestDto
    {
        public string Name { get; set; } = string.Empty;
        public SkillLevel SkillLevel { get; set; } = SkillLevel.Beginner;
        public string SkillLevelName => SkillLevel.ToString();
        public int ExperienceYears { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
    }
}
