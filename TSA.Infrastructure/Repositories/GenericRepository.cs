using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TSA.Infrastructure.Data;
using TSA.Infrastructure.Entities;
using TSA.Infrastructure.Interfaces;

namespace TSA.Infrastructure.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity
    {
        private readonly TSADbContext _context;
        protected readonly DbSet<TEntity> _entities;

        public GenericRepository(TSADbContext context)
        {
            _context = context;
            _entities = context.Set<TEntity>();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<TEntity> query = _entities;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return await orderBy(query).ToListAsync();
            }
            else
            {
                return await query.ToListAsync();
            }
        }

        public virtual async Task<TEntity> GetByIdAsync(object id)
        {
            return await _entities.FindAsync(id);
        }

        public virtual async Task Insert(TEntity entity)
        {
            await _entities.AddAsync(entity);
        }

        public virtual void Update(TEntity entityToUpdate)
        {
            _entities.Attach(entityToUpdate);
            _context.Entry(entityToUpdate).State = EntityState.Modified;
        }

        public virtual async Task DeleteAsync(object id)
        {
            TEntity entity = await _entities.FindAsync(id);
            _entities.Remove(entity);
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            if (_context.Entry(entityToDelete).State == EntityState.Detached)
            {
                _entities.Attach(entityToDelete);
            }

            _entities.Remove(entityToDelete);
        }
    }
}
