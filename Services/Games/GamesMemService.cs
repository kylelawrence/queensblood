using RNG = System.Security.Cryptography.RandomNumberGenerator;

namespace queensblood;

public class GamesMemService : IGamesService, IDisposable
{
    private static string GetNewId() => RNG.GetHexString(Values.GAME_ID_LENGTH);

    private readonly Dictionary<string, Game> gamesById = [];
    private readonly Dictionary<string, Game> gamesByPlayerId = [];
    private readonly Timer? cleanUpTimer = null;

    public GamesMemService()
    {
        cleanUpTimer = new Timer(
            CleanUpGames,
            null,
            TimeSpan.FromMinutes(Values.CLEANUP_INTERVAL),
            TimeSpan.FromMinutes(Values.CLEANUP_INTERVAL)
        );
    }

    private void CleanUpGames(object? state)
    {
        var keys = gamesByPlayerId.Keys;
        foreach (var key in keys)
        {
            var game = gamesByPlayerId[key];
            if (!game.IsActive)
            {
                gamesByPlayerId.Remove(key);
                gamesById.Remove(game.Id);
            }
        }
    }

    public bool TryCreateGame(string playerId, out Game game)
    {
        // No rapid starting of games, has to finish or forfeit previous game
        if (gamesByPlayerId.TryGetValue(playerId, out var foundGame) && foundGame.IsActive)
        {
            game = foundGame;
            return false;
        }

        var id = GetNewId();
        while (gamesById.ContainsKey(id)) id = GetNewId();

        game = new Game(id, playerId);

        gamesById[id] = game;
        gamesByPlayerId[playerId] = game;

        return true;
    }

    public Game FindGameByPlayerId(string playerId)
    {
        return gamesByPlayerId.TryGetValue(playerId, out var game) ? game : Game.None;
    }

    public Game FindGameById(string gameId)
    {
        return gamesById.TryGetValue(gameId, out var game) ? game : Game.None;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        cleanUpTimer?.Dispose();
    }
}