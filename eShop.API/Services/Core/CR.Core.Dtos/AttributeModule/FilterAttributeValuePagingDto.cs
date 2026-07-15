using CR.DtoBase;

namespace CR.Core.Dtos.AttributeModule
{
    public class FilterAttributeValuePagingDto : PagingRequestBaseDto
    {
        public int AttributeId { get; set; }
        public string? Value { get; set; }
        public DateTime? CreatDate { get; set; }
    }
}