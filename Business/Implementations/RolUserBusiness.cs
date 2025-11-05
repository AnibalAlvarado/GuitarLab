using AutoMapper;
using Business.Interfaces;
using Data.Implementations;
using Data.Interfaces;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Exceptions;

namespace Business.Implementations
{
    public class RolUserBusiness : RepositoryBusiness<RolUser, RolUserDto>, IRolUserBusiness
    {
        private readonly IGuitaristLessonData _data;
        private readonly IMapper _mapper;
        private readonly ILogger<RolUserBusiness> _logger;

        public RolUserBusiness(IGuitaristLessonData data, IMapper mapper, ILogger<RolUserBusiness> logger)
            : base(data, mapper)
        {
            _data = data;
            _mapper = mapper;
            _logger = logger;
        }


        public async Task<bool> ExistsAsync(int userId, int roleId)
        {
            try
            {
                return await _data.ExistsAsync(userId, roleId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar existencia del rol para el usuario");
                throw new BusinessException("Error al verificar existencia del rol para el usuario", ex);
            }
        }
        public async Task<IEnumerable<RolUserDto>> GetAllJoinAsync()
        {
            var entities = await _data.GetAllJoinAsync();
            return _mapper.Map<IEnumerable<RolUserDto>>(entities);
        }
        public override async Task<List<RolUserDto>> GetAll()
        {
            var entities = await _data.GetAll();
            return _mapper.Map<List<RolUserDto>>(entities);
        }

        public override async Task<RolUserDto> GetById(int id)
        {
            var entity = await _data.GetById(id);
            if (entity == null)
                throw new Exception($"No se encontró RolUser con ID {id}");
            return _mapper.Map<RolUserDto>(entity);
        }

        public override async Task<RolUserDto> Save(RolUserDto dto)
        {
            var entity = _mapper.Map<RolUser>(dto);
            entity.Asset = true;
            var saved = await _data.Save(entity);
            return _mapper.Map<RolUserDto>(saved);
        }

        public override async Task Update(RolUserDto dto)
        {
            var entity = _mapper.Map<RolUser>(dto);
            await _data.Update(entity);
        }

        public override async Task<int> Delete(int id)
        {
            return await _data.Delete(id);
        }

        public override async Task<bool> PermanentDelete(int id)
        {
            return await _data.PermanentDelete(id);
        }
    }
}
