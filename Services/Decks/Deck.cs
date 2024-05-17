#pragma warning disable IDE1006 // Naming Styles
namespace queensblood;

public record DeckCard(int index, int count)
{
    public bool IsValid => index >= 0 && index < Cards.Count && count >= 1 && count <= 2;
}

public record Deck(DeckCard[] cards)
{
    public const int DECK_SIZE = 15;

    public static readonly Deck None = new([]);

    public static Deck BuildDefault()
    {
        var cards = new DeckCard[] {
            new(0, 2), new(1, 2), new(2, 2), new(3, 2),
            new(4, 2), new(5, 2), new(6, 2), new(7, 1)
        };
        return new(cards);
    }

    public bool IsValid
    {
        get
        {
            var deckRightSize = cards.Sum(cd => cd.count) == DECK_SIZE;
            var allCardsCorrect = cards.Any(cd => cd.IsValid);
            return deckRightSize && allCardsCorrect;
        }
    }
}
