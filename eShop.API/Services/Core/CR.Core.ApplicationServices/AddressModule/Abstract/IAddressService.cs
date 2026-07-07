using CR.Core.Dtos.AddressModuleDto;
using CR.DtoBase;


namespace CR.Core.ApplicationServices.AddressModule.Abstracts;
public interface IAddressService
{
    Task<Result<List<AddressResponseDto>>> GetAddressesByUserIdAsync(CancellationToken cancellationToken = default);
    Task<Result<int>> SaveNewAddressAsync(SaveAddressRequestDto dto, CancellationToken cancellationToken = default);
    Task<Result> SetAsDefaultAsync(int addressId, CancellationToken cancellationToken = default);

}