namespace queensblood;

public static class PlayHandler
{
    public static void MapPlayRoutes(this WebApplication application)
    {
        application.MapPost("/play", Play);
        application.MapGet("/play", BaseHandlers.GoHome);
    }

    public static void Play(HttpContext context, IPlayerService playerService, IGamesService gamesService)
    {
        if (context.NeedsToGoHome()) return;

        var playerId = playerService.EnsurePlayerId(context);
        if (gamesService.TryCreateGame(playerId, out var game))
        {
            
        }
    }
}