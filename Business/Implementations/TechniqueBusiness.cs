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
    public class TechniqueBusiness : RepositoryBusiness<Technique, TechniqueDto>, ITechniqueBusiness
    {
        private readonly ITechniqueData _data;
        public TechniqueBusiness(ITechniqueData data, IMapper mapper)
            : base(data, mapper)
        {
            _data = data;
        }
    }
}
