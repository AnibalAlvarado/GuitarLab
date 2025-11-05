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
    public class ModuleBusiness : RepositoryBusiness<Module, ModuleDto>, IModuleBusiness
    {
        private readonly IModuleData _data;
        public ModuleBusiness(IModuleData data, IMapper mapper)
            : base(data, mapper)
        {
            _data = data;
        }
    }
}
