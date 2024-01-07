using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace ConcurrencyRateLimiterExample;
public class FathyController : ControllerBase
{
    private HashSet<string> _hashset= new();
    [EnableRateLimiting(BookingRateLimiterPolicy.PolicyName)]
    [HttpGet("api/flights/Book")]
    public async Task<IResult> Book(int flightId, int seatNumber)
    {
        var current = new HashSet<string>(_hashset);
        await Task.Delay(10000);
        if (!current.Add($"{flightId}-{seatNumber}"))
        {
            return Results.BadRequest("Already Booked");
        }
        _hashset = current;
        return Results.Ok("Booked");
    }

}
