using Cross.Helpers.Context;
using System.Linq.Expressions;

namespace Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        T FindOne(Expression<Func<T, bool>> predicate, FindOption? findOptions = null);
        List<T> Find(Expression<Func<T, bool>> predicate, FindOption? findOptions = null);
        void Add(T entity);
        void AddMany(List<T> entities);
        void Update(T entity);
        void Delete(T entity);
        void CleanContext();
    }
}
