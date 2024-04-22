using Amazon.SecurityToken.Model;

namespace queensblood;

public enum GameState
{
    PickingDecks,
    Mulligan,
    Player1Turn,
    Player2Turn,
    GameOver,
}

public enum PlayerType
{
    Player1,
    Player2,
    Spectator,
    Undecided,
}

public enum GameEventType
{
    DecksPicked,
}

public class Game(string id, string player1Id)
{
    public static readonly Game None = new("", "");

    public string Id { get; } = id;

    public DateTime Created { get; } = DateTime.Now;

    public DateTime LastUpdated { get; private set; } = DateTime.Now;

    public GameState State { get; private set; } = GameState.PickingDecks;

    private readonly string player1Id = player1Id;

    public Deck Player1Deck { get; private set; } = Deck.None;

    public List<int> Player1Hand { get; } = [];

    private string player2Id = "";

    public Deck Player2Deck { get; private set; } = Deck.None;

    public List<int> Player2Hand { get; } = [];

    public event EventHandler OnGameUpdated = delegate { };

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

        if (playerId == player1Id) return PlayerType.Player1;
        if (playerId == player2Id) return PlayerType.Player2;
        if (string.IsNullOrEmpty(player2Id))
        {
            player2Id = playerId;
            return PlayerType.Player2;
        }
        return PlayerType.Spectator;
    }

    public void PickDeck(string playerId, Deck deck)
    {
        LastUpdated = DateTime.Now;

        if (playerId == player1Id && Player1Deck == Deck.None)
        {
            Player1Deck = deck;
        }
        else if (playerId == player2Id && Player2Deck == Deck.None)
        {
            Player2Deck = deck;
        }

        if (Player1Deck != Deck.None && Player2Deck != Deck.None)
        {
            State = GameState.Mulligan;
        }

        OnGameUpdated(this, EventArgs.Empty);
    }
}