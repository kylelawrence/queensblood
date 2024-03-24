using System.Collections.ObjectModel;
using System.Diagnostics;
using MongoDB.Bson;
using MongoDB.Driver;

namespace queensblood;

public record Card(ObjectId Id, string Name, int PinCost, int Value, int Boosts);

public interface ICardsService
{
    public ReadOnlyCollection<Card> GetAllCards();
    public Task<ChangeResult> UpsertCard(Card card, bool isNew);
    public Task<bool> DeleteCard(Card card);
}

public record ChangeResult(Card Card, bool Success);

public class CardsMongoService : ICardsService
{
    private static readonly ReplaceOptions replaceOptions = new() { IsUpsert = true };
    private readonly IMongoDatabase db;
    private readonly IMongoCollection<Card> collection;

    public CardsMongoService(IMongoDatabase database)
    {
        db = database;
        collection = db.GetCollection<Card>("cards");
    }

    public ReadOnlyCollection<Card> GetAllCards()
    {
        return collection.AsQueryable().ToList().AsReadOnly();
    }

    public async Task<ChangeResult> UpsertCard(Card card, bool isNew)
    {
        // Clamp the values, just in case
        CardHelper.Change(ref card, pinCost: 0);

        var builder = Builders<Card>.Filter;
        var filter = isNew ? builder.Eq(c => c.Value, 0) : builder.Eq(c => c.Id, card.Id);
        var result = await collection.ReplaceOneAsync(filter, card, replaceOptions);
        var newId = result.UpsertedId != null ? result.UpsertedId.AsObjectId : card.Id;
        var updated = new Card(newId, card.Name, card.PinCost, card.Value, card.Boosts);
        return new(updated, true);
    }

    public async Task<bool> DeleteCard(Card card)
    {
        var filter = Builders<Card>.Filter.Eq(c => c.Id, card.Id);
        var result = await collection.DeleteOneAsync(filter);
        return result.DeletedCount == 1;
    }
}