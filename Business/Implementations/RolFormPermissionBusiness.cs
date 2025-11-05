using AutoMapper;
using Business.Interfaces;
using Data.Interfaces;
using Entity.DTOs;
using Entity.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Implementations
{
    public class RolFormPermissionBusiness : RepositoryBusiness<RolFormPermission, RolFormPermissionDto>, IRolFormPermissionBusiness
    {
        private readonly IRolFormPermissionData _data;
        private readonly IMapper _mapper;

        public RolFormPermissionBusiness(IRolFormPermissionData data, IMapper mapper)
            : base(data, mapper)
        {
            _data = data;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RolFormPermissionDto>> GetAllJoinAsync()
        {
            var entities = await _data.GetAllJoinAsync();
            return _mapper.Map<IEnumerable<RolFormPermissionDto>>(entities);
        }

    }
}
