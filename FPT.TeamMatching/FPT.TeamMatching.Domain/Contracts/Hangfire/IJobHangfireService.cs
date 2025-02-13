namespace FPT.TeamMatching.Domain.Contracts.Hangfire;

public interface IJobHangfireService
{
    void FireAndForgetJob();
    Task ReccuringJob();
    void DelayedJob();
    void ContinuationJob();
}