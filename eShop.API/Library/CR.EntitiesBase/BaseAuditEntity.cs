// Dùng cho entity cân track người tạo/ sửa
namespace CR.EntitiesBase;
public abstract class BaseAuditEntity : BaseEntity
{
    public int? CreatedBy { get; set; }
    public int? UpdatedBy { get; set; } 
}