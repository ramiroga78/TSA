using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TSA.Infrastructure.DTOs;
using TSA.Infrastructure.Interfaces;
using TSALibrary.Models;

namespace TSA.Infrastructure.Services
{
    public class RequestLogService : BaseService, IRequestLogService
    {
        public RequestLogService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        public async Task<IEnumerable<RequestLog>> GetAllRequestLog()
        {
            try
            {
                var requestLog = await _unitOfWork.RequestLogRepository.GetAllAsync();

                //return usersDto;
                return requestLog;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public async Task<RequestLog> GetRequestLogById(int id)
        {
            try
            {
                var requestLog = await _unitOfWork.RequestLogRepository.GetByIdAsync(id);

                return requestLog;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public async Task<int> AddAndSave(RequestLog requestLog)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                //var requestLogEntity = _mapper.Map<RequestLog>(requestLog);
                requestLog.RequestDate = System.DateTime.UtcNow;
                //Password logic, encrypt, etc

                await _unitOfWork.RequestLogRepository.Insert(requestLog);
                //await _unitOfWork.UserHistoryRepository.Insert(userHistory);

                int result = await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitAsync();

                return result;
            }
            catch (System.Exception)
            {
                await _unitOfWork.RollBackAsync();
                return 0;
            }
        }


        //Keep in mind that this method updates every property of the User, also those that aren't modified or aren't specified (nulling them)
        public async Task UpdateAndSave(RequestLog requestLog)
        {
            try
            {
                requestLog.ResponseDate = System.DateTime.UtcNow;
                _unitOfWork.RequestLogRepository.Update(requestLog);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (System.Exception)
            {
                throw;
            }
        }


        public IQueryable<RequestLog> GetIQueryable()
        {
            return _unitOfWork.RequestLogRepository.GetIQueryable();
        }

        public async Task<int> CountRequestLog()
        {
            return await _unitOfWork.RequestLogRepository.CountRequestLog();
        }

        public async Task<string> GetRequest(int id)
        {
            var requestLog = await _unitOfWork.RequestLogRepository.GetByIdAsync(id);
            var requestResponseDto = new RequestResponseDTO();
            if (requestLog.Request != null)
            {
                requestResponseDto.Request = requestLog.Request;
            }
            else
                requestResponseDto.Request = "<comment> Request vacío </comment>";
            return requestResponseDto.Request;
        }
        public async Task<string> GetResponse(int id)
        {
            var requestLog = await _unitOfWork.RequestLogRepository.GetByIdAsync(id);
            var requestResponseDto = new RequestResponseDTO();
            if (requestLog.Response != null)
            {
                requestResponseDto.Response = requestLog.Response;
            }
            else
                requestResponseDto.Response = "<comment> Response vacío </comment>";
            return requestResponseDto.Response;
        }


    }
}
