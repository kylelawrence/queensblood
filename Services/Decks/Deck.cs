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
