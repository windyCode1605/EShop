using System.Runtime.InteropServices;
using AutoMapper;
using CR.ApplicationBase;
using CR.ApplicationBase.Common;
using CR.Constants.ErrorCodes;
using CR.Core.ApplicationServices.AttributeModule.Abstract;
using CR.Core.ApplicationServices.Common;
using CR.Core.Domain.Catalog;
using CR.Core.Dtos.AttributeModule;
using CR.DtoBase;
using CR.Utils.DataUtils;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using StackExchange.Redis;

namespace CR.Core.ApplicationServices.AttributeModule.Implements
{
    public class AttributeValueService : ServiceBase<CoreDbContext>, IAttributeValueService
    {
        public AttributeValueService(
        CoreDbContext dbContext,
        ILogger<AttributeService> logger,
        IMapper mapper)
        : base(dbContext, logger, mapper)
        {
        }


        public async Task<Result<PageResult<AttributeValueResponseDto>>> GetValuesByAttributeIdAsync(FilterAttributeValuePagingDto input)
        {
            _logger.LogInformation("Method Name : {method}", nameof(GetValuesByAttributeIdAsync));
            var baseQuery = _dbContext.AttributeValues
            .Where(av => !av.Deleted);

            if (input.AttributeId >= 0)
            {
                baseQuery = baseQuery.Where(av => av.AttributeId == input.AttributeId);
            }
            var totalItems = await baseQuery.CountAsync();
            var AttributeValues = await baseQuery
            .Include(av => av.Attribute)
            .OrderByDescending(av => av.Value)
            .Paging(input)
            .ToListAsync();

            var itemsDto = AttributeValues.Select(av => new AttributeValueResponseDto
            {
                Id = av.Id,
                AttributeId = av.AttributeId,
                Value = av.Value,
                DisplayOrder = av.DisplayOrder,
                ColorHex = av.ColorHex,
            }).ToList();
            return Result<PageResult<AttributeResponseDto>>.Success(new PageResult<AttributeValueResponseDto>
            {
                TotalItems = totalItems,
                Items = itemsDto
            });
        }

        public async Task<Result<AttributeValueResponseDto>> CreateAsync(AttributeValueRequestDto input)
        {
            try
            {
                var existsAttribute = await _dbContext.Attributes.AnyAsync(a => a.Id == input.AttributeId && !a.Deleted);
                if (!existsAttribute)
                    return Result<AttributeValueResponseDto>.Failure(ErrorCode.InvalidInput, this.GetCurrentMethodInfo(), "Attribute not exists");
                var exists = await _dbContext.AttributeValues.AnyAsync(av => av.AttributeId == input.AttributeId
                && av.Value.ToLower() == input.Value.ToLower()
                && !av.Deleted);
                if (exists)
                    return Result<AttributeValueResponseDto>.Failure(ErrorCode.InvalidInput, this.GetCurrentMethodInfo(), "Attribute value already exists");
                var entity = _mapper.Map<CR.Core.Domain.Catalog.AttributeValue>(input);

                _dbContext.AttributeValues.Add(entity);
                await _dbContext.SaveChangesAsync();

                var resultDto = _mapper.Map<AttributeValueResponseDto>(entity);
                return Result<AttributeValueResponseDto>.Success(resultDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating attribute: {Value}", input.Value);
                return Result<AttributeValueResponseDto>.Failure(ErrorCode.UnknownError, this.GetCurrentMethodInfo(), ex.Message);
            }
        }
    }
}