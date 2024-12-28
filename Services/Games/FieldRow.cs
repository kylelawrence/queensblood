using System.Collections.ObjectModel;

namespace queensblood;

public class FieldRow(IList<FieldCell> cells)
{
    public int Player1Score => Cells.Sum((cell) =>
    {
        if (cell.Owner != PlayerType.Player1) return 0;
        return cell.Card?.Power ?? 0;
    }) + Player1ScoreAdjustment;

    public int Player2Score => Cells.Sum((cell) =>
    {
        if (cell.Owner != PlayerType.Player2) return 0;
        return cell.Card?.Power ?? 0;
    }) + Player2ScoreAdjustment;

    public int Player1ScoreAdjustment { get; set; } = 0;
    public int Player2ScoreAdjustment { get; set; } = 0;

    public readonly ReadOnlyCollection<FieldCell> Cells = new(cells);

    public IEnumerable<FieldCell> OccupiedCells => Cells.Where((cell) => cell.Card != null);
}