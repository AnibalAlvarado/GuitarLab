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
    public class LessonData : RepositoryData<Lesson>, ILessonData
    {
        public LessonData(ApplicationDbContext context, IConfiguration configuration, IAuditService auditService, ICurrentUserService currentUserService)
            : base(context, configuration, auditService, currentUserService)
        {

        }

        public  async Task<IEnumerable<LessonDto>> GetAllJoinAsync()
        {
            try
            {
                var lst = await _context.Lessons
                    .Where(x => !x.IsDeleted)
                    .Include(x => x.Technique)
                    .Select(x => new LessonDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Description = x.Description,
                        TechniqueId = x.TechniqueId,
                        Technique = x.Technique != null ? x.Technique.Name : null
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
    }
}
