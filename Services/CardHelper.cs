
using System.Collections.ObjectModel;
using MongoDB.Bson;

namespace queensblood;

public static class CardHelper
{
    public const int MAX_PIN_COST = 3;
    public const int MAX_VALUE = 6;
    public const int MAX_NAME_LENGTH = 12;

    public static Card Blank { get => new(ObjectId.GenerateNewId(DateTime.Now), "Name", 1, 1, 0); }
    public static readonly Card Null = new(ObjectId.Empty, "", 0, 0, 0);

    public static void Change(ref Card card, string name = "", int pinCost = 0, int value = 0)
    {
        name = name.Length > MAX_NAME_LENGTH ? name[..MAX_NAME_LENGTH] : name;
        card = new(
            card.Id,
            name == "" ? card.Name : name,
            pinCost == 0 ? card.PinCost : Math.Clamp(pinCost, 1, MAX_PIN_COST),
            value == 0 ? card.Value : Math.Clamp(value, 1, MAX_VALUE),
            card.Boosts
        );
    }
}