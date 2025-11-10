using Business.Custom;
using Business.Implementations;
using Business.Interfaces;
using Business.Interfaces.Auth;
using Data.Implementations;
using Data.Implementations.Auth;
using Data.interfaces.Auth;
using Data.Interfaces;
using Entity.Context;
using Entity.Dtos;
using Entity.Models;
using Microsoft.EntityFrameworkCore;
using ModelSecurity.Infrastructure.Cookies.Implements;
using System.Configuration;
using Utilities.Audit.Services;
using Utilities.Audit.Strategy;
using Utilities.BackgroundTasks;
using Utilities.Exceptions;
using Utilities.Helpers;
using Utilities.Helpers.Validators;
using Utilities.Implementations;
using Utilities.Interfaces;
using Web.Infrastructure.Cookies.Interfaces;

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
            //services.AddScoped<IRepositoryBusiness<Rol, RolDto>, RepositoryBusiness<Rol, RolDto>>();
            //services.AddScoped<IRepositoryData<Rol>, RepositoryData<Rol>>();
            //Obtener Usuario
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            // Inyección de dependencias para auditoría con Strategy
            
            services.AddScoped<IAuditService, AuditService>();
            services.AddScoped<IEmailService, EmailService>();
            // Genéricos base
            services.AddScoped(typeof(IRepositoryBusiness<,>), typeof(RepositoryBusiness<,>));
            services.AddScoped(typeof(IRepositoryData<>), typeof(RepositoryData<>));

            // Concretos
            services.AddScoped<IExerciseBusiness, ExerciseBusiness>();
            services.AddScoped<IExerciseData, ExerciseData>();

            services.AddScoped<IGuitaristBusiness, GuitaristBusiness>();
            services.AddScoped<IGuitaristData, GuitaristData>();

            services.AddScoped<IGuitaristLessonBusiness, GuitaristLessonBusiness>();
            services.AddScoped<IGuitaristLessonData, GuitaristLessonData>();

            services.AddScoped<ILessonBusiness, LessonBusiness>();
            services.AddScoped<ILessonData, LessonData>();

            services.AddScoped<ILessonExerciseBusiness, LessonExerciseBusiness>();
            services.AddScoped<ILessonExerciseData, LessonExerciseData>();

            services.AddScoped<ITechniqueBusiness, TechniqueBusiness>();
            services.AddScoped<ITechniqueData, TechniqueData>();

            services.AddScoped<ITuningBusiness, TuningBusiness>();
            services.AddScoped<ITuningData, TuningData>();

            services.AddScoped<IUserBusiness, UserBusiness>();
            services.AddScoped<IUserData, UserData>();

            //Jwt
            services.AddScoped<ITokenBusiness, TokenBusiness>();
            services.AddScoped<IRefreshTokenData, RefreshTokenData>();
            services.AddScoped<IAuthCookieFactory, AuthCookieFactory>();

            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddTransient<Validations>();
            return services;
        }
    }
}