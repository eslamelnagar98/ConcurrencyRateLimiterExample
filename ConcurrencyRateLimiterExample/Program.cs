using ConcurrencyRateLimiterExample;
using Neptunee.Swagger;
using Neptunee.Swagger.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddNeptuneeSwagger()
    .AddRateLimiter(options => options
        .AddPolicy<string, BookingRateLimiterPolicy>(BookingRateLimiterPolicy.PolicyName)
        .AddPolicy<string, BookingRateLimiterNoQueueLimitPolicy>(BookingRateLimiterNoQueueLimitPolicy.PolicyName));

var app = builder.Build();

app
    .UseRateLimiter()
    .UseNeptuneeSwagger();

var hashSet = new HashSet<string>();

async Task<IResult> Book(int flightId, int seatNumber)
{
    var current = new HashSet<string>(hashSet); 
    await Task.Delay(10000);
    if (!current.Add($"{flightId}-{seatNumber}"))
    {
        return Results.BadRequest("Already Booked");
    }
    hashSet = current;
    return Results.Ok("Booked");
}

app.MapPost("api/flights/Book", Book);
app.MapPost("api/flights/BookRateLimiting", Book).RequireRateLimiting(BookingRateLimiterPolicy.PolicyName);
app.MapPost("api/flights/BookRateLimitingNoQueue", Book).RequireRateLimiting(BookingRateLimiterNoQueueLimitPolicy.PolicyName);


app.Run();