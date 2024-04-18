#pragma warning disable IDE1006 // Naming Styles
namespace queensblood;

public record DeckCard(int index, int count);

public record Deck(DeckCard[] cards)
{
    public static readonly Deck None = new([]);
}
