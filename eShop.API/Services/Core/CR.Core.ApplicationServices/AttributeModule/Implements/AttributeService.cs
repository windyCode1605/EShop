using AutoMapper;
using CR.ApplicationBase;
using CR.Common;
using CR.Constants.ErrorCodes;
using CR.Core.ApplicationServices.AttributeModule.Abstract;
using CR.Core.Domain.Catalog;
using CR.Core.Dtos.AttributeModule;
using CR.DtoBase;
using CR.Utils.DataUtils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CR.Core.ApplicationServices.AttributeModule.Implements;

public class AttributeService : ServiceBase<CoreDbContext>, IAttributeService
{
    public AttributeService(
        CoreDbContext dbContext,
        ILogger<AttributeService> logger,
        IMapper mapper)
        : base(dbContext, logger, mapper)
    {
    }

    public async Task<Result<AttributeResponseDto>> CreateAsync(AttributeRequest dto)
    {
        try
        {
            var exists = await _dbContext.Attributes
                .AnyAsync(a => a.Name.ToLower() == dto.Name.ToLower() && !a.Deleted);

            if (exists)
            {
                _logger.LogWarning("CreateAttribute: Attribute with name {Name} already exists.", dto.Name);
                return Result<AttributeResponseDto>.Failure(ErrorCode.InvalidInput, this.GetCurrentMethodInfo(), "Attribute name already exists");
            }

            var entity = _mapper.Map<CR.Core.Domain.Catalog.Attribute>(dto);
            
            _dbContext.Attributes.Add(entity);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Attribute created successfully: Id={Id}, Name={Name}", entity.Id, entity.Name);

            var responseDto = _mapper.Map<AttributeResponseDto>(entity);
            return Result<AttributeResponseDto>.Success(responseDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating attribute: {Name}", dto.Name);
            return Result<AttributeResponseDto>.Failure(ErrorCode.UnknownError, this.GetCurrentMethodInfo(), ex.Message);
        }
    }

    public async Task<Result<AttributeResponseDto>> GetByIdAsync(int id)
    {
        var entity = await _dbContext.Attributes
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id && !a.Deleted);

        if (entity == null)
        {
            return Result<AttributeResponseDto>.Failure(ErrorCode.InvalidInput, this.GetCurrentMethodInfo(), "Attribute not found");
        }

        return Result<AttributeResponseDto>.Success(_mapper.Map<AttributeResponseDto>(entity));
    }

    public async Task<PaginatedResult<AttributeResponseDto>> GetAllAsync(int page, int size)
    {
        _logger.LogInformation("Method Name: {Method}, Page: {Page}, Size: {Size}", nameof(GetAllAsync), page, size);
        
        var query = _dbContext.Attributes
            .Where(a => !a.Deleted)
            .OrderByDescending(a => a.CreatedDate);

        var total = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * size)
            .Take(size)
            .AsNoTracking()
            .ToListAsync();

        return new PaginatedResult<AttributeResponseDto>
        {
            Items = _mapper.Map<List<AttributeResponseDto>>(items),
            TotalCount = total,
            Page = page,
            PageSize = size
        };
    }

    public async Task<Result<AttributeResponseDto>> UpdateAsync(int id, AttributeRequest dto)
    {
        try
        {
            var entity = await _dbContext.Attributes.FirstOrDefaultAsync(a => a.Id == id && !a.Deleted);
            if (entity == null)
            {
                return Result<AttributeResponseDto>.Failure(ErrorCode.InvalidInput, this.GetCurrentMethodInfo(), "Attribute not found");
            }

            var nameExists = await _dbContext.Attributes
                .AnyAsync(a => a.Id != id && a.Name.ToLower() == dto.Name.ToLower() && !a.Deleted);
            
            if (nameExists)
            {
                return Result<AttributeResponseDto>.Failure(ErrorCode.InvalidInput, this.GetCurrentMethodInfo(), "Attribute name already exists");
            }

            _mapper.Map(dto, entity);
            
            _dbContext.Attributes.Update(entity);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Attribute updated successfully: Id={Id}", entity.Id);

            return Result<AttributeResponseDto>.Success(_mapper.Map<AttributeResponseDto>(entity));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating attribute: {Id}", id);
            return Result<AttributeResponseDto>.Failure(ErrorCode.UnknownError, this.GetCurrentMethodInfo(), ex.Message);
        }
    }

    public async Task<Result> DeleteAsync(int id)
    {
        try
        {
            var entity = await _dbContext.Attributes.FirstOrDefaultAsync(a => a.Id == id && !a.Deleted);
            if (entity == null)
            {
                return Result.Failure(ErrorCode.InvalidInput, this.GetCurrentMethodInfo(), "Attribute not found");
            }

            entity.Deleted = true;
            _dbContext.Attributes.Update(entity);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Attribute deleted successfully: Id={Id}", id);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting attribute: {Id}", id);
            return Result.Failure(ErrorCode.UnknownError, this.GetCurrentMethodInfo(), ex.Message);
        }
    }
}
