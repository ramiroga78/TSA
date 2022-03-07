using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TSA.Infrastructure.DTOs;
using TSA.Infrastructure.Interfaces;
using TSALibrary.Models;

namespace TSA.Infrastructure.Services
{
    public class MessageService : BaseService, IMessageService
    {
        private new readonly IUnitOfWork _unitOfWork;
        private new readonly IMapper _mapper;

        public MessageService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Message>> GetTopNMessages(int messagesNumber, string orderByFieldName)
        {
            try
            {
                var messages = await _unitOfWork.MessageRepository.GetTopNMessages(messagesNumber, orderByFieldName);

                return messages;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public async Task AddAndSave(Message message)
        {
            try
            {
                await _unitOfWork.MessageRepository.Insert(message);

                int result = await _unitOfWork.SaveChangesAsync();
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public async Task MarkAsSent(int id, string email)
        {
            try
            {
                await _unitOfWork.MessageRepository.MarkAsSent(id, email);

                await _unitOfWork.SaveChangesAsync();
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        //public async Task<IQueryable<MessageLogDTO>> GetIQueryable()
        //{
        //    var listMessages = await _unitOfWork.MessageRepository.GetAllAsync();
        //    var listUsers = await _unitOfWork.UserRepository.GetAllAsync();
        //    var listMessageLogDto = new List<MessageLogDTO>();
        //    foreach(var message in listMessages)
        //    {
        //        var messageLogDto = new MessageLogDTO();
        //        if (message.Sent == true)
        //            messageLogDto.Sent = "Si";
        //        else
        //            messageLogDto.Sent = "No";          
        //        messageLogDto.CreatedDate = message.CreatedDate;
        //        messageLogDto.SentDate = message.SentDate;
        //        messageLogDto.Subject= message.Subject;
        //        foreach (var user in listUsers)
        //        {
        //            if (message.IdUser == user.Id)
        //                messageLogDto.User = user.Email;
        //        }
        //        listMessageLogDto.Add(messageLogDto);
        //    }
        //    IQueryable<MessageLogDTO> messagesIQueryable = listMessageLogDto.AsQueryable();
        //    return messagesIQueryable;
        //}
        public IQueryable<Message> GetIQueryable()
        {
            return _unitOfWork.MessageRepository.GetIQueryable();
        }
        public async Task<int> CountMessages ()
        {
            return await _unitOfWork.MessageRepository.CountMessages();
        }

        public async Task WriteSendError(int id, string error)
        {
            try
            {
                await _unitOfWork.MessageRepository.WriteSendError(id, error);

                await _unitOfWork.SaveChangesAsync();
            }
            catch (System.Exception)
            {
                throw;
            }
        }
    }
}
