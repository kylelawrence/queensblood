using System.Collections.ObjectModel;
using MongoDB.Driver;
using Rock = System.Collections.ObjectModel.ReadOnlyCollection<queensblood.Card>;

namespace queensblood;

public record CardSet(int Iteration, DateTime Created, Rock Cards, string Note)
{
    public static readonly CardSet Empty = new(0, DateTime.Now, new Rock([]), "Initial");

    public override string ToString()
    {
        return $"{Created} - {Note}";
    }
}

public record Card(string Name, int PinCost, int Value, int Boosts)
{
    public static readonly Card Null = new("", 0, 0, 0);
    public static readonly Card New = new("Name", 1, 1, 0);

    public override string ToString()
    {
        return $"\"{Name}\",{PinCost},{Value},{Boosts}";
    }
}

public interface ICardsService
{
    public ReadOnlyCollection<CardSet> GetCardSets();
    public Task<bool> SaveSet(string note, IList<Card> cards);
    public CardSet GetLatestCardSet();
}

public class CardsMongoService : ICardsService
{
    private readonly IMongoDatabase db;
    private readonly IMongoCollection<CardSet> cardSets;

    public CardsMongoService(IMongoDatabase database)
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
        return cardSets.AsQueryable().MaxBy(cs => cs.Iteration) ?? CardSet.Empty;
    }

    public async Task<bool> SaveSet(string note, IList<Card> cards)
    {
        var iteration = cardSets.AsQueryable().Any() ? cardSets.AsQueryable().Max(cs => cs.Iteration) + 1 : 1;
        var task = cardSets.InsertOneAsync(new(iteration, DateTime.Now, new Rock(cards), note));
        return await task.Try();
    }
}