using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using TSA.Infrastructure.Data;
using TSA.Infrastructure.Entities;
using TSA.Infrastructure.Interfaces;
using TSALibrary.Models;

namespace TSA.Infrastructure.Repositories
{
    public class MessageRepository : GenericRepository<Message>, IMessageRepository
    {
        private readonly TSADbContext _context;

        public MessageRepository(TSADbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Message>> GetTopNMessages(int messagesNumber, string orderByFieldName = null)
        {
            var propertyInfo = typeof(Message).GetProperty(orderByFieldName);

            if (orderByFieldName != null && propertyInfo != null)
            {
                return await _context.Messages
                .Where(x => x.Sent == false)
                .OrderBy(x => x.CreatedDate)//propertyInfo.GetValue(x., null))
                .Take(messagesNumber)
                .ToListAsync();
            }
            else
            {
                return await _context.Messages
                .Where(x => x.Sent == false)
                .Take(messagesNumber)
                .ToListAsync();
            }
        }

        public async Task MarkAsSent(int id, string email)
        {
            Message message = await _entities.FindAsync(id);

            message.Sent = true;
            message.SentTo = email;
            message.SentDate = DateTime.Now;
            message.SentError = false;
            message.ErrorReason = "";

        }

        public async Task WriteSendError(int id, string error)
        {
            Message message = await _entities.FindAsync(id);

            message.Sent = false;
            message.SentDate = DateTime.Now;
            message.SentError = true;
            message.ErrorReason = error;
        }

        public async Task<int> CountMessages()
        {
            return await _context.Messages.CountAsync();
        }
        public IQueryable<Message> GetIQueryable()
        {
            return _context.Messages.AsQueryable();
        }
    }
}
