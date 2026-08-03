namespace CR.Core.Dtos.CustomerModule
{
    public class CustomerListItemDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        /// <summary> Chữ cái đầu Avatar (VD: "MQ" cho Mai Quang) </summary>
        public string Initials { get; set; } = string.Empty;

        /// <summary> Trạng thái tài khoản: true = Active, false = Locked </summary>
        public bool IsActive { get; set; }

        /// <summary> Tổng số đơn hàng đã mua </summary>
        public int TotalOrders { get; set; }

        /// <summary> Tổng tiền đã chi tiêu (Lifetime Spent) </summary>
        public decimal TotalSpent { get; set; }

        /// <summary> Điểm thưởng tích lũy hiện tại </summary>
        // public int RewardPoints { get; set; }

        /// <summary> Phân nhóm khách hàng (Champions, At Risk, Regular) </summary>
        // public string CustomerSegment { get; set; } = "Regular";

        public DateTime? CreatedAt { get; set; }
        public DateTime? LastLoginDate { get; set; }
    }
}
