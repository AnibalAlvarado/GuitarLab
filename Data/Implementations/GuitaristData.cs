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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Audit.Services;
using Utilities.Interfaces;

namespace Data.Implementations
{
    public class GuitaristData : RepositoryData<Guitarist>, IGuitaristData
    {
        private readonly ILogger<GuitaristData> _logger;
        private readonly IAuditService _auditService;
        public GuitaristData(ApplicationDbContext context, IConfiguration configuration,ILogger<GuitaristData> logger, IAuditService auditService, ICurrentUserService currentUserService)
            : base(context, configuration, auditService, currentUserService)
        {
            _logger = logger;
            _auditService = auditService;
        }


        public async Task<Guitarist?> GetGuitaristByGuitaristnameAsync(string Guitaristname)
        {
            try
            {
                await AuditAsync("GetGuitaristByGuitaristnameAsync");
                return await _context.Set<Guitarist>()
                    .FirstOrDefaultAsync(u => u.Name == Guitaristname && u.Asset);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario por nombre de usuario: {Guitaristname}", Guitaristname);
                throw;
            }
        }

    }
}
