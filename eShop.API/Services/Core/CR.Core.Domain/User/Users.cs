using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CR.Constants.Common.Database;
using CR.Constants.Core.Users;
using CR.Core.Domain.AuthToken;
using Microsoft.EntityFrameworkCore;

namespace CR.Core.Domain.User
{
    [Table(nameof(Users), Schema = DbSchemas.Default)]
    [Index(nameof(Email), IsUnique = true)]
    [Index(nameof(Username), IsUnique = true)]
    public class Users 
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // --- ACCOUNT INFO ---
        [Required, MaxLength(128), Unicode(false)]
        public string Username { get; set; } = null!;
        
        [Required, MaxLength(128), Unicode(false)]
        public string Email { get; set; } = null!;

        [MaxLength(20), Unicode(false)]
        public string? Phone { get; set; }

        // --- SECURITY ---
        [Required, MaxLength(256), Unicode(false)]
        public string PasswordHash { get; set; } = null!;
        public bool IsTempPassword { get; set; }
        
        [MaxLength(128), Unicode(false)]
        public string? PinCode { get; set; }
        public bool IsTempPin { get; set; }

        public UserTypeEnum UserType { get; set; }
        public int Status { get; set; }
        public bool IsFirstTime { get; set; }
        
        public int LoginFailCount { get; set; }
        public DateTime? DateTimeLoginFailCount { get; set; }
        public DateTime? TimeLockUser { get; set; }
        
        public DateTime? LastLogin { get; set; }

        // --- NAVIGATION PROPERTIES ---
        // LIÊN KẾT 1-1 SANG BẢNG PROFILE
        public virtual UserProfile Profile { get; set; } = null!; 

        public List<UserRole> UserRoles { get; set; } = new();
        public List<NotificationToken> NotificationTokens { get; set; } = new();
        public virtual CR.Core.Domain.Carts.Cart? Cart { get; set; }
        public virtual ICollection<CR.Core.Domain.Orders.Order> Orders { get; set; } = new List<CR.Core.Domain.Orders.Order>();
        public virtual ICollection<CR.Core.Domain.Opts.AuthOtp> AuthOtps { get; set; } = new List<CR.Core.Domain.Opts.AuthOtp>();
        public virtual ICollection<CR.Core.Domain.Address.Addresses> Addresses { get; set; } = new List<CR.Core.Domain.Address.Addresses>();
        public virtual ICollection<CR.Core.Domain.Review.Reviews> Reviews { get; set; } = new List<CR.Core.Domain.Review.Reviews>();

        // --- AUDIT & CONCURRENCY ---
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public int? DeletedBy { get; set; }
        public bool Deleted { get; set; }
        
        [Timestamp]
        public byte[] RowVersion { get; set; } = null!;
    }
}