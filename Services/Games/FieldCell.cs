namespace queensblood;

public class FieldCell(int rowIndex, int columnIndex, int cardId, int pins, PlayerType owner)
{
    public static readonly FieldCell Empty = new(-1, -1, -1, 0, PlayerType.Undecided);

    public int RowIndex { get; private set; } = rowIndex;
    public int ColumnIndex { get; private set; } = columnIndex;
    public int CardId { get; set; } = cardId;
    public int Pins { get; set; } = pins;
    public PlayerType Owner { get; set; } = owner;
}