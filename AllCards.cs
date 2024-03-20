namespace queensblood;

// public record Card(uint ID, string Name, uint PinCost, uint Value, uint Boosts);

public class Card(uint id, string name, uint pinCost, uint value, uint boosts)
{
    public uint ID { get; set; } = id;
    public string Name { get; set; } = name;
    public uint PinCost { get; set; } = pinCost;
    public uint Value { get; set; } = value;
    public uint Boosts { get; set; } = boosts;

    public static Card GetBlank()
    {
        return new(0, "Name", 1, 1, 1);
    }
}

public class AllCards
{
    public static Card GetCard(uint id)
    {
        int[] derp = [0x00000];
        return _cards[id];
    }

    private static readonly Card[] _cards = [];
}