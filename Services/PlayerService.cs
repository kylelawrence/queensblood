using RNG = System.Security.Cryptography.RandomNumberGenerator;

namespace queensblood;

public interface IPlayerService
{
    string EnsurePlayerId(HttpContext context);
}

public class PlayerMemService() : IPlayerService
{
    private const string PLAYER_COOKIE = "playerId";
    private static string GetNewPlayerId() => RNG.GetHexString(32);

    private readonly HashSet<string> playerIds = [];

    private string GetUniquePlayerId()
    {
        var newPlayerId = GetNewPlayerId();
        while (playerIds.Contains(newPlayerId)) newPlayerId = GetNewPlayerId();
        playerIds.Add(newPlayerId);
        return newPlayerId;
    }

    public string EnsurePlayerId(HttpContext context)
    {
        // Get existing or new player Id
        if (!context.Request.Cookies.TryGetValue(PLAYER_COOKIE, out var playerId))
        {
            playerId = GetUniquePlayerId();
            context.Response.Cookies.Append(PLAYER_COOKIE, playerId);
        }
        else if (!playerIds.Contains(playerId))
        {
            playerId = GetUniquePlayerId();
        }

        return playerId;
    }
}