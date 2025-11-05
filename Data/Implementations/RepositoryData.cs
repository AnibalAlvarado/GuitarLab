using AutoMapper;
using Data.Interfaces;
using Entity.Annotations;
using Entity.Contexts;
using Entity.Dtos;
using Entity.Models;
using Entity.Models.Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Utilities.Audit.Services;
using Utilities.Exceptions;
using Utilities.Helpers;
using Utilities.Interfaces;
using static Dapper.SqlMapper;

namespace Data.Implementations
{
    public class RepositoryData<T> : ARepositoryData<T> where T : BaseModel
    {
        protected readonly ApplicationDbContext _context;
        protected readonly IConfiguration _configuration;
        private readonly IAuditService _auditService;  // <-- Aquí la inyección
        private readonly ICurrentUserService _currentUserService;

        public RepositoryData(ApplicationDbContext context, IConfiguration configuration, IAuditService auditService, ICurrentUserService currentUserService)
        {
            _context = context;
            _configuration = configuration;
            _auditService = auditService;
            _currentUserService = currentUserService;
        }
        protected async Task AuditAsync(string action, int entityId = 0, string changes = null)
        {
            var entry = new AuditLog
            {
                Action = action,
                EntityName = typeof(T).Name,
                EntityId = entityId,
                Timestamp = DateTime.UtcNow,
                UserName = _currentUserService.UserName ?? "SYSTEM",
                Changes = changes ?? "Sin detalles"
            };

            await _auditService.SaveAuditAsync(entry);
        }

        public override async Task<IEnumerable<T>> GetAll()
        {
            try
            {
                var lstModel = await _context.Set<T>()
                    .Where(x => x.IsDeleted == false) // 👈 Solo activos
                    .ToListAsync();

                return lstModel;
            }
            catch (DbException ex)
            {
                Console.WriteLine("Database error: " + ex.Message);
                throw;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Database update (EF) failed: " + ex.InnerException?.Message);
                throw;
            }
        }


        public override  async Task<T> GetById(int id)
        {

            try
            {
                var entity = await _context.Set<T>().AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);

                // Auditar acción GetById, enviamos la entidad si la encontró
                //await AuditAsync("GetById", id);

                return entity;
            }
            catch (DbException ex)
            {
                Console.WriteLine("Database error: " + ex.Message);
                throw;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Database update (EF) failed: " + ex.InnerException?.Message);
                throw;
            }


        }

        public override  async Task<T> Save(T entity)
        {
            try
            {
                _context.Set<T>().Add(entity);
                await _context.SaveChangesAsync();
                //await AuditAsync("Save", entity.Id);
                return entity;
            }
            catch (DbException ex)
            {
                Console.WriteLine("Database error: " + ex.Message);
                throw;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Database update (EF) failed: " + ex.InnerException?.Message);
                throw;
            }


        }

        public override  async Task Update(T entity)
        {
            try
            {
                _context.Entry(entity).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                //await AuditAsync("Update", entity.Id);
                _context.Entry(entity).State = EntityState.Detached;
            }
            catch (DbException ex)
            {
                Console.WriteLine("Database error: " + ex.Message);
                throw;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Database update (EF) failed: " + ex.InnerException?.Message);
                throw;
            }


        }

        public override async Task<bool> PermanentDelete(int id)
        {
            if (id <= 0)
                throw new DataException("El ID proporcionado no es válido. Debe ser mayor a cero.");

            var entity = await _context.Set<T>().FirstOrDefaultAsync(d => d.Id == id);

            if (entity == null)
                throw new DataException($"No se encontró un registro con el ID {id}.");

            try
            {
                _context.Remove(entity);
                await _context.SaveChangesAsync();
                //await AuditAsync("Delete", id);
                return true;
            }
            catch (DbUpdateException ex)
            {
                throw new DataException("Ocurrió un error al intentar eliminar el registro.", ex);
            }
            catch (Exception ex)
            {
                throw new DataException("Error inesperado durante la eliminación.", ex);
            }
        }

        public override async Task<int> Delete(int id)
        {
            try
            {
                var entity = await _context.Set<T>().FirstOrDefaultAsync(d => d.Id == id);
                if (entity == null)
                    throw new DataException($"No se encontró un registro con el ID {id}.");

                // Marcar como inactivo (soft delete)
                entity.IsDeleted = true;

                _context.Entry(entity).State = EntityState.Modified;
                int result = await _context.SaveChangesAsync();

                //await AuditAsync("Logical Delete", id);

                return result;
            }
            catch (DbException ex)
            {
                Console.WriteLine("Database error: " + ex.Message);
                throw;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Database update (EF) failed: " + ex.InnerException?.Message);
                throw;
            }
        }

        public override async Task<List<ExpandoObject>> GetAllDynamicAsync()
        {
            var entityType = typeof(T);
            var query = _context.Set<T>().AsQueryable();

             var foreignKeyProps = entityType
                 .GetProperties()
                 .Where(p => Attribute.IsDefined(p, typeof(ForeignIncludeAttribute)))
                 .ToList();

            // Incluye las propiedades de navegación en la consulta
            foreach (var prop in foreignKeyProps)
            {
                query = query.Include(prop.Name); // ejemplo: "Form", "Module"
            }

            var resultList = await query.ToListAsync();
            var dynamicList = new List<ExpandoObject>();

            foreach (var entity in resultList)
            {
                dynamic dyn = new ExpandoObject();
                var dict = (IDictionary<string, object?>)dyn;

                // ID principal
                dict["Id"] = entityType.GetProperty("Id")?.GetValue(entity);

                foreach (var prop in foreignKeyProps)
                {
                    var attr = prop.GetCustomAttribute<ForeignIncludeAttribute>()!;
                    var foreignValue = prop.GetValue(entity);

                    if (foreignValue == null) continue;

                    // Si no hay rutas especificadas, incluye el objeto completo
                    if (attr.SelectPaths == null || attr.SelectPaths.Length == 0)
                    {
                        dict[prop.Name] = foreignValue;
                    }
                    else
                    {
                        foreach (var path in attr.SelectPaths)
                        {
                            var value = ReflectionHelper.GetNestedPropertyValue(foreignValue, path);
                            var key = ReflectionHelper.PascalJoin(prop.Name, path);
                            dict[key] = value;
                        }
                    }
                }

                dynamicList.Add(dyn);
            }

            return dynamicList;
        }
    }
}
