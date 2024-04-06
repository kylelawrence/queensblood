using System.Collections.ObjectModel;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Rock = System.Collections.ObjectModel.ReadOnlyCollection<queensblood.Card>;

namespace queensblood;

public class CardSet
{
    public static readonly CardSet Empty = new(0, new Rock([]), "Initial");

    [BsonId]
    public ObjectId Id { get; } = ObjectId.GenerateNewId();
    public int Iteration { get; }
    public DateTime Created { get; }
    public Rock Cards { get; }
    public string Note { get; }

    public CardSet(int iteration, Rock cards, string note)
    {
        Id = ObjectId.GenerateNewId();
        Created = DateTime.Now;
        Iteration = iteration;
        Cards = cards;
        Note = note;
    }

    public CardSet(ObjectId id, int iteration, DateTime created, Rock cards, string note)
    {
        Id = id;
        Iteration = iteration;
        Created = created;
        Cards = cards;
        Note = note;
    }

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
        return cardSets
            .Find(x => true)
            .SortByDescending(cs => cs.Iteration)
            .Limit(1)
            .First() ?? CardSet.Empty;
    }

    public async Task<bool> SaveSet(string note, IList<Card> cards)
    {
        var iteration = cardSets.AsQueryable().Any() ? cardSets.AsQueryable().Max(cs => cs.Iteration) + 1 : 1;
        var task = cardSets.InsertOneAsync(new(iteration, new Rock(cards), note));
        return await task.Try();
    }
}