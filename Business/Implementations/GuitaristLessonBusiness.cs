using AutoMapper;
using Business.Interfaces;
using Data.Interfaces;
using Entity.Dtos;
using Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Implementations
{
    public class GuitaristLessonBusiness : RepositoryBusiness<GuitaristLesson, GuitaristLessonDto>, IGuitaristLessonBusiness
    {
        private readonly IGuitaristLessonData _data;
        private readonly IMapper _mapper;
        public GuitaristLessonBusiness(IGuitaristLessonData data, IMapper mapper)
            : base(data, mapper)
        {
            _data = data;
            _mapper = mapper;
        }

        public async Task<IEnumerable<GuitaristLessonDto>> GetAllJoinAsync()
        {
            var entities = await _data.GetAllJoinAsync();
            return _mapper.Map<IEnumerable<GuitaristLessonDto>>(entities);
        }

    }
}
