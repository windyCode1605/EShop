namespace CR.Core.Dtos.CustomerModule
{
    public class CustomerDetail360Dto : CustomerListItemDto
    {
        /// <summary> Giá trị đơn hàng trung bình (AOV = TotalSpent / TotalOrders) </summary>
        public decimal AverageOrderValue => TotalOrders > 0 ? TotalSpent / TotalOrders : 0;

        /// <summary> Danh sách sổ địa chỉ nhận hàng </summary>
        public List<CustomerAddressItemDto> Addresses { get; set; } = new();

        /// <summary> Danh sách các đơn hàng gần đây </summary>
        public List<CustomerRecentOrderItemDto> RecentOrders { get; set; } = new();

        /// <summary> Ghi chú nội bộ của nhân viên CSKH/Admin </summary>
        public List<CustomerAdminNoteItemDto> InternalNotes { get; set; } = new();
    }

    public class CustomerAddressItemDto
    {
        public int Id { get; set; }
        public string ReceiverName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string FullAddress { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
    }

    public class CustomerRecentOrderItemDto
    {
        public int OrderId { get; set; }
        public string OrderCode { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string OrderStatus { get; set; } = string.Empty;
    }

    public class CustomerAdminNoteItemDto
    {
        public int Id { get; set; }
        public string NoteContent { get; set; } = string.Empty;
        public string CreatedByAdmin { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
