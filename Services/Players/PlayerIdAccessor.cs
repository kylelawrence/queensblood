namespace queensblood;

public interface IPlayerIdAccessor
{
    string GetPlayerId();
}

public class PlayerIdAccessor(IHttpContextAccessor contextAccessor, IPlayersService playersService) : IPlayerIdAccessor
{
    public string GetPlayerId()
    {
        var context = contextAccessor.HttpContext;
        if (context != null &&
            context.TryGetCookie(IPlayersService.PLAYER_COOKIE, out var playerId) &&
            playerId != null &&
            playersService.IsValidPlayer(playerId))
        {
            return playerId;
        }

        return "";
    }
}