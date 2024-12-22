namespace queensblood;
public class FieldCell(int rowIndex, int cellIndex, int pins, PlayerType owner)
{
    public Card? Card { get; set; } = null;
    public int Pins { get; set; } = pins;
    public PlayerType Owner { get; set; } = owner;
    private readonly CellPosition player1Position = new(rowIndex, cellIndex);
    private readonly CellPosition player2Position = new(rowIndex, Values.LAST_COL_INDEX - cellIndex);
    public CellPosition GetPosition() => Owner == PlayerType.Player2 ? player2Position : player1Position;
    public CellPosition GetPosition(PlayerType playerType) => playerType == PlayerType.Player2 ? player2Position : player1Position;

    public Dictionary<string, Tuple<int, PlayerType>> PowerEffects { get; set; } = [];

    public Ability Destroy()
    {
        if (Card == null) return Ability.None;

        var ability = Card.Destroyed;
        Card = null;
        return ability;
    }
}