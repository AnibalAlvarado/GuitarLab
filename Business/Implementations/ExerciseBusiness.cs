using AutoMapper;
using Business.Interfaces;
using Data.Interfaces;
using Entity.Dtos;
using Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Implementations
{
    public class ExerciseBusiness : RepositoryBusiness<Exercise, ExerciseDto>, IExerciseBusiness
    {
        private readonly IExerciseData _data;
        public ExerciseBusiness(IExerciseData data, IMapper mapper)
            : base(data, mapper)
        {
            _data = data;
        }
    }
}
