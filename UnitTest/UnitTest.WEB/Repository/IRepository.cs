using System.Collections.Generic;
using System.Threading.Tasks;

namespace UnitTest.WEB.Repository
{
    //Moq yapısı abstraction uyumlu çalıştığı için işlemlerimizi loosecoupling hale getiriyoruz
    public interface IRepository<TEntity> where TEntity:class
    {
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity> GetByIdAsync(int id);
        Task CreateAsync(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
    }
}
