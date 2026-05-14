// ApplicationServices/OrderModule/Abstracts/IOrderService.cs
using CR.Core.ApplicationServices.OrderModule.Dtos;
using CR.DtoBase;

namespace CR.Core.ApplicationServices.OrderModule.Abstracts;

public interface IOrderService
{
    Task<Result<OrderDto>>               CreateOrder(CreateOrderDto input);
    Task<Result<OrderDto>>               GetById(int orderId);
    Task<Result<PagingResult<OrderDto>>> GetMyOrders(PagingRequestBaseDto input);
    Task<Result>                         CancelOrder(int orderId, string? reason = null);
}

public class PagingRequestBaseDto
{
}

public class PagingResult<T>
{
}