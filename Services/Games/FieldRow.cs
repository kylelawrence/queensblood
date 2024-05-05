namespace queensblood;

public class FieldRow(int rowIndex)
{
    public int Player1Score => Cells.Sum((cell) => {
        if (cell.Owner != PlayerType.Player1) return 0;
        return Cards.At(cell.CardId).Value;
    });

    public int Player2Score => Cells.Sum((cell) => {
        if (cell.Owner != PlayerType.Player2) return 0;
        return Cards.At(cell.CardId).Value;
    });

    private readonly FieldCell[] Cells = [
            new FieldCell(rowIndex, 0, -1, 1, PlayerType.Player1),
            new FieldCell(rowIndex, 1, -1, 0, PlayerType.Undecided),
            new FieldCell(rowIndex, 2, -1, 0, PlayerType.Undecided),
            new FieldCell(rowIndex, 3, -1, 0, PlayerType.Undecided),
            new FieldCell(rowIndex, 4, -1, 1, PlayerType.Player2),
        ];

    public int RowIndex { get; private set; } = rowIndex;

    public FieldCell[] GetCells(PlayerType playerType)
    {
        return playerType == PlayerType.Player2 ? Cells.Reverse().ToArray() : Cells;
    }
}