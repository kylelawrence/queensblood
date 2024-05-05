namespace queensblood;

public enum GameState
{
    PickingDecks,
    Mulligan,
    Playing,
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

    public PlayerType PlayerTurn { get; private set; } = PlayerType.Undecided;

    private readonly Random random = new();

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
    public Field Field { get; } = new();

    private bool playerHasSkipped = false;

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
            State = GameState.Playing;
            PlayerTurn = random.Next(2) == 0 ? PlayerType.Player1 : PlayerType.Player2;
        }

        OnGameUpdated(this, EventArgs.Empty);
    }

    public void PlayCard(string playerId, int handIndex, FieldCell cell)
    {
        var playerType = GetPlayerType(playerId);

        if (State != GameState.Playing) return;
        if (cell.CardId != -1) return;
        if (PlayerTurn != playerType) return;
        if (cell.Owner != playerType) return;

        var hand = playerId == player1Id ? player1Hand : player2Hand;
        if (handIndex < 0 || handIndex >= hand.Count) return;

        var cardId = hand[handIndex];
        var card = Cards.At(cardId);
        if (card == Card.Null) return;
        if (card.PinCost > cell.Pins) return;

        cell.CardId = cardId;
        hand.RemoveAt(handIndex);

        // Boost the pins
        for (var i = 0; i < Values.BOOST_COUNT; i++)
        {
            if (card.FieldIsBoosted(i))
            {
                var columnIndex = playerType == PlayerType.Player1 ? cell.ColumnIndex : Values.LAST_COL_INDEX - cell.ColumnIndex;

                // Use the offsets to get the target row and column
                var boostRow = cell.RowIndex + Boosts.GetRowOffset(i);
                var boostColumn = columnIndex + Boosts.GetColumnOffset(i);

                // Skip if the target is out of bounds
                if (boostRow < 0 || boostRow >= Values.ROWS || boostColumn < 0 || boostColumn >= Values.COLUMNS) continue;

                // Get the target cell
                var row = Field.Rows[boostRow];
                var cells = row.GetCells(playerType);
                var cellToBoost = cells[boostColumn];

                // Skip if the target cell is already occupied
                if (cellToBoost.CardId != -1) continue;

                // Boost the pins and set the owner
                cellToBoost.Pins = Math.Min(cellToBoost.Pins + 1, Values.MAX_PINS);
                cellToBoost.Owner = playerType;
            }
        }

        ChangeTurnAndDraw();
    }

    public void SkipTurn(string playerId)
    {
        var playerType = GetPlayerType(playerId);

        if (State != GameState.Playing) return;
        if (PlayerTurn != playerType) return;
        if (playerHasSkipped) {
            // end game
        }

        playerHasSkipped = true;

        ChangeTurnAndDraw();
    }

    private void ChangeTurnAndDraw()
    {
        playerHasSkipped = false;

        LastUpdated = DateTime.Now;

        PlayerTurn = PlayerTurn == PlayerType.Player1 ? PlayerType.Player2 : PlayerType.Player1;
        if (PlayerTurn == PlayerType.Player1 && player1Deck.Count > 0)
        {
            player1Hand.Add(player1Deck[0]);
            player1Deck.RemoveAt(0);
        }
        else if (PlayerTurn == PlayerType.Player2 && player2Deck.Count > 0)
        {
            player2Hand.Add(player2Deck[0]);
            player2Deck.RemoveAt(0);
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
            // Add a card twice if it has a count of 2
            playerDeck.Add(card.index);
            if (card.count == 2) playerDeck.Add(card.index);
        }

        for (var i = playerDeck.Count - 1; i > 0; --i)
        {
            // Swap the current card with a random card
            var j = random.Next(i + 1);
            (playerDeck[j], playerDeck[i]) = (playerDeck[i], playerDeck[j]);
        }

        playerHand = [.. playerDeck.Take(Values.HAND_SIZE)];
        playerDeck.RemoveRange(0, Values.HAND_SIZE);
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

    private PlayerType GetPlayerType(string playerId)
    {
        if (playerId == player1Id) return PlayerType.Player1;
        if (playerId == player2Id) return PlayerType.Player2;
        return PlayerType.Spectator;
    }
}