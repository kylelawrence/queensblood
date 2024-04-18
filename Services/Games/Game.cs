using Amazon.SecurityToken.Model;

namespace queensblood;

public enum GameState
{
    PickingDecks,
    Mulligan,
    PlayerATurn,
    PlayerBTurn,
    GameOver,
}

public enum PlayerType
{
    Player1,
    Player2,
    Spectator,
    Undecided,
}

public class Game(string id, string playerAId)
{
    public static readonly Game None = new("", "");

    public string Id { get; } = id;

    public DateTime Created { get; } = DateTime.Now;

    public DateTime LastUpdated { get; private set; } = DateTime.Now;

    public GameState State { get; private set; } = GameState.PickingDecks;

    private readonly string PlayerAId = playerAId;

    public Deck PlayerADeck { get; private set; } = new Deck([]);

    public List<int> PlayerAHand { get; } = [];

    private string PlayerBId = "";

    public Deck PlayerBDeck { get; private set; } = new Deck([]);

    public List<int> PlayerBHand { get; } = [];

    public bool IsActive
    {
        get
        {
            var hasId = !string.IsNullOrWhiteSpace(Id);
            var withinHour = DateTime.Now.Subtract(TimeSpan.FromHours(1)) < Created;
            var updatedRecently = DateTime.Now.Subtract(TimeSpan.FromMinutes(15)) < LastUpdated;
            var isActive = hasId && withinHour && updatedRecently;
            return isActive;
        }
    }

    public PlayerType Join(string playerId)
    {
        LastUpdated = DateTime.Now;

        if (playerId == PlayerAId) return PlayerType.Player1;
        if (string.IsNullOrEmpty(PlayerBId))
        {
            PlayerBId = playerId;
            return PlayerType.Player2;
        }
        return PlayerType.Spectator;
    }

    public void PickDeck(string playerId, Deck deck)
    {
        LastUpdated = DateTime.Now;
        
        if (playerId == PlayerAId)
        {
            PlayerADeck = deck;
        }
        else if (playerId == PlayerBId)
        {
            PlayerBDeck = deck;
        }
    }
}