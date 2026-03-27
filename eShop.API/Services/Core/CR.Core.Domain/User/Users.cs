using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CR.Constants.Common.Database;
using CR.Core.Domain.Address;

namespace CR.Core.Domain.User
{
    [Table(nameof(Users), Schema = DbSchemas.CRCore)]
    public class Users
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = null!;
        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = null!;

        [Required]
        [MaxLength(255)]
        public string Email { get; set; } = null!;

        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; } = null!;

        [MaxLength(20)]
        public string? Phone { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Addresses> Addresses { get; set; } = new List<Addresses>();
        public ICollection<Order.Orders> Orders { get; set; } = new List<Order.Orders>();
        public ICollection<Review.Reviews> Reviews { get; set; } = new List<Review.Reviews>();
        public Cart.Carts? Cart { get; set; }
    }
}