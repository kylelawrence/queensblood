using queensblood.Components.Pages;

namespace queensblood;

public record CellPosition(int Row, int Cell);

public enum GameState
{
    PickingDecks,
    Mulligan,
    Playing,
    GameOver,
    Debug,
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
    public static readonly Game None = new("", "");

    public string Id { get; }

    public DateTime Created { get; } = DateTime.Now;

    public DateTime LastUpdated { get; private set; } = DateTime.Now;

    public GameState State { get; private set; } = GameState.PickingDecks;

    public bool IsActive => Id != "" && State != GameState.GameOver && DateTime.Now.Subtract(TimeSpan.FromMinutes(Values.ACTIVE_MINUTES)) < LastUpdated;

    public event EventHandler OnGameUpdated = delegate { };

    public int Round { get; private set; } = 1;

    public PlayerType PlayerTurn { get; private set; } = PlayerType.Undecided;

    private readonly Random random = new();

    private readonly string player1Id;
    private string player2Id = "";

    public bool Player1IsReady => player1Deck.Count > 0;
    public bool Player2IsReady => player2Deck.Count > 0;

    public bool Player1Mulliganed { get; private set; } = false;
    public bool Player2Mulliganed { get; private set; } = false;

    private readonly List<Card> player1Deck = [];
    private readonly List<Card> player2Deck = [];
    private readonly List<Card> player1Hand = [];
    private readonly List<Card> player2Hand = [];

    private readonly Field player1Field;
    private readonly Field player2Field;
    public Field GetField(PlayerType playerType) => playerType == PlayerType.Player2 ? player2Field : player1Field;

    public bool PlayerHasSkipped = false;

    public PlayerType Winner { get; private set; } = PlayerType.Undecided;

    private int player1FinalScore = 0;
    private int player2FinalScore = 0;

    public Game(string id, string player1Id)
    {
        Id = id;
        this.player1Id = player1Id;

        var player1Rows = new FieldRow[Values.ROWS];
        var player2Rows = new FieldRow[Values.ROWS];
        for (var rowIndex = 0; rowIndex < Values.ROWS; ++rowIndex)
        {
            FieldCell[] cells = [
                new FieldCell(rowIndex, 0, 1, PlayerType.Player1),
                new FieldCell(rowIndex, 1, 0, PlayerType.Undecided),
                new FieldCell(rowIndex, 2, 0, PlayerType.Undecided),
                new FieldCell(rowIndex, 3, 0, PlayerType.Undecided),
                new FieldCell(rowIndex, 4, 1, PlayerType.Player2),
            ];
            player1Rows[rowIndex] = new FieldRow(cells);
            player2Rows[rowIndex] = new FieldRow(cells.Reverse().ToArray());
        }

        player1Field = new Field(player1Rows);
        player2Field = new Field(player2Rows);
    }

