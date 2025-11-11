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
    public class UserData : RepositoryData<User>, IUserData
    {
        public UserData(ApplicationDbContext context, IConfiguration configuration, IAuditService auditService, ICurrentUserService currentUserService)
            : base(context, configuration, auditService, currentUserService)
        {

        }

        public async Task<User?> GetByEmailOrUsernameAsync(string emailOrUsername)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.Email == emailOrUsername || u.Username == emailOrUsername);
        }

        public async Task<IEnumerable<UserDto>> GetAllJoinAsync()
        {
            try
            {
                var lst = await _context.Users
                    .Where(x => !x.IsDeleted)
                    .Include(x => x.Guitarist)
                    .Select(x => new UserDto
                    {
                        Id = x.Id,
                        Username = x.Username,
                        Email = x.Email,
                        Password = "***",
                        GuitaristId = x.GuitaristId,
                        Guitarist = x.Guitarist.Name ?? ""
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
