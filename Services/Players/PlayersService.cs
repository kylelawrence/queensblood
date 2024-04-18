using RNG = System.Security.Cryptography.RandomNumberGenerator;

namespace queensblood;

public interface IPlayersService
{
    public const string PLAYER_COOKIE = "playerId";
    void EnsurePlayerId(HttpContext context);
    bool IsValidPlayer(string id);
}

public class PlayersMemService() : IPlayersService
{
    private readonly HashSet<string> playerIdCache = [];

    private static string GetNewPlayerId() => RNG.GetHexString(32);

    private string GetUniquePlayerId()
    {
        // Generate new IDs until we get one that isn't in the cache
        var newPlayerId = GetNewPlayerId();
        while (playerIdCache.Contains(newPlayerId)) newPlayerId = GetNewPlayerId();
        return newPlayerId;
    }

    public void EnsurePlayerId(HttpContext context)
    {
        // If we don't have a player ID cookie, or if that ID doesn't exist in the cached IDs
        // Get a new ID
        var foundCookie = context.TryGetCookie(IPlayersService.PLAYER_COOKIE, out var playerId);
        if (!foundCookie || playerId == null || !playerIdCache.Contains(playerId))
        {
            playerId = GetUniquePlayerId();
        }

        // Ensure the ID is saved in the cookie and cache
        context.SetCookie(IPlayersService.PLAYER_COOKIE, playerId);
        playerIdCache.Add(playerId);
    }

    public bool IsValidPlayer(string id)
    {
        return playerIdCache.Contains(id);
    }
}