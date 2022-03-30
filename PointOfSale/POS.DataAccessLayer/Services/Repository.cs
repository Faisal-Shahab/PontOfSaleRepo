using Microsoft.EntityFrameworkCore;
using POS.DataAccessLayer.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.DataAccessLayer.Services
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly AppDbContext _dataContext;
        private DbSet<T> DbEntity;
        public Repository(AppDbContext dataContext)
        {
            _dataContext = dataContext;
            DbEntity = _dataContext.Set<T>();
        }

        public async Task<bool> Delete(T model)
        {
            DbEntity.Remove(model);
            return await Save();
        }

        public async Task<IQueryable<T>> GetAll()
        {
            return DbEntity;
        }

        public async Task<T> GetById(int ModelId)
        {
            return await DbEntity.FindAsync(ModelId);
        }

        public async Task<bool> Insert(T model)
        {
            await DbEntity.AddAsync(model);
            return await Save();
        }

        public async Task<bool> Update(T model)
        {
            _dataContext.Entry(model).State = EntityState.Modified;
            return await Save();
        }

        public async Task<bool> Save()
        {
            return await _dataContext.SaveChangesAsync() > 0;
        }
    }
}
