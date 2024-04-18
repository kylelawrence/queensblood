namespace queensblood;

public interface IGamesService
{
    bool TryCreateGame(string playerId, out Game Game);
    Game FindGameByPlayerId(string playerId);
    Game FindGameById(string gameId);
}
