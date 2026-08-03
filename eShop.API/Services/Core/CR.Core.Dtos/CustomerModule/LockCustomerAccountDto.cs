using System.ComponentModel.DataAnnotations;

namespace CR.Core.Dtos.CustomerModule
{
    public class LockCustomerAccountDto
    {
        [Required(ErrorMessage = "Vui lòng nhập lý do khóa/mở khóa tài khoản")]
        public string Reason { get; set; } = string.Empty;

        public bool Lock { get; set; } = true;
    }

    public class AdjustCustomerPointsDto
    {
        [Required(ErrorMessage = "Số điểm điều chỉnh không được để trống")]
        public int PointsDelta { get; set; } // +100 (cộng điểm) hoặc -50 (trừ điểm)

        [Required(ErrorMessage = "Vui lòng nhập lý do điều chỉnh điểm")]
        public string Reason { get; set; } = string.Empty;
    }
}
