namespace queensblood;

public record Card(uint ID, string Name, uint PinCount, uint Value, uint boosts);

public class AllCards
{
    public static Card GetCard(uint id)
    {
        int[] derp = [0x00000];
        return _cards[id];
    }

    private static readonly Card[] _cards = [];
}