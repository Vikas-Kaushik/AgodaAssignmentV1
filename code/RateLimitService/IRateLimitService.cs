namespace RateLimitServices
{
    public interface IRateLimitService
    {
        bool IsLimitedApi { get; }
    }
}
