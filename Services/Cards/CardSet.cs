using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
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
