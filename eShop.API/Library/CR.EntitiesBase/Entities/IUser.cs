using CR.EntitiesBase.Base;
using CR.EntitiesBase.interfaces;

namespace CR.EntitiesBase.Entities
{
    public interface IUser : IEntity<int> , IFullAudited
    {
        public string UserName { get; set; }
        public string PassWord { get; set; }
        public int Status { get; set; }
    }
}