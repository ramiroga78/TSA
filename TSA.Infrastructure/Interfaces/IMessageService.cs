using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using TSA.Infrastructure.DTOs;
using TSA.Infrastructure.Entities;
using TSALibrary.Models;

namespace TSA.Infrastructure.Interfaces
{
    public interface IMessageService
    {
        public Task<IEnumerable<Message>> GetTopNMessages(int messagesNumber, string orderByFieldName);
        public Task AddAndSave(Message message);
        public Task MarkAsSent(int id, string email);
        public Task WriteSendError(int id, string error);
        //public Task<IQueryable<MessageLogDTO>> GetIQueryable();
        public Task<int> CountMessages();
        public IQueryable<Message> GetIQueryable();
    }
}
