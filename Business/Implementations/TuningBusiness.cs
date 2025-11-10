using AutoMapper;
using Business.Interfaces;
using Data.Interfaces;
using Entity.Dtos;
using Entity.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Exceptions;

namespace Business.Implementations
{
    public class TuningBusiness : RepositoryBusiness<Tuning, TuningDto>, ITuningBusiness
    {
        private readonly ITuningData _data;
        private readonly IMapper _mapper;
        private readonly ILogger<TuningBusiness> _logger;
        public TuningBusiness(ITuningData data, IMapper mapper, ILogger<TuningBusiness> logger)
            : base(data, mapper)
        {
            _data = data;
            _mapper = mapper;
            _logger = logger;
        }

    }
}
