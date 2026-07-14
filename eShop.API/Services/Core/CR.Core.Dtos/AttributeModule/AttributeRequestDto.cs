using System.Text.Json.Serialization;
using CR.DtoBase;

namespace CR.Core.Dtos.AttributeModule;
public class AttributeRequest : BaseRequestDto
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required AttributeTypeEnum AttributeType { get; set; }
    public bool IsFilterable { get; set; }
    public bool IsVariantDefining { get; set; } // Xác định thuộc tính này có thể tạo ra biến thể được không

}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AttributeTypeEnum
{
    Text,
    Number,
    Color,
    Boolean
}
