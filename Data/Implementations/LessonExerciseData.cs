using AutoMapper;
using Data.Interfaces;
using Entity.Contexts;
using Entity.Dtos;
using Entity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Audit.Services;
using Utilities.Interfaces;

namespace Data.Implementations
{
    public class LessonExerciseData : RepositoryData<LessonExercise>, ILessonExerciseData
    {
        public LessonExerciseData(ApplicationDbContext context, IConfiguration configuration, IAuditService auditService, ICurrentUserService currentUserService)
            : base(context, configuration,auditService, currentUserService)
        {

        }

        public async Task<IEnumerable<LessonExerciseDto>> GetAllJoinAsync()
        {
            try
            {
                var lst = await _context.LessonExercises
                    .Where(x => !x.IsDeleted)
                    .Include(x => x.Lesson)
                    .Include(x => x.Exercise)
                    .Select(x => new LessonExerciseDto
                    {
                        Id = x.Id,
                        LessonId = x.LessonId,
                        ExerciseId = x.ExerciseId,

                        // 🔹 Solo los distintos
                        Lesson =  x.Lesson.Name ?? "" ,
                        Exercise =  x.Exercise.Name ?? "" 
                    })
                    .ToListAsync();

                return lst;
            }
            catch (DbException ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
                throw;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"EF update error: {ex.InnerException?.Message}");
                throw;
            }
        }

    }
}
