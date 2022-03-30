using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.DataAccessLayer.IServices
{
    public interface IRepository<T> where T : class
    {
        Task<IQueryable<T>> GetAll();
        Task<T> GetById(int id);
        Task<bool> Insert(T model);
        Task<bool> Update(T model);
        Task<bool> Delete(T model);
        Task<bool> Save();
    }
}
