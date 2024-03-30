using System.Collections.ObjectModel;
using System.Linq.Expressions;
using MongoDB.Driver;

namespace queensblood;

public record CardSet(int Iteration, DateTime Created, Card[] Cards, string Note)
{
    public static readonly CardSet Empty = new(0, DateTime.Now, [], "Initial");

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
    public int AddCard(Card card);
    public void UpdateCard(int index, Card card);
    public void DeleteCard(int index);
    public Task<bool> SaveLatestSet(string note);
    public CardSet GetLatestCardSet();
}

public class CardsMongoService : ICardsService
{
    private readonly List<Card> cards = [];
    private readonly IMongoDatabase db;
    private readonly IMongoCollection<CardSet> cardSets;

    public CardsMongoService(IMongoDatabase database)
    {
        db = database;
        cardSets = db.GetCollection<CardSet>("cardsets");
    }

    public int AddCard(Card card)
    {
        cards.Add(card);
        return cards.Count;
    }

    public void DeleteCard(int index)
    {
        if (index < 0 || index >= cards.Count) return;

        cards.RemoveAt(index);
    }

    public ReadOnlyCollection<CardSet> GetCardSets()
    {
        return cardSets.AsQueryable().OrderByDescending(cs => cs.Iteration).ToList().AsReadOnly();
    }

    public CardSet GetLatestCardSet()
    {
        return cardSets.AsQueryable().MaxBy(cs => cs.Iteration) ?? new(0, DateTime.Now, [], "Initial");
    }

    public async Task<bool> SaveLatestSet(string note)
    {
        var iteration = cardSets.AsQueryable().Any() ? cardSets.AsQueryable().Max(cs => cs.Iteration) + 1 : 1;
        var task = cardSets.InsertOneAsync(new(iteration, DateTime.Now, [.. cards], note));
        return await task.Try();
    }

    public void UpdateCard(int index, Card card)
    {
        cards[index] = card;
    }
}