using API.Contracts;
using API.Utilities.Handler;
using API.Data;

namespace API.Repositories
{
    public class GeneralRepository<TEntity> : IGeneralRepository<TEntity> where TEntity : class
    {
        protected readonly SMDbContext _context;

        public GeneralRepository(SMDbContext context)
        {
            _context = context;
        }


        public IEnumerable<TEntity> GetAll()
        {
            return _context.Set<TEntity>().ToList();
        }


        public TEntity? GetByGuid(Guid guid)
        {

            var entity = _context.Set<TEntity>().Find(guid);

            _context.ChangeTracker.Clear();

            return entity;
        }

        public TEntity? Create(TEntity entity)
        {
            try
            {
                _context.Set<TEntity>().Add(entity);
                _context.SaveChanges();
                return entity;
            }
            catch (Exception ex)
            {
                if (ex.InnerException is not null && ex.InnerException.Message.Contains("IX_tb_m_employees_email"))
                {
                    throw new ExceptionHandler("Email already exists");
                }
                if (ex.InnerException != null && ex.InnerException.Message.Contains("IX_tb_m_employees_phone_number"))
                {
                    throw new ExceptionHandler("Phone number already exists");
                }
                throw new ExceptionHandler(ex.InnerException?.Message ?? ex.Message);
            }
        }


    
        public bool Update(TEntity entity)
        {
            try
            {
                _context.Set<TEntity>().Update(entity); 
                _context.SaveChanges();  
                return true;
            }
            catch (Exception ex)
            {
                if (ex.InnerException is not null && ex.InnerException.Message.Contains("IX_tb_m_employees_email"))
                {
                    throw new ExceptionHandler("Email sudah ada");
                }
                if (ex.InnerException != null && ex.InnerException.Message.Contains("IX_tb_m_employees_phone_number"))
                {
                    throw new ExceptionHandler("Phone number already exists");
                }
                throw new ExceptionHandler(ex.InnerException?.Message ?? ex.Message);
            }
        }


        public bool Delete(TEntity entity)
        {
            try
            {
                _context.Set<TEntity>().Remove(entity);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
