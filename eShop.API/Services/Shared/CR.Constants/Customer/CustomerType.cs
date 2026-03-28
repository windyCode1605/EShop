namespace CR.Constants.Customer;

public enum CustomerTypes
{
    /// <summary>
    /// Đại lý bán lẻ
    /// </summary>
    Agency = 0,

    /// <summary>
    /// Nhà hàng đơn lẻ
    /// </summary>
    SingleRestaurant = 1,

    /// <summary>
    /// Doanh nghiệp sở hữu chuỗi nhà hàng (Cha)
    /// </summary>
    ChainEnterprise = 2,

    /// <summary>
    /// Nhà hàng trong chuỗi thuộc sở hữu của doanh nghiệp (Con)
    /// </summary>
    ChainBranch = 3,
}
