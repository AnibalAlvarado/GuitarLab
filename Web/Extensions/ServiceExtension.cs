using Business.Implementations;
using Business.Interfaces;
using Data.Implementations;
using Data.Interfaces;
using Entity.Context;
using Entity.DTOs;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using Utilities.Audit.Services;
using Utilities.Audit.Strategy;
using Utilities.BackgroundTasks;
using Utilities.Exceptions;
using Utilities.Helpers;
using Utilities.Helpers.Validators;
using Utilities.Implementations;
using Utilities.Interfaces;

namespace Web.Extensions
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddAppServices(this IServiceCollection services)
        {
            //segundo plano
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
            services.AddHostedService<QueuedHostedService>();
            // sin necesidad de crear Business o Data concreta
            services.AddScoped<IRepositoryBusiness<Rol, RolDto>, RepositoryBusiness<Rol, RolDto>>();
            services.AddScoped<IRepositoryData<Rol>, RepositoryData<Rol>>();
            //Obtener Usuario
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            // Inyección de dependencias para auditoría con Strategy
            
            services.AddScoped<IAuditService, AuditService>();
            services.AddScoped<IEmailService, EmailService>();
            // Genéricos base
            services.AddScoped(typeof(IRepositoryBusiness<,>), typeof(RepositoryBusiness<,>));
            services.AddScoped(typeof(IRepositoryData<>), typeof(RepositoryData<>));

            // Concretos
            services.AddScoped<IFormBusiness, FormBusiness>();
            services.AddScoped<ILessonData, LessonData>();

            services.AddScoped<IFormModuleBusiness, FormModuleBusiness>();
            services.AddScoped<ILessonExerciseData, LessonExerciseData>();

            services.AddScoped<IModuleBusiness, ModuleBusiness>();
            services.AddScoped<IModuleData, ModuleData>();

            services.AddScoped<IPermissionBusiness, PermissionBusiness>();
            services.AddScoped<ITuningData, TuningData>();

            services.AddScoped<IPersonBusiness, PersonBusiness>();
            services.AddScoped<ITechniqueData, TechniqueData>();

            services.AddScoped<IRolBusiness, RolBusiness>();
            services.AddScoped<IRolData, GuitaristLesson>();

            services.AddScoped<IRolFormPermissionBusiness, RolFormPermissionBusiness>();
            services.AddScoped<IRolFormPermissionData, RolFormPermissionData>();

            services.AddScoped<IRolUserBusiness, RolUserBusiness>();
            services.AddScoped<IGuitaristLessonData, GuitaristLessonData>();

            services.AddScoped<IUserBusiness, UserBusiness>();
            services.AddScoped<IUserData, GuitaristData>();

            services.AddTransient<Validations>();
            return services;
        }
    }
}