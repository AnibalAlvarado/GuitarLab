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
    public class FormModuleBusiness : RepositoryBusiness<FormModule, FormModuleDto>, IFormModuleBusiness
    {
        private readonly ILessonExerciseData _data;
        private readonly IMapper _mapper;
        public FormModuleBusiness(ILessonExerciseData data, IMapper mapper)
            : base(data, mapper)
        {
            _data = data;
            _mapper = mapper;
        }

        public async Task<IEnumerable<FormModuleDto>> GetAllJoinAsync()
        {
            var entities = await _data.GetAllJoinAsync();
            return _mapper.Map<IEnumerable<FormModuleDto>>(entities);
        }

    }
}
