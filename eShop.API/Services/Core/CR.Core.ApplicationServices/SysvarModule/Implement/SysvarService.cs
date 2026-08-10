using AutoMapper;
using CR.ApplicationBase;
using CR.Constants.ErrorCodes;
using CR.Core.ApplicationServices.SysvarModule.Abstracts;
using CR.Core.Dto.SysvarModule;
using CR.DtoBase;
using CR.Utils.DataUtils;
using Microsoft.EntityFrameworkCore;
using CR.Constants.Common.SysVar;

namespace CR.Core.ApplicationServices.SysvarModule.Implements
{
    public class SysVarService : ServiceBase<CoreDbContext>, ISysvarService
    {
        private static readonly Dictionary<string, Type> SysVarRegistry = new()
        {
            { VarNames.LOGINMAXTURN, typeof(int) },
            { VarNames.OTP_MAX_TURN, typeof(int) },
            { VarNames.SECOND, typeof(int) },
            { VarNames.OTP_LENGTH, typeof(int) },
            { VarNames.OTP_RESEND_COOLDOWN, typeof(int) },
            { VarNames.API_KEY, typeof(string) },
            { VarNames.DEFAULT_OTP, typeof(string) }
        };

        public SysVarService(
            CoreDbContext dbContext,
            ILogger<SysVarService> logger,
            IMapper mapper,
            IHttpContextAccessor httpContext)
        : base(dbContext, logger, mapper, httpContext) { }

        public async Task<Result<List<SysvarResponsDto>>> GetSysvarsAsync()
        {
            _logger.LogInformation("Method {method}", nameof(GetSysvarsAsync));
            var sysVars = await _dbContext.SysVars
                .AsNoTracking()
                .ToListAsync();

            return Result<List<SysvarResponsDto>>.Success(_mapper.Map<List<SysvarResponsDto>>(sysVars));
        }

        public async Task<Result> UpdateSysVarAsync(int id, SysvarUpdateDto dto)
        {
            _logger.LogInformation("Method {method}, Id={id}", nameof(UpdateSysVarAsync), id);

            var sys = await _dbContext.SysVars
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sys == null)
            {
                return Result.Failure(ErrorCode.SysVarNotFound, this.GetCurrentMethodInfo(), $"Không tìm thấy cấu hình hệ thống với ID: {id}");
            }

            var newValue = dto.VarValue ?? string.Empty;

            // Validate based on the expected type
            if (SysVarRegistry.TryGetValue(sys.VarName, out var expectedType))
            {
                if (expectedType == typeof(int) && !int.TryParse(newValue, out _))
                {
                    return Result.Failure(ErrorCode.BadRequest, this.GetCurrentMethodInfo(), $"Giá trị của cấu hình {sys.VarName} phải là một số nguyên hợp lệ.");
                }
                if (expectedType == typeof(bool) && !bool.TryParse(newValue, out _))
                {
                    return Result.Failure(ErrorCode.BadRequest, this.GetCurrentMethodInfo(), $"Giá trị của cấu hình {sys.VarName} phải là True hoặc False.");
                }
            }

            sys.VarValue = newValue;

            _dbContext.SysVars.Update(sys);
            await _dbContext.SaveChangesAsync();

            return Result.Success();
        }
    }
}