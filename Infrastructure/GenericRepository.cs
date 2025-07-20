using Cross.Helpers.Context;
using Infrastructure.Integrations.PostgreSQL.Base;
using Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure
{
    public class GenericRepository<T>(PostgreSqlContext _dbContext) : IGenericRepository<T> where T : class
    {
        public void Add(T entity)
        {
            _dbContext.Set<T>().Add(entity);
            _dbContext.SaveChanges();
        }

        public void AddMany(List<T> entities)
        {
            _dbContext.Set<T>().AddRange(entities);
            _dbContext.SaveChanges();
        }

        public T FindOne(Expression<Func<T, bool>> predicate, FindOption? findOptions = null)
        {
            var result = Get(findOptions).FirstOrDefault(predicate);
            return result!;
        }

        public List<T> Find(Expression<Func<T, bool>> predicate, FindOption? findOptions = null)
        {
            var query = Get(findOptions).Where(predicate);
            var results = query.ToList();
            return results;
        }

        public void Update(T entity)
        {
            _dbContext.Set<T>().Update(entity);
            _dbContext.SaveChanges();
        }

        private IQueryable<T> Get(FindOption? findOptions = null)
        {
            findOptions ??= new FindOption();
            IQueryable<T> query = _dbContext.Set<T>();

            if (findOptions.IsIgnoreAutoIncludes)
                query = query.IgnoreAutoIncludes();

            if (findOptions.IsAsNoTracking)
                query = query.AsNoTracking();

            return query;
        }

        public void Delete(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            _dbContext.SaveChanges();
        }

        public void CleanContext()
        {
            _dbContext.ChangeTracker.Clear();
        }


    }
    
}
