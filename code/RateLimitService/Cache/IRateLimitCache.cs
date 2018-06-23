namespace RateLimitServices
{
    public interface IRateLimitCache
    {
        // Read-Only Items        
        uint SlotSpanInSeconds { get; }
        uint[] RequestCounts { get; }
        uint MaxRequestsInSlotSpan { get; }
        uint TimeSpanToKeepBlockedInSeconds { get; }

        // Mutable Items
        uint TimeOfLastApiCall { get; set; }
        uint TotalRequestsInSlotSpan { get; set; }        
        ushort PastIndex { get; set; }        
        uint LastTimeWhenThresholdCrossed { get; set; }
    }
}