namespace RateLimitServices
{
    public interface IRateLimitServiceForGetProjectByRoom
    {
        bool IsLimited { get; }
    }
}
