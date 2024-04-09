using System.Collections.ObjectModel;

namespace queensblood;

public interface ICardSetService
{
    public ReadOnlyCollection<CardSet> GetCardSets();
    public Task<bool> SaveSet(string note, IList<Card> cards);
    public CardSet GetLatestCardSet();

}