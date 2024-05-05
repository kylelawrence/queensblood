namespace queensblood;

public class Field
{
    public FieldRow[] Rows { get; private set; }

    public Field()
    {
        Rows = [
            new FieldRow(0), 
            new FieldRow(1), 
            new FieldRow(2), 
        ];
    }
}