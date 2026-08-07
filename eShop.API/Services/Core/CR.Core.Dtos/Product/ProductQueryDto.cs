using CR.DtoBase;

namespace CR.Core.Dtos.Product;

public class ProductQueryDto : PagingRequestBaseDto
{
    public int? CategoryId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    // Keyword is already included in PagingRequestBaseDto
}
