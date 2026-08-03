using CR.DtoBase;

namespace CR.Core.Dtos.CustomerModule
{
    public class CustomerAdminQueryDto : PagingRequestBaseDto
    {
        /// <summary> Lọc theo Trạng thái: null = Tất cả, true = Hoạt động, false = Bị khóa </summary>
        public bool? IsActive { get; set; }

        /// <summary> Lọc theo Phân nhóm: VIP, Champions, AtRisk, Regular </summary>
        public string? CustomerSegment { get; set; }

        /// <summary> Lọc theo khoảng ngày đăng ký </summary>
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        /// <summary> Lọc theo khoảng tổng chi tiêu (LTV) </summary>
        public decimal? MinSpent { get; set; }
        public decimal? MaxSpent { get; set; }
    }
}
