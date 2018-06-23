namespace RateLimitServices
{
    public interface IRateLimitService
    {
        bool IsLimited(RateLimitedApis rateLimitedApi);
    }
}
