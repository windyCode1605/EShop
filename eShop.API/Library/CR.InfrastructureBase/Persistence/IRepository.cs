using CR.DtoBase;
namespace CR.InfrastructureBase.Persistence
{
    public interface IRepository<TEntity, TFilter>
        where TEntity : class
        where TFilter : class
    {
        
    }
}