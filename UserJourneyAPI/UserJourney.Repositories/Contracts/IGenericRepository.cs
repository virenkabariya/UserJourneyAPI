namespace UserJourney.Repositories.Contracts
{
    using System.Linq.Expressions;

    public interface IGenericRepository<TEntity>
    {
        Task<IEnumerable<TEntity>> AllAsync();

        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate);

        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);

        Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate);

        Task<TEntity> GetFirstAsync(Expression<Func<TEntity, bool>> predicate);

        Task<IEnumerable<TEntity>> FindByAsync(Expression<Func<TEntity, bool>> predicate);

        Task<TEntity> CreateAsync(TEntity entity);

        Task<TEntity> UpdateAsync(TEntity entity);

        Task DeleteAsync(object id);

        Task DeleteByAsync(Expression<Func<TEntity, bool>> predicate);

        Task<TEntity> GetByIdAsync(object id);

    }
}
