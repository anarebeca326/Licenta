using System.Collections.Generic;

namespace persistence
{
    interface IRepository<T, ID>
    {
        List<T> GetAll();
        T Get(ID id);
        bool Create(T entity);
        bool Delete(ID id);
        bool Update(T entity);
    }
}
