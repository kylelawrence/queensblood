using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Components;

namespace queensblood;

public record Card(int ID, string Name, int PinCost, int Value, int Boosts);

public interface ICardsService
{
    public ReadOnlyDictionary<int, Card> AllCards { get; }
    public void AddCard(string name, int pinCost, int value, int boosts);
    public void UpdateCard(int id, string name, int pinCost, int value, int boosts);
    public void DeleteCard(int id);
    public Task<bool> SaveCards();
}

public class CardsService : ICardsService
{
    public const string ALL_CARDS_FILES = "wwwroot/cards.tsv";
    public const char CARDS_DEF_SEPARATOR = '\t';
    private const StringSplitOptions splitOptions = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;

    private int nextCardID = 0;

    private readonly Dictionary<int, Card> allCards;

    public ReadOnlyDictionary<int, Card> AllCards { get; private set; }

    public CardsService(ICardsFileService fileService)
    {
        // Failing to read the cards file should just fail the whole app.
        var lines = File.Exists(ALL_CARDS_FILES) ? File.ReadAllLines(ALL_CARDS_FILES) : [];
        var cards = new Dictionary<int, Card>(lines.Length);
        var maxCardID = 0;
        foreach (var line in lines)
        {
            var parts = line.Split(CARDS_DEF_SEPARATOR, splitOptions);
            if (parts.Length != 4) continue;
            try
            {
                var id = int.Parse(parts[0]);
                var name = parts[1];
                var pinCost = int.Parse(parts[2]);
                var value = int.Parse(parts[3]);
                var boosts = int.Parse(parts[4]);
                ClampDetails(ref name, ref pinCost, ref value);
                cards[id] = new(id, name, pinCost, value, boosts);
                if (id > maxCardID)
                {
                    maxCardID = id;
                }
            }
            catch { }
        }

        nextCardID = maxCardID + 1;
        allCards = cards;
        AllCards = allCards.AsReadOnly();
    }

    public void AddCard(string name, int pinCost, int value, int boosts)
    {
        ClampDetails(ref name, ref pinCost, ref value);
        allCards[nextCardID] = new(nextCardID, name, pinCost, value, boosts);
        nextCardID++;
    }

    public void UpdateCard(int id, string name, int pinCost, int value, int boosts)
    {
        ClampDetails(ref name, ref pinCost, ref value);
        allCards[id] = new(id, name, pinCost, value, boosts);
    }

    public void DeleteCard(int id)
    {
        allCards.Remove(id);
    }

    public async Task<bool> SaveCards()
    {
        var lines = allCards.Select(pair =>
        {
            var card = pair.Value;
            var line = $"{card.ID}\t{card.Name}\t{card.PinCost}\t{card.Value}\t{card.Boosts}";
            return line;
        });

        return await File.WriteAllLinesAsync(ALL_CARDS_FILES, lines).Try();
    }

    private static void ClampDetails(ref string name, ref int pinCost, ref int value)
    {
        name = name[..12];
        pinCost = Math.Clamp(pinCost, 1, 3);
        value = Math.Clamp(value, 1, 6);
    }
}