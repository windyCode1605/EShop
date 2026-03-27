// Library/CR.DtoBase/BaseResponseDto.cs
namespace CR.DtoBase;

public abstract class BaseResponseDto
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
}