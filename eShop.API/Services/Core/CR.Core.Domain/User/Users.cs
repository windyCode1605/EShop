using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CR.Constants.Common.Database;
using CR.Constants.Core.Users;
using CR.Core.Domain.Address;
using CR.Core.Domain.AuthToken;
using CR.Core.Domain.Permission;
using CR.EntitiesBase.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using IUser = CR.EntitiesBase.Entities.IUser;


namespace CR.Core.Domain.User
{
    [Table(nameof(Users), Schema = DbSchemas.CRCore)]
    public class Users : IUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string UserName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        [Required]
        [MaxLength(255)]
        public GenderTypes? Gender { get; set; }
        /// <summary>
        ///  Mã số người dùng có thể mssv hoặc mã nv
        /// </summary>
        [Unicode(false)]
        [MaxLength(20)]
        public string? UserCode { get; set; }
        public DateTime? DateOfBirth {get; set; }
        /// <summary>
        /// Quê quán của người dùng
        /// </summary>
        [MaxLength(500)]
        public string? HomeTown { get; set; }
        /// <summary>
        ///  Số ID (CCCD) của người dùng 
        /// </summary>
        [Unicode(false)]
        [MaxLength(125)]
        public string? IdCode { get; set; }

        public ICollection<Addresses> Addresses { get; set; } = new List<Addresses>();
        public ICollection<Order.Orders> Orders { get; set; } = new List<Order.Orders>();
        public ICollection<Review.Reviews> Reviews { get; set; } = new List<Review.Reviews>();
        public Cart.Carts? Cart { get; set; }
        [Required]
        [MaxLength(128)]
        [Unicode(false)]
        public required string PassWord { get ; set ; }
        [MaxLength(256)]
        public string? FullName { get ; set ; }
        public UserTypeEnum UserType { get ; set; }
        /// <summary>
        ///  Thời gian xóa tài khoản theo status = 3 (LOCK) để có thể khôi phục lại tài khoản nếu bị khóa nhầm trong thời gian quy định
        /// </summary>
        public DateTime? LockedStatus { get; set; }
        [MaxLength(128)]
        [Unicode(false)]
        public string? PinCode { get; set; }
        
        /// <summary>
        ///  Id của người dùng được tạo ra tài khoản này, có thể là admin hoặc người dùng tự đăng ký
        /// </summary>
        public string? CustomerId { get; set; }
        
        /// <summary>
        /// Có phải mã pin tạm thời không 
        /// </summary>
        public bool IsTempPin { get; set; }
        
        /// <summary>
        /// Có phải mật khẩu tạm thời không 
        /// </summary>
        public bool IsTempPassword { get; set; }

        /// <summary>
        /// Lần đầu đăng nhập app 
        /// Mặc định là false. True khi tạo tài khoản trên Cms chọn là mật khẩu tạm. !IsPassWordTemp
        /// </summary>
        public bool IsFirstTime { get; set; }
        public string? OperatingSystem { get; set ;}
        public string? Brower { get; set; }
        public DateTime? LastLogin { get; set; }

        [MaxLength(2048)]
        public string? AvatarImageUri { get; set; }
         /// <summary>
        /// S3 Key
        /// </summary>
        [MaxLength(2024)]
        public string? S3Key { get; set; }

        /// <summary>
        /// requestId Otp
        /// </summary>
        [MaxLength(128)]
        public string? OtpRequestId { get; set; }

        /// <summary>
        /// Thời gian gửi lại Otp khi quá số lần gửi Otp
        /// </summary>
        public DateTime? ResendOtpDate { get; set; }
        public int LoginFailCount { get; set; }
        public DateTime DateTimeLoginFailCount { get; set; }
        
        /// <summary>
        /// Mã bí mật khi xác nhận quên mật khẩu
        /// </summary>
        [MaxLength(128)]
        public string? SecretPassWordCode { get; set; }
        
        /// <summary>
        /// Thời gian hết hạn mã bí mật khi xác nhận quên mật khẩu
        /// </summary>
        public DateTime? SecretPasswordExpireTime { get; set; }
        public List<UserRole> UserRoles { get; set; } = [];
        public List<CustomerUserRole> CustomerUserRoles { get; set; } = new();
        public List<NotificationToken> NotificationTokens { get; set; } = [];
        /// <summary>
        /// Id đối tượng thê cho tài khoản thuê
        /// </summary>
        public int? TenantId { get; set; }
        /// <summary>
        /// Domain đăng kí tenant
        /// </summary>
        [MaxLength(128)]
        public string? TenantDomainRegister { get; set; }

        /// <summary>
        /// Tên tenant đăng kí
        /// </summary>
        [MaxLength(128)]
        public string? TenantNameRegister { get; set; }

        /// <summary>
        /// Ngôn ngữ của tenant đăng ký 
        /// </summary>
        [MaxLength(10)]
        [Unicode(false)]
        public string? TenantLanguageRegister { get; set; }

        /// <summary>
        /// Có phải admin của khách hàng không 
        /// </summary>
        public bool IsAdminCustomer { get; set; }

        /// <summary>
        /// Thời gian khóa User (Trường hợp khi đăng nhập sai quá 5 lần sẽ khóa 1 tiếng)
        /// </summary>
        public DateTime? TimeLockUser { get; set; }
        public int Status { get ; set ; }
        public DateTime? CreatedDate { get ; set ; }
        public int? CreatedBy { get ; set ; }
        public DateTime? ModifiedDate { get ; set ; }
        public int? ModifiedBy { get ; set ; }
        public DateTime? DeletedDate { get ; set ; }
        public int? DeletedBy { get ; set ; }
        public bool Deleted { get ; set ; }
    }

}