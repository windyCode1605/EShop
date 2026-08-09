namespace CR.Core.Dtos.CategoryDto
{
    public class CreateCategoryDto
    {
        public int? ParentId { get; set; }
        public required string Name { get; set; }
        public string? Slug { get; set; }
    }
}