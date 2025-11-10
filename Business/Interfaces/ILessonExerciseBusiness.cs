using Entity.Dtos;
using Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface ILessonExerciseBusiness : IRepositoryBusiness<LessonExercise, LessonExerciseDto>
    {

        Task<IEnumerable<LessonExerciseDto>> GetAllJoinAsync();

    }

}
