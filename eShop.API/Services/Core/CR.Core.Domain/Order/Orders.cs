using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CR.Constants;
using CR.Constants.Common.Database;
using CR.Core.Domain.Address;
using CR.Core.Domain.Payment;
using CR.Core.Domain.Product;
using CR.Core.Domain.User;
using CR.EntitiesBase.interfaces;

namespace CR.Core.Domain.Order
{

    [Table(nameof(Orders), Schema = DbSchemas.CRCore)]
    public class Orders : ICreatedBy, IModifiedBy
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int UserId { get; set; }

        public int AddressId { get; set; }

        [Required]
        public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ShippingFee { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        public DateTime PlacedAt { get; set; } = DateTime.UtcNow; 

        [ForeignKey(nameof(UserId))]
        public User.Users User { get; set; } = null!;

        [ForeignKey(nameof(AddressId))]
        public Addresses Address { get; set; } = null!;

        public ICollection<OrderItems> Items { get; set; } = new List<OrderItems>();
        public ICollection<Payments> Payments { get; set; } = new List<Payments>();
        public ICollection<OrderCoupons> OrderCoupons { get; set; } = new List<OrderCoupons>();
        public DateTime? CreatedDate { get ; set ; }
        public int? CreatedBy { get ; set; }
        public DateTime? ModifiedDate { get ; set ; }
        public int? ModifiedBy { get ; set; }
    }

    
}