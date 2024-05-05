namespace queensblood;

public record Card(string Name, int PinCost, int Value, int Boosts)
{
    public static readonly Card Null = new("", 0, 0, 0);
    public static readonly Card New = new("Name", 1, 1, 0);

    public override string ToString()
    {
        return $"\"{Name}\",{PinCost},{Value},{Boosts}";
    }

    private static readonly int[] fieldValues =
        [1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384, 32768, 65536, 131072, 262144, 524288, 1048576, 2097152, 4194304, 8388608, 16777216];

    public bool FieldIsBoosted(int index)
    {
        if (index < 0 || index > 24 || index == 12) return false;
        return (Boosts & fieldValues[index]) != 0;
    }
}