namespace queensblood;

public interface IDecksService
{
    Task SaveDeck(int index, DeckCard[] cards);
    Task<Deck> LoadDeck(int index);
    Task<int> GetSelectedDeck();
}
