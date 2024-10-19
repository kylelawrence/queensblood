namespace queensblood;

public class Field(FieldRow[] rows)
{
    public static readonly Field None = new([]);

    public FieldRow[] Rows { get; private set; } = rows;

    public IEnumerable<FieldCell> GetOccupiedCells()
    {
        return Rows.SelectMany((row) => row.GetOccupiedCells());
    }
}