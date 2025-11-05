using AutoMapper;
using Data.Interfaces;
using Entity.Contexts;
using Entity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
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

        public async Task<IEnumerable<LessonExercise>> GetAllJoinAsync()
        {
            return await _context.LessonExercises
                .Include(x => x.Lesson)
                .Include(x => x.Exercise)
                .ToListAsync();
        }
    }
}
