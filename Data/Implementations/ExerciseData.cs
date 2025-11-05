using AutoMapper;
using Data.Interfaces;
using Entity.Contexts;
using Entity.Models;
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
    public class ExerciseData : RepositoryData<Exercise>, IExerciseData
    {
        public ExerciseData(ApplicationDbContext context, IConfiguration configuration, IAuditService auditService, ICurrentUserService currentUserService)
            : base(context, configuration, auditService, currentUserService)
        {

        }
    }
}
