namespace TalkingTails.Repository.Interfaces;

public interface IUnitOfWork
{
    IGenericRepository<T> GenericRepository<T>()
        where T : class;
    Task<int> SaveChangesAsync();

    Task BeginTransactionAsync();

    Task CommitTransactionAsync();

    Task RollbackTransactionAsync();
}
