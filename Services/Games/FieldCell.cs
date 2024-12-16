namespace queensblood;

public class FieldCell(int rowIndex, int cellIndex, int pins, PlayerType owner)
{
    public Card? Card { get; set; } = null;
    public int Pins { get; set; } = pins;
    public PlayerType Owner { get; set; } = owner;
    public int RowIndex { get; set; } = rowIndex;
    public int CellIndex { get; set; } = cellIndex;

    public Dictionary<string, Tuple<int, PlayerType>> PowerEffects { get; set; } = [];

    public Ability Destroy()
    {
        if (Card == null) return Ability.None;

        var ability = Card.Destroyed;
        Card = null;
        return ability;
    }
}