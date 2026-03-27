using CR.EntitiesBase.Base;
using CR.EntitiesBase.interfaces;

namespace CR.EntitiesBase.Entities
{
    public interface IRole : IEntity<int>, IFullAudited
    {
        string Name { get; set; }
        string? Description { get; set; }
    }
}