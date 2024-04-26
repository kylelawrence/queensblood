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

    public event EventHandler OnGameUpdated = delegate { };

    public string Id { get; } = id;

    public DateTime Created { get; } = DateTime.Now;

    public DateTime LastUpdated { get; private set; } = DateTime.Now;

    public GameState State { get; private set; } = GameState.PickingDecks;

    private readonly string player1Id = player1Id;
    private string player2Id = "";

    public bool Player1IsReady => player1Deck.Count > 0;
    public bool Player2IsReady => player2Deck.Count > 0;

    public bool Player1Mulliganed { get; private set; } = false;
    public bool Player2Mulliganed { get; private set; } = false;

    private List<int> player1Deck = [];
    private List<int> player2Deck = [];
    private List<int> player1Hand = [];
    private List<int> player2Hand = [];

    private readonly Random random = new();

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

        if (playerId == player1Id && !Player1IsReady)
        {
            BuildAndShuffle(deck, out player1Deck, out player1Hand);

        }
        else if (playerId == player2Id && !Player2IsReady)
        {
            BuildAndShuffle(deck, out player2Deck, out player2Hand);
        }

        if (Player1IsReady && Player2IsReady)
        {
            State = GameState.Mulligan;
        }

        OnGameUpdated(this, EventArgs.Empty);
    }

    public void Mulligan(string playerId, List<int> indices)
    {
        if (State != GameState.Mulligan) return;

        LastUpdated = DateTime.Now;

        if (playerId == player1Id)
        {
            PullAndReplaceCards(player1Hand, player1Deck, indices);
            Player1Mulliganed = true;
        }
        else if (playerId == player2Id)
        {
            PullAndReplaceCards(player2Hand, player2Deck, indices);
            Player2Mulliganed = true;
        }

        if (Player1Mulliganed && Player2Mulliganed)
        {
            State = random.Next(2) == 0 ? GameState.Player1Turn : GameState.Player2Turn;
        }

        OnGameUpdated(this, EventArgs.Empty);
    }

    public List<int> GetHand(string playerId)
    {
        if (playerId == player1Id) return player1Hand;
        if (playerId == player2Id) return player2Hand;
        return [];
    }

    private void BuildAndShuffle(Deck deck, out List<int> playerDeck, out List<int> playerHand)
    {
        playerDeck = [];

        foreach (var card in deck.cards)
        {
            playerDeck.Add(card.index);
            if (card.count == 2) playerDeck.Add(card.index);
        }

        for (var i = playerDeck.Count - 1; i > 0; --i)
        {
            var j = random.Next(i + 1);
            (playerDeck[j], playerDeck[i]) = (playerDeck[i], playerDeck[j]);
        }

        playerHand = [.. playerDeck.Take(5)];
        playerDeck.RemoveRange(0, 5);
    }

    private static void PullAndReplaceCards(List<int> hand, List<int> deck, List<int> indices)
    {
        foreach (var index in indices)
        {
            if (index < 0 || index >= hand.Count || deck.Count == 0) continue;
            var card = hand[index];
            hand.RemoveAt(index);
            hand.Add(deck[0]);
            deck.RemoveAt(0);
            deck.Add(card);
        }
    }
}