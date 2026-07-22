using CR.DtoBase;

namespace CR.Core.ApplicationServices.ProductModule.Abstracts
{
    public interface IProductImageService
    {
        Task<Result> add();
    }
}