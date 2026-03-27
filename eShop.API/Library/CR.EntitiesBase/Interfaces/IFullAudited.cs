// FIle này định nghĩa một interface IFullAudited, kế thừa từ IAudited, để thêm các trường CreatedBy và LastModifiedBy.
// IFullAudited thường được dùng cho các entity cần lưu thông tin về người tạo và người sửa cuối cùng, bên cạnh các trường CreatedAt và LastModifiedAt đã có trong IAudited.
using System;
namespace CR.EntitiesBase.interfaces 
{
    public interface IFullAudited : ICreatedBy, IModifiedBy, ISoftDeleted { }
    public interface ICreatedBy
    {
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
    }
    public interface IModifiedBy
    {
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
    }
    public interface ISoftDeleted
    {
        public DateTime? DeletedDate { get; set; }
        public int? DeletedBy { get; set; }
        public bool Deleted { get; set; }
    }

}