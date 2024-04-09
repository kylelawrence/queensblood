namespace queensblood;

public static class PlayHandler
{
    public static RouteHandlerBuilder MapPlayRoutes(this WebApplication application)
    {
        return application.MapGet("/play", Play);
    }

    public static void Play(HttpContext context, IGamesService gamesService)
    {
                
    }
}