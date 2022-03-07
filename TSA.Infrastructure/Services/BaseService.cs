using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSA.Infrastructure.Interfaces;

namespace TSA.Infrastructure.Services
{
    public class BaseService
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IMapper _mapper;
        //protected readonly ILogger _logger;

        public BaseService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
    }
}