    public static Game CreateDebug()
    {
        return new("debug", "debug1")
        {
            player2Id = "debug2",
            State = GameState.Debug
        };
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

    public List<Card> GetHand(string playerId)
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
            BuildAndShuffle(deck, player1Deck, player1Hand);

        }
        else if (playerId == player2Id && !Player2IsReady)
        {
            BuildAndShuffle(deck, player2Deck, player2Hand);
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

    private static void PullAndReplaceCards(List<Card> hand, List<Card> deck, List<int> indices)
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
        var player1Score = 0;
        var player2Score = 0;
        if (PlayerHasSkipped)
        {
            var partyAnimalActive = false;
            foreach (var cell in player1Field.OccupiedCells)
            {
                if (cell.Card!.Name != "Ultimate Party Animal") continue;
                partyAnimalActive = true;
                break;
            }

            // Game over time, let's calculate scores
            foreach (var row in player1Field.Rows)
            {
                if (row.Player1Score > row.Player2Score)
                {
                    row.Player1ScoreAdjustment = row.Cells.Sum((cell) => cell.Owner == PlayerType.Player1 ? (cell.Card?.LaneWon?.Value ?? 0) : 0);
                    if (partyAnimalActive) row.Player1ScoreAdjustment += row.Player2Score;
                    player1Score += row.Player1Score;
                }
                if (row.Player2Score > row.Player1Score)
                {
                    row.Player2ScoreAdjustment = row.Cells.Sum((cell) => cell.Owner == PlayerType.Player2 ? (cell.Card?.LaneWon?.Value ?? 0) : 0);
                    if (partyAnimalActive) row.Player2ScoreAdjustment += row.Player1Score;
                    player2Score += row.Player2Score;
                }
            }

            player1FinalScore = player1Score;
            player2FinalScore = player2Score;

            State = GameState.GameOver;
            Winner = PlayerType.Undecided;
            if (player1Score > player2Score) Winner = PlayerType.Player1;
            if (player2Score > player1Score) Winner = PlayerType.Player2;

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
        if (State != GameState.Playing || PlayerTurn != playerType || cell.Owner != playerType) return;

        // Get the correct hand based on player type
        var hand = playerId == player1Id ? player1Hand : player2Hand;
        if (handIndex < 0 || handIndex >= hand.Count) return;

        // Pull the card from the hand
        var card = hand[handIndex];
        if (card.Replaces && cell.Card == null) return;
        if (card.Replaces && cell.Owner != playerType) return;
        if (!card.Replaces && cell.Card != null) return;
        if (card == null || card.Cost > cell.Pins) return;
        hand.RemoveAt(handIndex);

        // Place the card
        PlaceCard(card, field, rowIndex, cellIndex);

        // Change turn
        ChangeTurnAndDraw();
    }

    public void DebugPlayCard(Card card, int rowIndex, int cellIndex)
    {
        if (Id != "debug") return;
        var cell = player1Field.Rows[rowIndex].Cells[cellIndex];
        if (card.Replaces && cell.Card == null) return;
        if (!card.Replaces && cell.Card != null) return;
        cell.Owner = PlayerType.Player1;
        PlaceCard(card, player1Field, rowIndex, cellIndex);
    }

    public void DebugOpponentPlayCard(Card card, int rowIndex, int cellIndex)
    {
        if (Id != "debug") return;
        var cell = player1Field.Rows[rowIndex].Cells[cellIndex];
        cell.Owner = PlayerType.Player2;
        PlaceCard(card, player2Field, rowIndex, 4 - cellIndex);
    }

    private void PlaceCard(Card card, Field field, int rowIndex, int cellIndex)
    {
        var cell = field.Rows[rowIndex].Cells[cellIndex];

        // Boost the pins before we possibly destroy any cards
        foreach (var offset in card.RankPositions)
        {
            var targetRowIndex = rowIndex + offset.Row;
            var targetCellIndex = cellIndex + offset.Cell;

            if (targetRowIndex < 0 || targetRowIndex >= Values.ROWS || targetCellIndex < 0 || targetCellIndex >= Values.COLUMNS) continue;

            var targetCell = field.Rows[targetRowIndex].Cells[targetCellIndex];
            if (targetCell.Card != null) continue;

            targetCell.Pins = Math.Min(3, targetCell.Pins + card.RankBoost);
            targetCell.Owner = cell.Owner;
        }

        // Replacer cards destroy the card they replace
        // And must be played against a cell with a card
        if (card.Replaces)
        {
            DestroyCard(cell);
        }

        cell.Card = card;

        RunAbility(card.Played, card.AbilityPositions, field, rowIndex, cellIndex);

        LingerAbility(card.InPlay, card.AbilityPositions, field, rowIndex, cellIndex);

        RunCardPlayedAbilities(field, rowIndex, cellIndex);

        CalculateLingeringEffects();

        CalculateSelfEffects();
    }

    private void CalculateSelfEffects()
    {
        foreach (var cell in player1Field.OccupiedCells)
        {
            var ability = cell.Card!.InPlay;
            if (ability.Effect == Effect.None) continue;
            if (ability.TargetType != CardRelation.Self) continue;

            var powerAdjustment = 0;
            foreach (var occupiedCell in player1Field.OccupiedCells)
            {
                if (occupiedCell == cell) continue;
                if (ability.TargetTrigger == CardRelation.Ally && occupiedCell.Owner != cell.Owner) continue;
                if (ability.TargetTrigger == CardRelation.Enemy && occupiedCell.Owner == cell.Owner) continue;
                if (ability.ValueType == ValueType.Enhanced && !occupiedCell.Card!.HasBeenEnhanced) continue;
                if (ability.ValueType == ValueType.Enfeebled && !occupiedCell.Card!.HasBeenEnfeebled) continue;
                powerAdjustment += ability.Value;
            }

            cell.Card!.SelfAdjustment = powerAdjustment;
        }
    }

    private void RunCardPlayedAbilities(Field hostField, int hostRowIndex, int hostCellIndex)
    {
        var hostCell = hostField.Rows[hostRowIndex].Cells[hostCellIndex];
        foreach (var occupiedCell in hostField.OccupiedCells)
        {
            if (occupiedCell == hostCell) continue;
            var cardPlayedAbility = occupiedCell.Card!.CardPlayed;
            if (cardPlayedAbility == null) continue;
            if (cardPlayedAbility.TargetTrigger == CardRelation.Ally && occupiedCell.Owner != hostCell.Owner) continue;
            if (cardPlayedAbility.TargetTrigger == CardRelation.Enemy && occupiedCell.Owner == hostCell.Owner) continue;
            var occupiedPosition = occupiedCell.GetPosition();
            RunAbility(cardPlayedAbility, occupiedCell.Card!.AbilityPositions, GetField(occupiedCell.Owner), occupiedPosition.Row, occupiedPosition.Cell);
        }
    }

    private void RunCardDestroyedAbilities(Field hostField, int hostRowIndex, int hostCellIndex)
    {
        var hostCell = hostField.Rows[hostRowIndex].Cells[hostCellIndex];
        foreach (var occupiedCell in hostField.OccupiedCells)
        {
            if (occupiedCell == hostCell) continue;
            var cardDestroyed = occupiedCell.Card!.CardDestroyed;
            if (cardDestroyed == null) continue;
            if (cardDestroyed.TargetTrigger == CardRelation.Ally && occupiedCell.Owner != hostCell.Owner) continue;
            if (cardDestroyed.TargetTrigger == CardRelation.Enemy && occupiedCell.Owner == hostCell.Owner) continue;
            var occupiedPosition = occupiedCell.GetPosition();
            RunAbility(cardDestroyed, occupiedCell.Card.AbilityPositions, GetField(occupiedCell.Owner), occupiedPosition.Row, occupiedPosition.Cell);
        }
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

    private void RunAbility(Ability ability, RankOffset[] positions, Field field, int rowIndex, int cellIndex)
    {
        switch (ability.Effect)
        {
            case Effect.AddCard:
                AddCards(ability, field, rowIndex, cellIndex);
                break;
            case Effect.SpawnCards:
                SpawnCards(ability, field, rowIndex, cellIndex);
                break;
            case Effect.Enfeeble:
                EnfeebleTargets(ability, positions, field, rowIndex, cellIndex);
                break;
            case Effect.Enhance:
                EnhanceTargets(ability, positions, field, rowIndex, cellIndex);
                break;
            case Effect.Destroy:
                DestroyTargets(ability, positions, field, rowIndex, cellIndex);
                break;
            default:
                break;
        }
    }

    public static PlayerType GetOpponent(PlayerType playerType) => playerType == PlayerType.Player1 ? PlayerType.Player2 : PlayerType.Player1;

    private static void LingerAbility(Ability ability, RankOffset[] positions, Field field, int rowIndex, int cellIndex)
    {
        var hostCell = field.Rows[rowIndex].Cells[cellIndex];

        if (ability.Effect == Effect.None) return;
        if (ability.TargetType == CardRelation.Self) return;

        foreach (var targetPosition in GetTargetPositions(rowIndex, cellIndex, positions))
        {
            var targetCell = field.Rows[targetPosition.Row].Cells[targetPosition.Cell];
            var cellKey = $"{rowIndex}-{cellIndex}";
            var value = ability.Effect == Effect.Enhance ? ability.Value : -ability.Value;

            // Target type of both is defaulted as undecided
            PlayerType targetPlayerType = PlayerType.Undecided;
            if (ability.TargetType == CardRelation.Ally) targetPlayerType = hostCell.Owner;
            if (ability.TargetType == CardRelation.Enemy) targetPlayerType = GetOpponent(hostCell.Owner);

            targetCell.PowerEffects[cellKey] = new(value, targetPlayerType);
        }
    }

    private static void UnlingerAbility(Ability ability, RankOffset[] positions, Field field, int rowIndex, int cellIndex)
    {
        if (ability.Effect == Effect.None) return;

        foreach (var targetPosition in GetTargetPositions(rowIndex, cellIndex, positions))
        {
            var targetCell = field.Rows[targetPosition.Row].Cells[targetPosition.Cell];
            var cellKey = $"{rowIndex}-{cellIndex}";

            targetCell.PowerEffects.Remove(cellKey);
        }
    }

    private void CalculateLingeringEffects()
    {
        foreach (var cell in player1Field.OccupiedCells)
        {
            var powerAdjustment = 0;
            foreach (var (value, target) in cell.PowerEffects.Values)
            {
                if (target != PlayerType.Undecided && target != cell.Owner) continue;

                powerAdjustment += value;
                Ability changedAbility = Ability.None;
                if (value > 0)
                {
                    if (cell.Card!.Power7.Effect != Effect.None && !cell.Card!.HasHitPower7 && cell.Card!.Power + powerAdjustment >= 7)
                    {
                        changedAbility = cell.Card!.Power7;
                        cell.Card!.HasHitPower7 = true;
                    }
                    else if (!cell.Card!.HasBeenEnhanced)
                    {
                        changedAbility = cell.Card!.Enhanced;
                    }
                    cell.Card!.HasBeenEnhanced = true;
                }
                else
                {
                    if (!cell.Card!.HasBeenEnfeebled)
                    {
                        changedAbility = cell.Card!.Enfeebled;
                    }
                    cell.Card!.HasBeenEnfeebled = true;
                }

                var cellPosition = cell.GetPosition();
                RunAbility(changedAbility, cell.Card.AbilityPositions, GetField(cell.Owner), cellPosition.Row, cellPosition.Cell);
            }

            cell.Card!.PowerAdjustment = powerAdjustment;

            if (cell.Card!.AdjustedPower <= 0)
            {
                DestroyCard(cell);
            }
        }
    }

    private void AddCards(Ability ability, Field field, int rowIndex, int cellIndex)
    {
        var cell = field.Rows[rowIndex].Cells[cellIndex];
        var hand = cell.Owner == PlayerType.Player1 ? player1Hand : player2Hand;

        foreach (var cardMaker in Cards.CardsToAdd(ability.Value))
        {
            var cardToAdd = cardMaker();
            hand.Add(cardToAdd);
        }
    }

    private void SpawnCards(Ability ability, Field field, int rowIndex, int cellIndex)
    {
        var cell = field.Rows[rowIndex].Cells[cellIndex];
        var owner = cell.Owner;

        var cardSpawns = Cards.CardsToSpawn(ability.Value);
        List<CellPosition> spawnLocations = [];
        var cardSpawnIndex = 0;
        for (var spawnRow = 0; spawnRow < Values.ROWS; ++spawnRow)
        {
            for (var spawnCell = 0; spawnCell < Values.COLUMNS; ++spawnCell)
            {
                var cellToSpawn = field.Rows[spawnRow].Cells[spawnCell];
                if (cellToSpawn.Card != null) continue;
                if (cellToSpawn.Owner != owner) continue;
                spawnLocations.Add(new(spawnRow, spawnCell));
            }
        }

        // Shuffle spawn locations
        for (var i = spawnLocations.Count - 1; i > 0; --i)
        {
            var j = random.Next(i + 1);
            (spawnLocations[j], spawnLocations[i]) = (spawnLocations[i], spawnLocations[j]);
        }

        foreach (var (spawnRow, spawnCell) in spawnLocations)
        {
            var cardMaker = cardSpawns[cardSpawnIndex >= cardSpawns.Length ? 0 : cardSpawnIndex];
            cardSpawnIndex++;

            PlaceCard(cardMaker(), field, spawnRow, spawnCell);
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

        var maxPins = hand.Min((card) => card.Cost);
        return player1Field.Rows.Any((row) => row.Cells.Any((cell) => cell.Card == null && cell.Pins >= maxPins));
    }

    private void BuildAndShuffle(Deck deck, List<Card> playerDeck, List<Card> playerHand)
    {
        foreach (var card in deck.cards)
        {
            // Add a card twice if it has a count of 2
            var cardToAdd = Cards.At(card.index);
            if (cardToAdd == null) continue;
            playerDeck.Add(cardToAdd);
            if (card.count == 2) playerDeck.Add(Cards.At(card.index)!);
        }

        for (var i = playerDeck.Count - 1; i > 0; --i)
        {
            // Swap the current card with a random card
            var j = random.Next(i + 1);
            (playerDeck[j], playerDeck[i]) = (playerDeck[i], playerDeck[j]);
        }

        for (var i = 0; i < Values.HAND_SIZE; ++i)
        {
            playerHand.Add(playerDeck[i]);
        }

        playerDeck.RemoveRange(0, Values.HAND_SIZE);
    }

    private void EnfeebleTargets(Ability ability, RankOffset[] positions, Field field, int rowIndex, int cellIndex)
    {
        var cell = field.Rows[rowIndex].Cells[cellIndex];

        foreach (var targetPosition in GetTargetPositions(rowIndex, cellIndex, positions))
        {
            var targetCell = field.Rows[targetPosition.Row].Cells[targetPosition.Cell];
            if (targetCell.Card == null) continue;

            if (ability.TargetType == CardRelation.Enemy && targetCell.Owner == cell.Owner) continue;
            if (ability.TargetType == CardRelation.Ally && targetCell.Owner != cell.Owner) continue;

            targetCell.Card.Enfeeble(ability.Value);
            if (targetCell.Card.AdjustedPower <= 0)
            {
                DestroyCard(targetCell);
                continue;
            }

            RunAbility(targetCell.Card.HasBeenEnfeebled ? Ability.None : targetCell.Card.Enfeebled, targetCell.Card.AbilityPositions, field, targetPosition.Row, targetPosition.Cell);
            targetCell.Card.HasBeenEnfeebled = true;
        }
    }

    private void EnhanceTargets(Ability ability, RankOffset[] positions, Field field, int rowIndex, int cellIndex)
    {
        var cell = field.Rows[rowIndex].Cells[cellIndex];

        if (ability.TargetType == CardRelation.Self)
        {
            if (cell.Card == null) return;

            cell.Card.HasBeenEnhanced = true;
            cell.Card.Enhance(ability.Value);
            return;
        }

        foreach (var targetPosition in GetTargetPositions(rowIndex, cellIndex, positions))
        {
            var targetCell = field.Rows[targetPosition.Row].Cells[targetPosition.Cell];
            if (targetCell.Card == null) continue;

            if (ability.TargetType == CardRelation.Enemy && targetCell.Owner == cell.Owner) continue;
            if (ability.TargetType == CardRelation.Ally && targetCell.Owner != cell.Owner) continue;

            if (targetCell.Card.Power7.Effect != Effect.None && !targetCell.Card.HasHitPower7)
            {
                targetCell.Card.HasHitPower7 = true;
                targetCell.Card.HasBeenEnhanced = true;
                RunAbility(targetCell.Card.Power7, targetCell.Card.AbilityPositions, field, targetPosition.Row, targetPosition.Cell);
                continue;
            }

            targetCell.Card.Enhance(ability.Value);

            if (targetCell.Card.HasBeenEnhanced) continue;
            RunAbility(targetCell.Card.Enhanced, targetCell.Card.AbilityPositions, field, targetPosition.Row, targetPosition.Cell);
        }
    }

    private void DestroyTargets(Ability ability, RankOffset[] positions, Field field, int rowIndex, int cellIndex)
    {
        var cell = field.Rows[rowIndex].Cells[cellIndex];

        foreach (var targetPosition in GetTargetPositions(rowIndex, cellIndex, positions))
        {
            var targetCell = field.Rows[targetPosition.Row].Cells[targetPosition.Cell];
            if (targetCell.Card == null) continue;

            if (ability.TargetType == CardRelation.Enemy && targetCell.Owner == cell.Owner) continue;
            if (ability.TargetType == CardRelation.Ally && targetCell.Owner != cell.Owner) continue;

            DestroyCard(targetCell);
        }
    }

    public void DestroyCard(FieldCell cell)
    {
        if (cell.Card == null) return;
        var field = GetField(cell.Owner);
        var cardPositions = cell.Card.AbilityPositions;
        var cellPosition = cell.GetPosition();
        UnlingerAbility(cell.Card.InPlay, cardPositions, field, cellPosition.Row, cellPosition.Cell);
        RunAbility(cell.Destroy(), cardPositions, field, cellPosition.Row, cellPosition.Cell);
        RunCardDestroyedAbilities(field, cellPosition.Row, cellPosition.Cell);
    }

    public int GetFinalScore(PlayerType playerType)
    {
        if (playerType == PlayerType.Player1) return player1FinalScore;
        if (playerType == PlayerType.Player2) return player2FinalScore;
        return 0;
    }
}
