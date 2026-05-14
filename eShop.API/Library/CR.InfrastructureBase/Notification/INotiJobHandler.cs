namespace CR.InfrastructureBase.Notification
{
    public interface INotiJobHandler
    {
        Task HandleAsync(string jobName, string payload);
    }
}