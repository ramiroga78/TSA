using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSA.Infrastructure.Entities;
using TSA.Infrastructure.Interfaces;
using TSALibrary.Models;
namespace TSA.Infrastructure.Services
{
    public class DeltaServiceHistory : BaseService, IDeltaHistoryService
    {
        public DeltaServiceHistory(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        public async Task SetChanges(Delta delta)
        {
            try
            {
                DeltaHistory deltaHistory = new DeltaHistory();

                deltaHistory.IdHistory = delta.Id;
                deltaHistory.DeltaTypeId = delta.DeltaTypeId;
                deltaHistory.EventCode = delta.EventCode;
                deltaHistory.EventName = delta.EventName;
                deltaHistory.EventDescription = delta.EventDescription;
                deltaHistory.StopService = delta.StopService;
                deltaHistory.ControlOperator = delta.ControlOperator;
                deltaHistory.ControlOperatorValue = delta.ControlOperatorValue;
                deltaHistory.AddUserId = delta.AddUserId;
                deltaHistory.EditUserId = delta.EditUserId;
                deltaHistory.EditDate = delta.EditDate;
                deltaHistory.IsActive = delta.IsActive;

                await _unitOfWork.DeltaHistoryRepository.Insert(deltaHistory);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (System.Exception ex)
            {
                //log
            }
        }
    }
}
