namespace UserJourney.Repositories.Contracts
{
    using System.Threading.Tasks;

    public interface IUnitOfWork
    {
        IGenericRepository<T> GetRepository<T>() where T : class;
        Task SaveAsync();
    }
}
