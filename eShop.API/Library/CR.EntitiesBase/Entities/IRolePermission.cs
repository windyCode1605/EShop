using CR.EntitiesBase.Base;
using CR.EntitiesBase.interfaces;

namespace CR.EntitiesBase.Entities
{
    public interface IRolePermission<TRole> : IEntity<int> , ICreatedBy
    {
        TRole Role { get; set; }
        string PermissionKey { get; set; }     
    }
}