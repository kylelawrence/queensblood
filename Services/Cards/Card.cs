namespace queensblood;

public record Card(string Name, int PinCost, int Value, int Boosts)
{
    public static readonly Card Null = new("", 0, 0, 0);
    public static readonly Card New = new("Name", 1, 1, 0);

    public override string ToString()
    {
        return $"\"{Name}\",{PinCost},{Value},{Boosts}";
    }
}