using System.Security.Cryptography;
using System.Text;

namespace CR.Utils.Sercurity
{
   public static class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            byte[] salt = new byte[16];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            using (SHA256 sha256 = SHA256.Create())
            {
                // Kết hợp mật khẩu và salt
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] saltedPassword = new byte[passwordBytes.Length + salt.Length];
                Array.Copy(passwordBytes, 0, saltedPassword, 0, passwordBytes.Length);
                Array.Copy(salt, 0, saltedPassword, passwordBytes.Length, salt.Length);

                // Tính toán hàm băm
                byte[] hashBytes = sha256.ComputeHash(saltedPassword);

                // Kết hợp salt và hash thành một chuỗi duy nhất để lưu trữ
                byte[] saltedHash = new byte[salt.Length + hashBytes.Length];
                Array.Copy(salt, saltedHash, salt.Length);
                Array.Copy(hashBytes, 0, saltedHash, salt.Length, hashBytes.Length);

                // Chuyển đổi sang chuỗi Base64 để lưu trữ
                string hashBase64 = Convert.ToBase64String(saltedHash);
                return hashBase64;
            }
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            /// Giải mã chuỗi Base64 để lấy salt và hash đã lưu
            byte[] saltedHash = Convert.FromBase64String(hashedPassword);
            // Tách salt ra khỏi chuỗi đã lưu
            byte[] salt = new byte[16];
            Array.Copy(saltedHash, salt, salt.Length);
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] saltedPassword = new byte[passwordBytes.Length + salt.Length];
                Array.Copy(passwordBytes, 0, saltedPassword, 0, passwordBytes.Length); //  Câu lệnh này sử dụng phương thức Array.Copy để sao chép các byte của mật khẩu đã được mã hóa (passwordBytes) vào mảng saltedPassword, bắt đầu từ vị trí 0. Điều này có nghĩa là phần đầu của mảng saltedPassword sẽ chứa các byte của mật khẩu đã được mã hóa, và phần tiếp theo sẽ chứa các byte của salt. Cụ thể, nó sao chép passwordBytes.Length byte từ mảng passwordBytes vào mảng saltedPassword, bắt đầu từ vị trí 0 của saltedPassword.
                Array.Copy(salt, 0, saltedPassword, passwordBytes.Length, salt.Length); // Câu lệnh này sử dụng phương thức Array.Copy để sao chép các byte của salt vào mảng saltedPassword, bắt đầu từ vị trí passwordBytes.Length. Điều này có nghĩa là phần tiếp theo của mảng saltedPassword sẽ chứa các byte của salt, ngay sau phần chứa các byte của mật khẩu đã được mã hóa. Cụ thể, nó sao chép salt.Length byte từ mảng salt vào mảng saltedPassword, bắt đầu từ vị trí passwordBytes.Length của saltedPassword.
                byte[] hashBytes = sha256.ComputeHash(saltedPassword); // Câu lệnh này sử dụng phương thức ComputeHash của đối tượng sha256 để tính toán hàm băm của mảng saltedPassword. Kết quả là một mảng byte chứa giá trị băm đã được tính toán từ mật khẩu đã được mã hóa kết hợp với salt. Mảng hashBytes sẽ chứa giá trị băm mà chúng ta sẽ so sánh với phần hash đã lưu trong saltedHash để xác minh tính đúng đắn của mật khẩu nhập vào.

                for( int i = 0; i < hashBytes.Length; i++)
                {
                    if (hashBytes[i] != saltedHash[salt.Length + i])
                    {
                        return false; // Mật khẩu không khớp
                    }
                }
            }
            // Kiểm tra mật khẩu với băm đã lưu
            return true; // Mật khẩu khớp
        }
    }
}