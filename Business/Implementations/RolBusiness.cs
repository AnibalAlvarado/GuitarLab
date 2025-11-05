using AutoMapper;
using Business.Interfaces;
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
    public class RolBusiness : RepositoryBusiness<Rol, RolDto>, IRolBusiness
    {
        private readonly IRolData _data;
        private readonly IMapper _mapper;
        private readonly ILogger<RolBusiness> _logger;
        public RolBusiness(IRolData data, IMapper mapper, ILogger<RolBusiness> logger)
            : base(data, mapper)
        {
            _data = data;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<RolDto> GetByNameAsync(string name)
        {
            try
            {
                var rolEntity = await _data.GetByNameAsync(name);
                return _mapper.Map<RolDto>(rolEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el rol por nombre");
                throw new BusinessException("Error al obtener el rol por nombre", ex);
            }
        }
    }
}
