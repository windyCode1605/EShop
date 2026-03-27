using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CR.Constants.Common.Database;
using CR.EntitiesBase;

namespace CR.Core.Domain.Cart
{
    [Table(nameof(Carts), Schema = DbSchemas.CRCore)]
    public class Carts : BaseEntity
    {
        [Required]
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User.Users User { get; set; } = null!;

        public ICollection<CartItems> Items { get; set; } = new List<CartItems>();
    }

    
}