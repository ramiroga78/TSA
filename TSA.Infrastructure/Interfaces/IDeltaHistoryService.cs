using System.Threading.Tasks;
using TSALibrary.Models;
namespace TSA.Infrastructure.Interfaces
{
    public interface IDeltaHistoryService
    {
        public Task SetChanges(Delta delta);
    }
}
