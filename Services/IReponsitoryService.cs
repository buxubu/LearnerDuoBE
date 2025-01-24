using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace DatingApp.Services
{
    public interface IReponsitoryService<T> where T : class
    {
        Task Insert(T entity);
        Task Commit();
        void Delete(T entity);
        Task<T> GetById(int? id);
        void Update(T entity);
        Task<IEnumerable<T>> GetListT();
    }

    public class ReponsitoryService<T> : IReponsitoryService<T> where T : class
    {
        private readonly DbContext _dbContext;
        public ReponsitoryService(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Commit()
        {
            await _dbContext.SaveChangesAsync();
        }

        public void Delete(T entity)
        {
            EntityEntry entityEntry = _dbContext.Entry<T>(entity);
            entityEntry.State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
        }

        public async Task<T> GetById(int? id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetListT()
        {
            return await _dbContext.Set<T>().AsNoTracking().ToListAsync();
        }


        public async Task Insert(T entity)
        {
            await _dbContext.Set<T>().AddAsync(entity);
        }

        public void Update(T entity)
        {
            throw new NotImplementedException();
        }
    }
}
