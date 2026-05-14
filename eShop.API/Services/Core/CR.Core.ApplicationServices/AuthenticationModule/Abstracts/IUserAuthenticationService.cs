using CR.Core.Domain.User;

namespace CR.Core.ApplicationServices.AuthenticationModule.Abstracts
{
    /// <summary>
    /// Dịch vụ xác thực người dùng, bao gồm các chức năng liên quan đến đăng
    /// nhập, đăng ký, quản lý tài khoản người dùng, và các hoạt động liên quan đến bảo mật khác.
    /// </summary>
    public interface IUserAuthenticationService
    {
        /// <summary>
        /// Validate user admin 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<Users> ValidateWebUser(string userName, string password);
        /// <summary>
        /// xác thực người dùng khi đăng nhập app ( có thể là customer hoặc admin)
        /// </summary>
        Task<Users> ValidateAppUser(string userName, string password);


        
        /// <summary>
        /// Tìm kiếm user theo id ( dùng khi goi authentication)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Users> FindUserAuthorizationById(int id);



    }
}