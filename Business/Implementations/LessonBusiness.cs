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
    public class LessonBusiness : RepositoryBusiness<Lesson, LessonDto>, ILessonBusiness
    {
        private readonly ILessonData _data;
        public LessonBusiness(ILessonData data, IMapper mapper)
            : base(data, mapper)
        {
            _data = data;
        }

        public async Task<IEnumerable<LessonDto>> GetAllJoinAsync()
        {
            return await _data.GetAllJoinAsync();
        }
    }
}
