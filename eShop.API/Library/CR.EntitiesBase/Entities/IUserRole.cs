using CR.EntitiesBase.Base;
using CR.EntitiesBase.interfaces;

namespace CR.EntitiesBase.Entities
{
    public interface IUserRole<TUser, TRole> : IEntity<int> , IFullAudited
    {
        TUser User { get; set; }
        TRole Role { get; set; }

    }
 }