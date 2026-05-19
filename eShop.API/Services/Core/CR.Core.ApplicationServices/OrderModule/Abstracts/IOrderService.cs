// ApplicationServices/OrderModule/Abstracts/IOrderService.cs
using CR.Core.ApplicationServices.OrderModule.Dtos;
using CR.DtoBase;

namespace CR.Core.ApplicationServices.OrderModule.Abstracts;

public interface IOrderService
{
    Task<Result<OrderDto>>               CreateOrder(CreateOrderDto input);
    Task<Result<OrderDto>>               GetById(int orderId);  
    Task<Result<PageResult<OrderDto>>>   GetMyOrders(FilterOrderPagingDto input);
    Task<Result<PageResult<OrderDto>>> GetAllOrders(FilterOrderPagingDto input);  // Admin
    Task<Result>                         CancelOrder(int orderId, string? reason = null);
    Task<Result>                         UpdateOrderStatus(int orderId, string newStatus); // Admin
}