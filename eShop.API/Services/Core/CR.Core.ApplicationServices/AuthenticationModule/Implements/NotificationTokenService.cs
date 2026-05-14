using CR.Core.ApplicationServices.AuthenticationModule.Abstracts;
using CR.Core.ApplicationServices.Common;
using CR.Core.Domain.AuthToken;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CR.Core.ApplicationServices.AuthenticationModule.Implements
{
    public class NotificationTokenService : CoreServiceBase, INotificationTokenService
    {
        public NotificationTokenService(
            ILogger<NotificationTokenService> logger,
            IHttpContextAccessor httpContext
        )
            : base(logger, httpContext) { }

        public void AddNotificationToken(int userId, string? fcmToken, string? apnsToken)
        {
            _logger.LogInformation(
                $"{nameof(AddNotificationToken)}: userId = {userId}, fcmToken = {fcmToken}, apnsToken = {apnsToken}"
            );
            var transaction = _dbContext.Database.BeginTransaction();
            var authTokens = _dbContext.NotificationTokens;
            authTokens.Where(x => x.UserId == userId).ExecuteDelete();
            authTokens.Add(
                new NotificationToken
                {
                    FcmToken = fcmToken,
                    UserId = userId,
                }
            );
            _dbContext.SaveChanges();
            transaction.Commit();
        }
    }
}
