namespace UserJourney.Repositories.Concrete
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using UserJourney.Repositories.Contracts;
    using UserJourney.Repositories.EF;

    public abstract class UnitOfWork : IUnitOfWork
    {
        private DbContext _context;

        public UnitOfWork(UserJourneyContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("Context was not supplied");
            }
            _context = context;
        }

        private Dictionary<Type, object> repositories = new Dictionary<Type, object>();

        public IGenericRepository<T> GetRepository<T>() where T : class
        {
            if (!repositories.Keys.Contains(typeof(T)))
            {
                repositories.Add(typeof(T), new GenericRepository<T>(_context));
            }

            return (IGenericRepository<T>)repositories[typeof(T)];
        }

        public async Task SaveAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
    }
}
