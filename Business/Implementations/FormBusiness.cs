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
    public class FormBusiness : RepositoryBusiness<Form, FormDto>, IFormBusiness
    {
        private readonly ILessonData _data;
        public FormBusiness(ILessonData data, IMapper mapper)
            : base(data, mapper)
        {
            _data = data;
        }
    }
}
