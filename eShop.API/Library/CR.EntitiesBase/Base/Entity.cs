namespace CR.EntitiesBase.Base
{
    public class Entity<Tkey> : IEntity<Tkey>
    {
        public Tkey Id { get; set; } = default!;
    }
    public interface IEntity<TKey>
    {
        TKey Id { get; set; }
    }
    public class Entity : IEntity<int>
    {
        
    }
}