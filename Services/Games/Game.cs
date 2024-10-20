namespace queensblood;

public record CellPosition(int Row, int Cell);

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

public class Game
{
    public int Round { get; private set; } = 1;

    public static readonly Game None = new("", "");

    public event EventHandler OnGameUpdated = delegate { };

    public string Id { get; }

    public DateTime Created { get; } = DateTime.Now;

    public DateTime LastUpdated { get; private set; } = DateTime.Now;

    public GameState State { get; private set; } = GameState.PickingDecks;

    public PlayerType PlayerTurn { get; private set; } = PlayerType.Undecided;

    private readonly Random random = new();

    private readonly string player1Id;
    private string player2Id = "";

    public bool Player1IsReady => player1Deck.Count > 0;
    public bool Player2IsReady => player2Deck.Count > 0;

    public bool Player1Mulliganed { get; private set; } = false;
    public bool Player2Mulliganed { get; private set; } = false;

    private List<int> player1Deck = [];
    private List<int> player2Deck = [];
    private List<int> player1Hand = [];
    private List<int> player2Hand = [];

    // public Field Field { get; } = new();
    private readonly Field player1Field;
    private readonly Field player2Field;
    public Field GetField(PlayerType playerType) => playerType == PlayerType.Player2 ? player2Field : player1Field;

    public bool PlayerHasSkipped = false;

    public PlayerType Winner { get; private set; } = PlayerType.Undecided;

    public bool IsActive => Id != "" && DateTime.Now.Subtract(TimeSpan.FromMinutes(Values.ACTIVE_MINUTES)) < LastUpdated;

