using RNG = System.Security.Cryptography.RandomNumberGenerator;

namespace queensblood;

public class Game(string id)
{
    public static readonly Game None = new("");

    public string Id { get; } = id;

    public string PlayerAId { get; set; } = "";

    public bool IsActive
    {
        get
        {
            return !string.IsNullOrWhiteSpace(Id);
        }
    }
}

public interface IGamesService
{
    bool TryCreateGame(string playerId, out Game Game);
    Game FindGameByPlayerId(string playerId);
}

public class GamesMemService : IGamesService
{
    private static string GetNewId() => RNG.GetHexString(4);

    private readonly Dictionary<string, Game> gamesById = [];
    private readonly Dictionary<string, Game> gamesByPlayerId = [];

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

        game = new Game(id)
        {
            PlayerAId = playerId
        };

        gamesById[id] = game;
        gamesByPlayerId[playerId] = game;

        return true;
    }

    public Game FindGameByPlayerId(string playerId)
    {
        return gamesByPlayerId.TryGetValue(playerId, out var game) ? game : Game.None;
    }
}