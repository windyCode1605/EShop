namespace CR.Core.Dtos.AttributeModule;
public class AttributeResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }


    public string AttributeType { get; set; } = null!;

    public bool IsFilterable { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsVariantDefining { get; set; }
}