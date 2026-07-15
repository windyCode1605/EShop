namespace CR.Core.Dtos.AttributeModule
{
    public class AttributeValueRequestDto
    {
        public required int AttributeId { get; set; }
        public required string Value { get; set; }
        public required int DisplayOrder { get; set; }
        public string ColorHex { get; set; }
    }
}