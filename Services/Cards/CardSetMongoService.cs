using System.Collections.ObjectModel;
using MongoDB.Driver;

namespace queensblood;

public class CardSetMongoService : ICardSetService
{
    private readonly IMongoDatabase db;
    private readonly IMongoCollection<CardSet> cardSets;

    public CardSetMongoService(IMongoDatabase database)
    {
        db = database;
        cardSets = db.GetCollection<CardSet>("cardsets");
    }

    public ReadOnlyCollection<CardSet> GetCardSets()
    {
        var list = cardSets.AsQueryable().OrderByDescending(cs => cs.Iteration).ToList();
        if (list.Count == 0)
        {
            list.Add(CardSet.Empty);
        }
        return list.AsReadOnly();
    }

    public CardSet GetLatestCardSet()
    {
        return cardSets
            .Find(x => true)
            .SortByDescending(cs => cs.Iteration)
            .Limit(1)
            .First() ?? CardSet.Empty;
    }

    public async Task<bool> SaveSet(string note, IList<Card> cards)
    {
        var iteration = cardSets.AsQueryable().Any() ? cardSets.AsQueryable().Max(cs => cs.Iteration) + 1 : 1;
        var task = cardSets.InsertOneAsync(new(iteration, new(cards), note));
        return await task.Try();
    }
}