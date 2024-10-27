namespace queensblood;

public class Field(FieldRow[] rows)
{
    public static readonly Field None = new([]);

    public FieldRow[] Rows { get; private set; } = rows;

    public IEnumerable<FieldCell> OccupiedCells => Rows.SelectMany((row) => row.OccupiedCells);
}