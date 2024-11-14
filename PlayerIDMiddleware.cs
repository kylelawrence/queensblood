namespace queensblood;

public class PlayerIDMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context, IPlayersService playersService)
    {
        playersService.EnsurePlayerId(context);
        await _next(context);
    }
}

public static class PlayerIDMiddlewareExtensions
{
    public static IApplicationBuilder UsePlayerID(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<PlayerIDMiddleware>();
    }
}