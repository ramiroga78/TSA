using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TSA.Infrastructure.Entities;

namespace TSA.Infrastructure.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : BaseEntity
    {
        Task<IEnumerable<TEntity>> GetAllAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "");
        Task<TEntity> GetByIdAsync(object id);
        Task Insert(TEntity entity);
        void Update(TEntity entityToUpdate);
        Task DeleteAsync(object id);
        void Delete(TEntity entityToDelete);
    }
}