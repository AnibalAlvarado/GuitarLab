using AutoMapper;
using Entity.Dtos;
using Entity.Models;
using System;
using System.Reflection;
using System.Security;
using Utilities.Interfaces;


namespace Utilities.Implementations
{
    public class AutoMapperProfiles : Profile
    {
        

        public AutoMapperProfiles() : base()
        {
         

            // Ejemplo de mapeo
            CreateMap<Exercise, ExerciseDto>().ReverseMap();
            CreateMap<Guitarist, GuitaristDto>().ReverseMap();
            CreateMap<Lesson, LessonDto>().ReverseMap();
            CreateMap<LessonExercise, LessonExerciseDto>().ReverseMap();
            CreateMap<GuitaristLesson, GuitaristLessonDto>().ReverseMap();
            CreateMap<Tuning, TuningDto>().ReverseMap();
            CreateMap<Technique, TechniqueDto>().ReverseMap();
            CreateMap<User, UserDto>().ReverseMap();
        }
    }
}