    public Game(string id, string player1Id)
    {
        Id = id;
        this.player1Id = player1Id;

        var player1Rows = new FieldRow[Values.ROWS];
        var player2Rows = new FieldRow[Values.ROWS];
        for (var rowIndex = 0; rowIndex < Values.ROWS; ++rowIndex)
        {
            FieldCell[] cells = [
                new FieldCell(null, 1, PlayerType.Player1),
                new FieldCell(null, 0, PlayerType.Undecided),
                new FieldCell(null, 0, PlayerType.Undecided),
                new FieldCell(null, 0, PlayerType.Undecided),
                new FieldCell(null, 1, PlayerType.Player2),
            ];
            player1Rows[rowIndex] = new FieldRow(cells);
            player2Rows[rowIndex] = new FieldRow(cells.Reverse().ToArray());
        }

        player1Field = new Field(player1Rows);
        player2Field = new Field(player2Rows);
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

    private PlayerType GetPlayerType(string playerId)
    {
        if (playerId == player1Id) return PlayerType.Player1;
        if (playerId == player2Id) return PlayerType.Player2;
        return PlayerType.Spectator;
    }

    public List<int> GetHand(string playerId)
    {
        if (playerId == player1Id) return player1Hand;
        if (playerId == player2Id) return player2Hand;
        return [];
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

    public void SkipTurn(string playerId)
    {
        var playerType = GetPlayerType(playerId);

        if (State != GameState.Playing) return;
        if (PlayerTurn != playerType) return;
        if (PlayerHasSkipped)
        {
            State = GameState.GameOver;
            Winner = GetWinner();
            OnGameUpdated(this, EventArgs.Empty);
            return;
        }

        ChangeTurnAndDraw(true);
    }

    public void Forfeit(string playerId)
    {
        if (State == GameState.GameOver) return;

        var playerType = GetPlayerType(playerId);
        Winner = playerType == PlayerType.Player1 ? PlayerType.Player2 : PlayerType.Player1;
        State = GameState.GameOver;
        OnGameUpdated(this, EventArgs.Empty);
    }

    public void PlayCard(string playerId, int handIndex, int rowIndex, int cellIndex)
    {
        var playerType = GetPlayerType(playerId);

        // Get the correct field and cell based on player type
        if (rowIndex < 0 || rowIndex >= Values.ROWS || cellIndex < 0 || cellIndex >= Values.COLUMNS) return;
        var field = GetField(playerType);
        var cell = field.Rows[rowIndex].Cells[cellIndex];

        // Check some validity
        var validCell = State == GameState.Playing && PlayerTurn == playerType && cell.Owner == playerType;
        if (!validCell) return;

        // Get the correct hand based on player type
        var hand = playerId == player1Id ? player1Hand : player2Hand;
        if (handIndex < 0 || handIndex >= hand.Count) return;

        // Pull the card from the hand
        var cardId = hand[handIndex];
        var card = Cards.At(cardId);
        if (card == null || card.Cost > cell.Pins) return;
        hand.RemoveAt(handIndex);

        // Replacer cards destroy the card they replace
        // And must be played against a cell with a card
        if (card.Replaces)
        {
            if (cell.Card == null) return;
            RunAbility(cell.Card.Destroyed, field, rowIndex, cellIndex);
            // TODO Notify that a card has been destroyed
        }
        // Regular cards can only be played on empty cells
        else
        {
            if (cell.Card != null) return;
        }

        // Place it
        cell.Card = card;

        // Run it's on-play ability
        RunAbility(cell.Card.Played, field, rowIndex, cellIndex);

        // TODO Notify that a card has been played

        // Boost the pins
        foreach (var offset in card.RankPositions)
        {
            var targetRowIndex = rowIndex + offset.Row;
            var targetCellIndex = cellIndex + offset.Cell;

            if (targetRowIndex < 0 || targetRowIndex >= Values.ROWS || targetCellIndex < 0 || targetCellIndex >= Values.COLUMNS) continue;

            var targetCell = field.Rows[targetRowIndex].Cells[targetCellIndex];
            if (targetCell.Card != null) continue;

            targetCell.Pins += card.RankBoost;
        }

        // Change turn
        ChangeTurnAndDraw();
    }

    private static IEnumerable<CellPosition> GetTargetPositions(int startRowIndex, int startCellIndex, RankOffset[] offsets)
    {
        foreach (var offset in offsets)
        {
            var rowIndex = startRowIndex + offset.Row;
            var cellIndex = startCellIndex + offset.Cell;

            if (rowIndex < 0 || rowIndex >= Values.ROWS || cellIndex < 0 || cellIndex >= Values.COLUMNS) continue;

            yield return new(rowIndex, cellIndex);
        }
    }

    private void RunAbility(Ability ability, Field field, int rowIndex, int cellIndex)
    {
        switch (ability.Effect)
        {
            case Effect.AddCard:
                break;
            case Effect.SpawnCards:
                break;
            case Effect.Enfeeble:
                EnfeebleTargets(ability.Value, field, rowIndex, cellIndex);
                break;
            case Effect.Enhance:
                EnhanceTargets(ability.Value, field, rowIndex, cellIndex);
                break;
            case Effect.Destroy:
                DestroyTargets(field, rowIndex, cellIndex);
                break;
            default:
                break;
        }
    }

    private void ChangeTurnAndDraw(bool fromSkip = false)
    {
        PlayerHasSkipped = fromSkip;

        LastUpdated = DateTime.Now;

        PlayerTurn = PlayerTurn == PlayerType.Player1 ? PlayerType.Player2 : PlayerType.Player1;
        Round++;

        // Don't draw for first round
        if (Round == 2)
        {
            OnGameUpdated(this, EventArgs.Empty);
            return;
        }

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

    public bool HasAvailableCellToPlay(string playerId)
    {
        var playerType = GetPlayerType(playerId);
        if (playerType == PlayerType.Spectator || playerType == PlayerType.Undecided) return true;

        var hand = playerId == player1Id ? player1Hand : player2Hand;

        var maxPins = hand.Min((cardId) => Cards.At(cardId)?.Cost ?? 0);
        return player1Field.Rows.Any((row) => row.Cells.Any((cell) => cell.Card == null && cell.Pins >= maxPins));
    }

    private PlayerType GetWinner()
    {
        var player1Score = player1Field.Rows.Sum((row) => row.Player1Score);
        var player2Score = player1Field.Rows.Sum((row) => row.Player2Score);

        if (player1Score > player2Score) return PlayerType.Player1;
        if (player2Score > player1Score) return PlayerType.Player2;

        return PlayerType.Undecided;
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

    private static PlayerType Opposite(PlayerType playerType) => playerType == PlayerType.Player1 ? PlayerType.Player2 : PlayerType.Player1;

    private void EnfeebleTargets(int amount, Field field, int rowIndex, int cellIndex)
    {
        var cell = field.Rows[rowIndex].Cells[cellIndex];

        if (cell.Card == null) return;

        foreach (var targetPosition in GetTargetPositions(rowIndex, cellIndex, cell.Card.AbilityPositions))
        {
            var targetCell = field.Rows[targetPosition.Row].Cells[targetPosition.Cell];
            if (targetCell.Card == null) continue;

            targetCell.Card.Enfeeble(amount);
            RunAbility(targetCell.Card.Enfeebled, field, targetPosition.Row, targetPosition.Cell);

            if (targetCell.Card.Power <= 0)
            {
                RunAbility(targetCell.Card.Destroyed, field, targetPosition.Row, targetPosition.Cell);
                // TODO Notify that a card has been destroyed
            }
        }
    }

    private void EnhanceTargets(int amount, Field field, int rowIndex, int cellIndex)
    {
        var cell = field.Rows[rowIndex].Cells[cellIndex];

        if (cell.Card == null) return;

        foreach (var targetPosition in GetTargetPositions(rowIndex, cellIndex, cell.Card.AbilityPositions))
        {
            var targetCell = field.Rows[targetPosition.Row].Cells[targetPosition.Cell];
            if (targetCell.Card == null) continue;

            targetCell.Card.Enhance(amount);
            RunAbility(targetCell.Card.Enhanced, field, targetPosition.Row, targetPosition.Cell);
        }
    }

    private void DestroyTargets(Field field, int rowIndex, int cellIndex)
    {
        var cell = field.Rows[rowIndex].Cells[cellIndex];

        if (cell.Card == null) return;

        foreach (var targetPosition in GetTargetPositions(rowIndex, cellIndex, cell.Card.AbilityPositions))
        {
            var targetCell = field.Rows[targetPosition.Row].Cells[targetPosition.Cell];
            if (targetCell.Card == null) continue;

            RunAbility(targetCell.Destroy(), field, targetPosition.Row, targetPosition.Cell);
        }
    }
}