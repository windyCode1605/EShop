namespace CR.Core.Dtos.AttributeModule
{
    public class AttributeValueResponseDto
    {
        public int Id { get; set; }
        public int AttributeId { get; set; }
        public string Value { get; set; }
        public int DisplayOrder { get; set; }
        public string ColorHex { get; set; }
    }
}