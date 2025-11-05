using AutoMapper;
using Data.Interfaces;
using Entity.Contexts;
using Entity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Audit.Services;
using Utilities.Exceptions;
using Utilities.Interfaces;

namespace Data.Implementations
{
    public class GuitaristLessonData : RepositoryData<GuitaristLesson>, IGuitaristLessonData
    {
        private readonly ILogger<GuitaristLessonData> _logger;

        public GuitaristLessonData(ApplicationDbContext context, IConfiguration configuration,  ILogger<GuitaristLessonData> logger, IAuditService auditService, ICurrentUserService currentUserService)
            : base(context, configuration, auditService, currentUserService)
        {
            _logger = logger;
        }

        public async Task<IEnumerable<GuitaristLesson>> GetAllJoinAsync()
        {
            //await AuditAsync("GetAllJoinAsync");

            return await _context.GuitaristLessons
                .Include(x => x.Guitarist)
                .Include(x => x.Lesson)
                .ToListAsync();
        }
        public override async Task<GuitaristLesson> GetById(int id)
        {
            //await AuditAsync("GetById", id);
            return await _context.GuitaristLessons
                .Include(ru => ru.Guitarist)
                .Include(ru => ru.Lesson)
                .FirstOrDefaultAsync(ru => ru.Id == id);
        }
    }
}
