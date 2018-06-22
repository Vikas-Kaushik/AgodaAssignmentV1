namespace RateLimitServices
{
    public interface IRateLimitServiceForGetProjectByCity
    {
        bool IsLimited{ get; }
    }
}
