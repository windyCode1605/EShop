using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CR.Constants.Common.Database;
using CR.EntitiesBase;

namespace CR.Core.Domain.Address
{
    [Table(nameof(Addresses), Schema = DbSchemas.Default)]
    public class Addresses : BaseEntity
    {
        public int? UserId { get; set; }
        [Required]
        [MaxLength(200)]
        public string Street { get; set; } = null!;
        [Required]
        [MaxLength(100)]
        public string City { get; set; } = null!;
        [Required]
        [MaxLength(100)]
        public string Province { get; set; } = null!;

        [Required]
        [MaxLength(256)]
        public string? ReceiverName { get; set; }

        [Required]
        [MaxLength(40)]
        public string? ReceiverPhone { get; set; }
        [Required]
        public bool IsDefault { get; set; } = false;
        [ForeignKey(nameof(UserId))]
        public User.Users? User { get; set; } = null!;
        public ICollection<CR.Core.Domain.Orders.Order> Orders { get; set; } = new List<CR.Core.Domain.Orders.Order>();
    }
}
