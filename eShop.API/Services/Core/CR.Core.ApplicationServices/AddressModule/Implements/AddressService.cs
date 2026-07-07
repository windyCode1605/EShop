using CR.Core.ApplicationServices.AddressModule.Abstracts;
using CR.Core.ApplicationServices.Common;
using CR.Core.Domain.Address;
using CR.Core.Dtos.AddressModuleDto;
using CR.DtoBase;
using CR.InfrastructureBase;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto.Engines;


namespace CR.Core.ApplicationServices.AddressModule.Implements;
public class AddressService : CoreServiceBase, IAddressService
{
    public AddressService(ILogger<AddressService> logger, IHttpContextAccessor httpContext)
        : base(logger, httpContext)
    {
    }

    public async Task<Result<List<AddressResponseDto>>> GetAddressesByUserIdAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("${method} called", nameof(GetAddressesByUserIdAsync));
        var userId = _httpContext.GetCurrentUserId();

        var addresses = await _dbContext.Addresses
            .AsNoTracking()
            .Where(a => a.UserId == userId && !a.IsDeleted)
            .OrderByDescending(a => a.IsDefault)
            .ThenByDescending(a => a.CreatedAt)
            .Select(a => new AddressResponseDto
            {
                Id = a.Id,
                Street = a.Street,
                City = a.City,
                Province = a.Province,
                ReceiverName = a.ReceiverName ?? string.Empty,
                ReceiverPhone = a.ReceiverPhone ?? string.Empty,
                IsDefault = a.IsDefault,
            })
            .ToListAsync(cancellationToken);
        return Result<List<AddressResponseDto>>.Success(addresses);
    }

    public async Task<Result<int>> SaveNewAddressAsync(SaveAddressRequestDto dto, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{method} is called", nameof(SaveNewAddressAsync));
        var userId = _httpContext.GetCurrentUserId();
        var strategy = _dbContext.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            // nếu đây là default mới -> unset default cũ trước khi insert
            if (dto.IsDefault)
            {
                await _dbContext.Addresses
                    .Where(a => a.UserId == userId && !a.IsDeleted)
                    .ExecuteUpdateAsync(
                        setters => setters.SetProperty(a => a.IsDefault, false),
                        cancellationToken
                    );
            }
            var NewAddress = new Addresses
            {
                UserId = userId,
                Street = dto.Street,
                City = dto.City,
                Province = dto.Province,
                ReceiverName = dto.ReceiverName,
                ReceiverPhone = dto.ReceiverPhone,
                IsDefault = dto.IsDefault,
                CreatedAt = DateTime.Now
            };
            _dbContext.Addresses.Add(NewAddress);

            try
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogWarning(ex, "Lỗi cập nhật DB khi set address cho user {userId}", userId);
                throw new Exception("Có lỗi xảy ra khi lưu địa chỉ, vui lòng thử lại.", ex);
            }
            return Result<int>.Success(NewAddress.Id);
        });

    }

    public async Task<Result> SetAsDefaultAsync(int addressId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{Method} called for address {addressId}", nameof(SetAsDefaultAsync), addressId);
        var userId = _httpContext.GetCurrentUserId();
        var strategy = _dbContext.Database.CreateExecutionStrategy();

        // Thêm return ở đây
        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            var address = await _dbContext.Addresses
                .Where(a => a.Id == addressId && a.UserId == userId && !a.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);

            if (address == null)
            {
                // Sửa thành Exception hợp lý hoặc Result.Failure
                throw new ArgumentException("Địa chỉ không tồn tại hoặc không thuộc về bạn.");
            }

            if (address.IsDefault)
            {
                await transaction.CommitAsync(cancellationToken);
                return Result.Success(); // Thêm giá trị trả về
            }

            await _dbContext.Addresses
                .Where(a => a.UserId == userId && a.IsDefault && !a.IsDeleted)
                .ExecuteUpdateAsync(
                    setters => setters.SetProperty(a => a.IsDefault, false),
                    cancellationToken
                );

            address.IsDefault = true;

            try
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken); // Dùng biến transaction để commit
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync(cancellationToken); // Đã sửa Rollback
                _logger.LogWarning(ex, "Race condition khi set default address {addressId}", addressId);
                throw new Exception("Có xung đột khi đặt địa chỉ mặc định, vui lòng thử lại.");
            }

            return Result.Success(); // Thêm giá trị trả về
        });
    }

    private static bool IsDefaultUniqueViolation(DbUpdateException ex)
    => ex.InnerException is SqlException sqlEx && (sqlEx.Number == 2601 || sqlEx.Number == 2627);
}