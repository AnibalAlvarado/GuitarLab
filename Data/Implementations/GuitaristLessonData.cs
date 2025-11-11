using AutoMapper;
using Data.Interfaces;
using Entity.Contexts;
using Entity.Dtos;
using Entity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
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

        public async Task<IEnumerable<GuitaristLessonDto>> GetAllJoinAsync()
        {
            try
            {
                var lst = await _context.GuitaristLessons
                    .Where(x => !x.IsDeleted)
                    .Include(x => x.Guitarist)
                    .Include(x => x.Lesson)
                    .Select(x => new GuitaristLessonDto
                    {
                        // 🔹 Campos que se copian tal cual (mismo nombre y tipo)
                        Id = x.Id,
                        GuitaristId = x.GuitaristId,
                        LessonId = x.LessonId,
                        Status = x.Status,
                        ProgressPercent = x.ProgressPercent,

                        // 🔹 Solo se mapean los diferentes
                        Guitarist = x.Guitarist.Name ?? "",
                        Lesson = x.Lesson.Name ?? ""
                    })
                    .ToListAsync();

                return lst;
            }
            catch (DbException ex)
            {
                Console.WriteLine("Database error: " + ex.Message);
                throw;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("EF update error: " + ex.InnerException?.Message);
                throw;
            }
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
