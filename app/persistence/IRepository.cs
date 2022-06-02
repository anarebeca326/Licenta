using System.Collections.Generic;

namespace persistence
{
    public interface IRepository<T, ID>
    {
        List<T> GetAll();
        T Get(ID id);
        bool Create(T entity);
        bool Delete(ID id);
        bool Update(T entity);
    }
}
