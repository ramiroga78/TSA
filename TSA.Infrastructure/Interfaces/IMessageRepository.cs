using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TSA.Infrastructure.Entities;
using TSALibrary.Models;

namespace TSA.Infrastructure.Interfaces
{
    public interface IMessageRepository : IGenericRepository<Message>
    {
        public Task<IEnumerable<Message>> GetTopNMessages(int messagesNumber, string orderByFieldName);
        public Task MarkAsSent(int id, string email);
        public Task WriteSendError(int id, string error);
        public Task<int> CountMessages();
        public IQueryable<Message> GetIQueryable();
    }
}
