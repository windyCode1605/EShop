using CR.Constants.ErrorCodes;
using CR.EntitiesBase.Base;
using Microsoft.AspNetCore.Http;

namespace CR.ApplicationBase.Localization;

// Concrete mapper used by DI to resolve IMapErrorCode.
public sealed class MapErrorCode : MapErrorCodeBase<MapErrorCode.ErrorCodes>
{
    public MapErrorCode(ILocalization localization, IHttpContextAccessor httpContext)
        : base(localization, httpContext)
    {
    }

    // Bridge type to satisfy MapErrorCodeBase<TErrorCode> where TErrorCode : IErrorCode.
    public sealed class ErrorCodes : IErrorCode
    {
        public const int BadRequest = ErrorCode.BadRequest;
        public const int UsernameOrPasswordIncorrect = ErrorCode.UsernameOrPasswordIncorrect;
        public const int UserNotFound = ErrorCode.UserNotFound;
        public const int UserIsDeactive = ErrorCode.UserIsDeactive;
        public const int InvalidUserType = ErrorCode.InvalidUserType;
        public const int UserNotHavePermission = ErrorCode.UserNotHavePermission;
        public const int UserStatusIsInvalid = ErrorCode.UserStatusIsInvalid;
        public const int UserIsVerify = ErrorCode.UserIsVerify;
        public const int UserIsRequestVerify = ErrorCode.UserIsRequestVerify;
        public const int UserIsRegistered = ErrorCode.UserIsRegistered;
        public const int OptCodeNotValid = ErrorCode.OptCodeNotValid;
        public const int OptCodeIsExpired = ErrorCode.OptCodeIsExpired;
        public const int TokenIsInvalid = ErrorCode.TokenIsInvalid;
        public const int UserRegisterExistPersonalEmail = ErrorCode.UserRegisterExistPersonalEmail;
        public const int AppPasswordIncorrect = ErrorCode.AppPasswordIncorrect;
        public const int UserLoginUserTypeInvalid = ErrorCode.UserLoginUserTypeInvalid;
        public const int UserIsLock = ErrorCode.UserIsLock;
        public const int UserCurrentPasswordIncorrect = ErrorCode.UserCurrentPasswordIncorrect;
        public const int UserIsInactiveBecauseMultiLoginTime = ErrorCode.UserIsInactiveBecauseMultiLoginTime;
        public const int SysVarsIsNotConfig = ErrorCode.SysVarsIsNotConfig;
    }
}
