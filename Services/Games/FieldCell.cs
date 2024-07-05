namespace queensblood;

public class FieldCell(int rowIndex, int columnIndex, Card card, int pins, PlayerType owner)
{
    public static readonly FieldCell Empty = new(-1, -1, Card.Null, 0, PlayerType.Undecided);

    public int RowIndex { get; private set; } = rowIndex;
    public int ColumnIndex { get; private set; } = columnIndex;
    public Card Card { get; set; } = card;
    public int Pins { get; set; } = pins;
    public PlayerType Owner { get; set; } = owner;
}