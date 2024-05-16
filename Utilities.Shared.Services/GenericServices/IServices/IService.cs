namespace Utilities.Shared.Services.GenericServices.IServices
{
    public interface IService<TEntity> :
        IWriteService<TEntity>,
        IReadService<TEntity>
        where TEntity : class, new()
    {
    }
}
